using apitesteserverlinux.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WorkspaceMapping : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(w => w.OwnerUserId)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.HasIndex(w => w.OwnerUserId)
            .HasDatabaseName("IX_Workspaces_OwnerUserId");

        // Opcional (recomendado): evitar workspace duplicado com mesmo nome para o mesmo dono
        builder.HasIndex(w => new { w.OwnerUserId, w.Name })
            .IsUnique()
            .HasDatabaseName("IX_Workspaces_OwnerUserId_Name");
    }
}
