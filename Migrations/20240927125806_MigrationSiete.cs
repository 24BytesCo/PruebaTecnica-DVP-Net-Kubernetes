using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnicaDVPNetKubernetes.Migrations
{
    /// <inheritdoc />
    public partial class MigrationSiete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_AspNetUsers_UserId",
                table: "WorkTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkTasks_UserId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WorkTasks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_UserId",
                table: "WorkTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_AspNetUsers_UserId",
                table: "WorkTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
