using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreateReportProgramExpendituresRecordAndReportProgramExpendituresCategoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportProgramExpendituresCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProgramExpendituresCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportProgramExpendituresRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingYear = table.Column<int>(type: "int", nullable: false),
                    ProgramCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    AmountUah = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(13,2)", precision: 13, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProgramExpendituresRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportProgramExpendituresRecord_ReportProgramExpendituresCategories_ProgramCategoryId",
                        column: x => x.ProgramCategoryId,
                        principalTable: "ReportProgramExpendituresCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportProgramExpendituresRecord_ProgramCategoryId",
                table: "ReportProgramExpendituresRecord",
                column: "ProgramCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportProgramExpendituresRecord");

            migrationBuilder.DropTable(
                name: "ReportProgramExpendituresCategories");
        }
    }
}
