using apitesteserverlinux.Api.Common;
using apitesteserverlinux.Api.Dtos.Users;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace apitesteserverlinux.Api.ControllerApi;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantMemberRepository _tenantMemberRepository;
    private readonly JwtTokenService _jwtTokenService;

    public UsersController(
        IUserRepository repository,
        ITenantRepository tenantRepository,
        ITenantMemberRepository tenantMemberRepository,
        JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _tenantRepository = tenantRepository;
        _tenantMemberRepository = tenantMemberRepository;
        _jwtTokenService = jwtTokenService;
    }
    //inicio

    // POST api/v1/users/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserRequest model)
    {
        try
        {
            if (await _repository.GetByEmailAsync(model.Email) is not null)
                return BadRequest(ApiResponse<string>.Fail(
                    "E-mail já cadastrado.",
                    new[] { "U1C1" }
                ));

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repository.PhoneNumberExistsAsync(model.PhoneNumber))
            {
                return BadRequest(ApiResponse<string>.Fail(
                    "O número de telefone já está em uso."
                ));
            }

            var passwordHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(model.Password)));

            var admin = new User(
                name: model.Name,
                email: model.Email,
                passwordHash: passwordHash,
                phoneNumber: model.PhoneNumber,
                role: UserRole.Admin
            );

            await _repository.AddAsync(admin);

            var tenant = new Tenant($"{admin.Name} - SaaS", admin.Id);
            await _tenantRepository.AddAsync(tenant);

            var tenantMember = new TenantMember(tenant.Id, admin.Id, TenantRole.Owner);
            await _tenantMemberRepository.AddAsync(tenantMember);

            var token = _jwtTokenService.CreateToken(
                userId: admin.Id,
                email: admin.Email,
                role: admin.Role.ToString(),
                tenantId: tenant.Id,
                tenantRole: tenantMember.Role.ToString(),
                workspaceId: null
            );

            return Ok(ApiResponse<object>.Ok(new
            {
                user = new UserResponse
                {
                    Id = admin.Id,
                    Name = admin.Name,
                    Email = admin.Email,
                    IsActive = admin.IsActive
                },
                tenant = new
                {
                    Id = tenant.Id,
                    Name = tenant.Name
                },
                token
            }, "Conta criada com sucesso. Você é o administrador do seu SaaS."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Ocorreu um erro inesperado ao criar a conta.",
                    new[] { "U1X1" }
                )
            );
        }
    }

    // GET api/v1/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _repository.GetByIdAsync(id);

            if (user is null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuário não encontrado.",
                    new[] { "U1G1" }
                ));

            return Ok(ApiResponse<UserResponse>.Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive
            }));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao buscar o usuário.",
                    new[] { "U1X1" }
                )
            );
        }
    }

    // GET api/v1/users
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _repository.GetAllAsync();

            var result = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                IsActive = u.IsActive
            });

            return Ok(ApiResponse<IEnumerable<UserResponse>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao listar usuários.",
                    new[] { "U1X1" }
                )
            );
        }
    }

    // POST api/v1/users
    // (este agora é o endpoint do Admin criar usuários comuns)
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest model)
    {
        try
        {
            if (await _repository.GetByEmailAsync(model.Email) is not null)
                return BadRequest(ApiResponse<string>.Fail(
                    "E-mail já cadastrado.",
                    new[] { "U1C1" }
                ));

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repository.PhoneNumberExistsAsync(model.PhoneNumber))
            {
                return BadRequest(ApiResponse<string>.Fail(
                    "O número de telefone já está em uso."
                ));
            }

            var passwordHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(model.Password)));

            var user = new User(
                name: model.Name,
                email: model.Email,
                passwordHash: passwordHash,
                phoneNumber: model.PhoneNumber,
                role: UserRole.User // ✅ usuário comum
            );

            await _repository.AddAsync(user);

            return Ok(ApiResponse<UserResponse>.Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive
            }, "Usuário criado com sucesso."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Ocorreu um erro inesperado ao criar o usuário.",
                    new[] { "U1X1" }
                )
            );
        }
    }

    // PUT api/v1/users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest model)
    {
        try
        {
            var user = await _repository.GetByIdAsync(id);

            if (user is null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuário não encontrado.",
                    new[] { "U1U1" }
                ));

            typeof(User).GetProperty("Name")!.SetValue(user, model.Name.Trim());
            typeof(User).GetProperty("IsActive")!.SetValue(user, model.IsActive);

            await _repository.UpdateAsync(user);

            return Ok(ApiResponse<string>.Ok("Usuário atualizado com sucesso."));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao atualizar o usuário.",
                    new[] { "U1X1" }
                )
            );
        }
    }

    // DELETE api/v1/users/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await _repository.GetByIdAsync(id);

            if (user is null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuário não encontrado.",
                    new[] { "U1D1" }
                ));

            await _repository.DeleteAsync(id);

            return Ok(ApiResponse<string>.Ok("Usuário removido com sucesso."));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao remover o usuário.",
                    new[] { "U1X1" }
                )
            );
        }
    }

}
