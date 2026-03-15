using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apitesteserverlinux.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantOwnerForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_workspaces_tenants_TenantId",
                table: "workspaces",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_workspaces_tenants_TenantId",
                table: "workspaces");
        }
    }
}
