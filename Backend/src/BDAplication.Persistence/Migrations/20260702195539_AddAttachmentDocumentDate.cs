using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDAplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentDocumentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentDate",
                table: "Attachments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentDate",
                table: "Attachments");
        }
    }
}
