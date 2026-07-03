using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDAplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlertHoursBeforeDue",
                table: "TaskBoard",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "TaskBoard",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TaskBoard",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoard_DueDate",
                table: "TaskBoard",
                column: "DueDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskBoard_DueDate",
                table: "TaskBoard");

            migrationBuilder.DropColumn(
                name: "AlertHoursBeforeDue",
                table: "TaskBoard");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "TaskBoard");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TaskBoard");
        }
    }
}
