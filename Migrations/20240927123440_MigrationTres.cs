using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnicaDVPNetKubernetes.Migrations
{
    /// <inheritdoc />
    public partial class MigrationTres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_AspNetUsers_CreatedByUserId",
                table: "WorkTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_TaskStatusId",
                table: "WorkTasks");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "WorkTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
                name: "FK_WorkTasks_AspNetUsers_CreatedByUserId",
                table: "WorkTasks",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_TaskStatusId",
                table: "WorkTasks",
                column: "TaskStatusId",
                principalTable: "WorkTaskStatuses",
                principalColumn: "TaskStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_WorkTaskStatusTaskStatusId",
                table: "WorkTasks",
                column: "WorkTaskStatusTaskStatusId",
                principalTable: "WorkTaskStatuses",
                principalColumn: "TaskStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_AspNetUsers_CreatedByUserId",
                table: "WorkTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_TaskStatusId",
                table: "WorkTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_WorkTaskStatusTaskStatusId",
                table: "WorkTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkTasks_WorkTaskStatusTaskStatusId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "WorkTaskStatusTaskStatusId",
                table: "WorkTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_AspNetUsers_CreatedByUserId",
                table: "WorkTasks",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_WorkTaskStatuses_TaskStatusId",
                table: "WorkTasks",
                column: "TaskStatusId",
                principalTable: "WorkTaskStatuses",
                principalColumn: "TaskStatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
