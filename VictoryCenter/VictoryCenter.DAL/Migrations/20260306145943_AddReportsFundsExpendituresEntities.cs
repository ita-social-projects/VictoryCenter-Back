using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddReportsFundsExpendituresEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportFundsExpendituresCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFundsExpendituresCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportFundsExpendituresSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DisclaimerTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFundsExpendituresSettings", x => x.Id);
                    table.CheckConstraint("CK_ReportFundsExpendituresSettings_SingletonId", "Id = 1");
                });

            migrationBuilder.CreateTable(
                name: "ReportFundsExpendituresRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFundsExpendituresRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportFundsExpendituresRecords_ReportFundsExpendituresCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ReportFundsExpendituresCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportFundsExpendituresRecords_CategoryId",
                table: "ReportFundsExpendituresRecords",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportFundsExpendituresRecords");

            migrationBuilder.DropTable(
                name: "ReportFundsExpendituresSettings");

            migrationBuilder.DropTable(
                name: "ReportFundsExpendituresCategories");
        }
    }
}
