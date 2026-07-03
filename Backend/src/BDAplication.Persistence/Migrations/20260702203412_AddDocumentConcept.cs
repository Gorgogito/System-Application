using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDAplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentConcept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentConceptId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentConcepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentConcepts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_DocumentConceptId",
                table: "Attachments",
                column: "DocumentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentConcepts_Code",
                table: "DocumentConcepts",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_DocumentConcepts_DocumentConceptId",
                table: "Attachments",
                column: "DocumentConceptId",
                principalTable: "DocumentConcepts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_DocumentConcepts_DocumentConceptId",
                table: "Attachments");

            migrationBuilder.DropTable(
                name: "DocumentConcepts");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_DocumentConceptId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DocumentConceptId",
                table: "Attachments");
        }
    }
}
