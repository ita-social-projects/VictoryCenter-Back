using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishedReportFundsExpendituresSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishedReportFundsExpendituresRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceRecordId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryNameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedReportFundsExpendituresRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishedReportFundsExpendituresSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisclaimerTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisclaimerTitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProgramExpendituresReportingYear = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedReportFundsExpendituresSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishedReportProgramExpendituresRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceRecordId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryNameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedReportProgramExpendituresRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedReportFundsExpendituresRecords");

            migrationBuilder.DropTable(
                name: "PublishedReportFundsExpendituresSnapshots");

            migrationBuilder.DropTable(
                name: "PublishedReportProgramExpendituresRecords");
        }
    }
}
