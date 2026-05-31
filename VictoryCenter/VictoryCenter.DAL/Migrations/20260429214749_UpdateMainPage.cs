using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMainPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImpactStatistics_MainPageId",
                table: "ImpactStatistics");

            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "Metrics",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ImpactStatistics",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PdfSectionLocalizations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PdfSectionLocalizations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "Metrics",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Prefix",
                table: "Metrics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Metrics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ImpactStatisticsLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactStatisticsLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ImpactStatisticsLocalizations_ImpactStatistics_EntityId",
                        column: x => x.EntityId,
                        principalTable: "ImpactStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImpactStatisticsLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetricLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_MetricLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricLocalizations_Metrics_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpactStatistics_MainPageId",
                table: "ImpactStatistics",
                column: "MainPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImpactStatisticsLocalizations_LanguageId",
                table: "ImpactStatisticsLocalizations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricLocalizations_LanguageId",
                table: "MetricLocalizations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpactStatisticsLocalizations");

            migrationBuilder.DropTable(
                name: "MetricLocalizations");

            migrationBuilder.DropIndex(
                name: "IX_ImpactStatistics_MainPageId",
                table: "ImpactStatistics");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Metrics");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Metrics",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ImpactStatistics",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PdfSectionLocalizations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PdfSectionLocalizations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Metrics",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ImpactStatistics_MainPageId",
                table: "ImpactStatistics",
                column: "MainPageId");
        }
    }
}
