using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnicaDVPNetKubernetes.Migrations
{
    public partial class MigrationDos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_AspNetUsers_AssignedToUserId",
                table: "WorkTasks");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AspNetRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_AspNetUsers_AssignedToUserId",
                table: "WorkTasks",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_AspNetUsers_AssignedToUserId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AspNetRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_AspNetUsers_AssignedToUserId",
                table: "WorkTasks",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
