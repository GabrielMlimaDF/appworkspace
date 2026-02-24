namespace apitesteserverlinux.Api.Dtos.Workspace;
using System.ComponentModel.DataAnnotations;

public class CreateWorkspaceRequest
{
    [Required, MinLength(2), MaxLength(120)]
    public string Name { get; set; } = string.Empty;
}

