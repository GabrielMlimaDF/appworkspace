using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apitesteserverlinux.Api.Migrations
{
    /// <inheritdoc />
    public partial class WorkspaceTenantFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Workspaces",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_OwnerUserId_Name",
                table: "Workspaces");

            migrationBuilder.RenameTable(
                name: "Workspaces",
                newName: "workspaces");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_OwnerUserId",
                table: "workspaces",
                newName: "IX_workspaces_OwnerUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "workspaces",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "workspaces",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_workspaces",
                table: "workspaces",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_members",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_members", x => new { x.TenantId, x.UserId });
                    table.ForeignKey(
                        name: "FK_tenant_members_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tenant_members_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workspaces_TenantId",
                table: "workspaces",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_members_UserId",
                table: "tenant_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_OwnerUserId",
                table: "tenants",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_members");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workspaces",
                table: "workspaces");

            migrationBuilder.DropIndex(
                name: "IX_workspaces_TenantId",
                table: "workspaces");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "workspaces");

            migrationBuilder.RenameTable(
                name: "workspaces",
                newName: "Workspaces");

            migrationBuilder.RenameIndex(
                name: "IX_workspaces_OwnerUserId",
                table: "Workspaces",
                newName: "IX_Workspaces_OwnerUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Workspaces",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workspaces",
                table: "Workspaces",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_OwnerUserId_Name",
                table: "Workspaces",
                columns: new[] { "OwnerUserId", "Name" },
                unique: true);
        }
    }
}
