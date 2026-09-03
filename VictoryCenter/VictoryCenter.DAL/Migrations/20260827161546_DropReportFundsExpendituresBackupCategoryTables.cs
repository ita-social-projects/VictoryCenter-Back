using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DropReportFundsExpendituresBackupCategoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackupReportFundsExpendituresRecords_BackupReportFundsExpendituresCategories_CategoryId",
                table: "BackupReportFundsExpendituresRecords");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresCategoryLocalizations");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresCategories");

            migrationBuilder.DropIndex(
                name: "IX_BackupReportFundsExpendituresRecords_CategoryId",
                table: "BackupReportFundsExpendituresRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresCategoryLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresCategoryLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_BackupReportFundsExpendituresCategoryLocalizations_BackupReportFundsExpendituresCategories_EntityId",
                        column: x => x.EntityId,
                        principalTable: "BackupReportFundsExpendituresCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackupReportFundsExpendituresCategoryLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportFundsExpendituresRecords_CategoryId",
                table: "BackupReportFundsExpendituresRecords",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportFundsExpendituresCategoryLocalizations_LanguageId",
                table: "BackupReportFundsExpendituresCategoryLocalizations",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackupReportFundsExpendituresRecords_BackupReportFundsExpendituresCategories_CategoryId",
                table: "BackupReportFundsExpendituresRecords",
                column: "CategoryId",
                principalTable: "BackupReportFundsExpendituresCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
