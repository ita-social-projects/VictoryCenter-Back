using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReportProgramExpendituresRecordCategoryEntityAndMakeProgramExpendituresRecordEntityReferenceHippotherapyProgramCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportProgramExpendituresRecord_ReportProgramExpendituresCategories_ProgramCategoryId",
                table: "ReportProgramExpendituresRecord");

            migrationBuilder.DropTable(
                name: "ReportProgramExpendituresCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportProgramExpendituresRecord",
                table: "ReportProgramExpendituresRecord");

            migrationBuilder.RenameTable(
                name: "ReportProgramExpendituresRecord",
                newName: "ReportProgramExpendituresRecords");

            migrationBuilder.RenameColumn(
                name: "ProgramCategoryId",
                table: "ReportProgramExpendituresRecords",
                newName: "HippotherapyProgramCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportProgramExpendituresRecord_ProgramCategoryId",
                table: "ReportProgramExpendituresRecords",
                newName: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportProgramExpendituresRecords",
                table: "ReportProgramExpendituresRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportProgramExpendituresRecords_HippotherapyProgramCategories_HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecords",
                column: "HippotherapyProgramCategoryId",
                principalTable: "HippotherapyProgramCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportProgramExpendituresRecords_HippotherapyProgramCategories_HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportProgramExpendituresRecords",
                table: "ReportProgramExpendituresRecords");

            migrationBuilder.RenameTable(
                name: "ReportProgramExpendituresRecords",
                newName: "ReportProgramExpendituresRecord");

            migrationBuilder.RenameColumn(
                name: "HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecord",
                newName: "ProgramCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportProgramExpendituresRecords_HippotherapyProgramCategoryId",
                table: "ReportProgramExpendituresRecord",
                newName: "IX_ReportProgramExpendituresRecord_ProgramCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportProgramExpendituresRecord",
                table: "ReportProgramExpendituresRecord",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReportProgramExpendituresCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProgramExpendituresCategories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ReportProgramExpendituresRecord_ReportProgramExpendituresCategories_ProgramCategoryId",
                table: "ReportProgramExpendituresRecord",
                column: "ProgramCategoryId",
                principalTable: "ReportProgramExpendituresCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
