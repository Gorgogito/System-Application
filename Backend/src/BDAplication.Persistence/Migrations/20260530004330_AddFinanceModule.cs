using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDAplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "typeconcept",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeconcept", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Concept = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TypeConceptId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsTransfer = table.Column<bool>(type: "bit", nullable: false),
                    TransferSourceDestiny = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movement_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movement_typeconcept_TypeConceptId",
                        column: x => x.TypeConceptId,
                        principalTable: "typeconcept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "transfer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceAccountId = table.Column<int>(type: "int", nullable: false),
                    SourceMovementId = table.Column<int>(type: "int", nullable: false),
                    DestinyAccountId = table.Column<int>(type: "int", nullable: false),
                    DestinyMovementId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transfer_account_DestinyAccountId",
                        column: x => x.DestinyAccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfer_account_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfer_movement_DestinyMovementId",
                        column: x => x.DestinyMovementId,
                        principalTable: "movement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfer_movement_SourceMovementId",
                        column: x => x.SourceMovementId,
                        principalTable: "movement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_Code",
                table: "account",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movement_AccountId",
                table: "movement",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_movement_Code",
                table: "movement",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movement_TypeConceptId",
                table: "movement",
                column: "TypeConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_DestinyAccountId",
                table: "transfer",
                column: "DestinyAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_DestinyMovementId",
                table: "transfer",
                column: "DestinyMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_SourceAccountId",
                table: "transfer",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_transfer_SourceMovementId",
                table: "transfer",
                column: "SourceMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_typeconcept_Code",
                table: "typeconcept",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transfer");

            migrationBuilder.DropTable(
                name: "movement");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "typeconcept");
        }
    }
}
