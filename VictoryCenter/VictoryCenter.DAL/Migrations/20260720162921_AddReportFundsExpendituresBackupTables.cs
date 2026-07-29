using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddReportFundsExpendituresBackupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DisclaimerTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ProgramExpendituresReportingYear = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupReportProgramExpendituresRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    HippotherapyProgramCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportProgramExpendituresRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackupReportProgramExpendituresRecords_HippotherapyProgramCategories_HippotherapyProgramCategoryId",
                        column: x => x.HippotherapyProgramCategoryId,
                        principalTable: "HippotherapyProgramCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresCategoryLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackupReportFundsExpendituresRecords_BackupReportFundsExpendituresCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BackupReportFundsExpendituresCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackupReportFundsExpendituresSettingsLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    DisclaimerTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupReportFundsExpendituresSettingsLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_BackupReportFundsExpendituresSettingsLocalizations_BackupReportFundsExpendituresSettings_EntityId",
                        column: x => x.EntityId,
                        principalTable: "BackupReportFundsExpendituresSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackupReportFundsExpendituresSettingsLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportFundsExpendituresCategoryLocalizations_LanguageId",
                table: "BackupReportFundsExpendituresCategoryLocalizations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportFundsExpendituresRecords_CategoryId",
                table: "BackupReportFundsExpendituresRecords",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportFundsExpendituresSettingsLocalizations_LanguageId",
                table: "BackupReportFundsExpendituresSettingsLocalizations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_BackupReportProgramExpendituresRecords_HippotherapyProgramCategoryId",
                table: "BackupReportProgramExpendituresRecords",
                column: "HippotherapyProgramCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresCategoryLocalizations");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresRecords");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresSettingsLocalizations");

            migrationBuilder.DropTable(
                name: "BackupReportProgramExpendituresRecords");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresCategories");

            migrationBuilder.DropTable(
                name: "BackupReportFundsExpendituresSettings");
        }
    }
}
