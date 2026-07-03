using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDAplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeDocumentDateRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rellena registros históricos con su DateCreated antes de aplicar NOT NULL
            migrationBuilder.Sql(
                "UPDATE Attachments SET DocumentDate = DateCreated WHERE DocumentDate IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "Attachments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "Attachments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
