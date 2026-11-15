using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiWorkspace.Infrastructure.Persistence.Migrations.Ledger
{
    /// <inheritdoc />
    public partial class AddLedgerTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    SourceAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DestinationAccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<string>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransactions_IdempotencyKey",
                table: "LedgerTransactions",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerTransactions");
        }
    }
}
