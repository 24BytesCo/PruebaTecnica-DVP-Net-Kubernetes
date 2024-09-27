using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnicaDVPNetKubernetes.Migrations
{
    /// <inheritdoc />
    public partial class MigrationCinco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_WorkTaskStatusTaskStatusId",
                table: "WorkTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkTasks_WorkTaskStatusTaskStatusId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "WorkTaskStatusTaskStatusId",
                table: "WorkTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkTaskStatusTaskStatusId",
                table: "WorkTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_WorkTaskStatusTaskStatusId",
                table: "WorkTasks",
                column: "WorkTaskStatusTaskStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_WorkTaskStatusTaskStatusId",
                table: "WorkTasks",
                column: "WorkTaskStatusTaskStatusId",
                principalTable: "WorkTaskStatuses",
                principalColumn: "TaskStatusId");
        }
    }
}
