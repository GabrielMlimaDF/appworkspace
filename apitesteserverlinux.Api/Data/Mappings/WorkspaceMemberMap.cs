using apitesteserverlinux.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace apitesteserverlinux.Api.Data.Mappings;

public class WorkspaceMemberMap : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkspaceId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.WorkspaceId);
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new { x.WorkspaceId, x.UserId })
            .IsUnique();
    }
}