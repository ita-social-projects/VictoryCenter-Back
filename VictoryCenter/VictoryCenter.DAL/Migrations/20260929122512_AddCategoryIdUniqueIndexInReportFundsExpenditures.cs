using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdUniqueIndexInReportFundsExpenditures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportFundsExpendituresRecords_CategoryId",
                table: "ReportFundsExpendituresRecords");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFundsExpendituresRecords_CategoryId",
                table: "ReportFundsExpendituresRecords",
                column: "CategoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportFundsExpendituresRecords_CategoryId",
                table: "ReportFundsExpendituresRecords");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFundsExpendituresRecords_CategoryId",
                table: "ReportFundsExpendituresRecords",
                column: "CategoryId");
        }
    }
}
