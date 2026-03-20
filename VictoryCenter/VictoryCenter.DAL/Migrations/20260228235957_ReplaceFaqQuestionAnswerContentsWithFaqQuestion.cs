using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFaqQuestionAnswerContentsWithFaqQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "ProgramSectionContents");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "ProgramSectionContents");

            migrationBuilder.AddColumn<long>(
                name: "FaqQuestionId",
                table: "ProgramSectionContents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSectionContents_FaqQuestionId",
                table: "ProgramSectionContents",
                column: "FaqQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramSectionContents_FaqQuestions_FaqQuestionId",
                table: "ProgramSectionContents",
                column: "FaqQuestionId",
                principalTable: "FaqQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramSectionContents_FaqQuestions_FaqQuestionId",
                table: "ProgramSectionContents");

            migrationBuilder.DropIndex(
                name: "IX_ProgramSectionContents_FaqQuestionId",
                table: "ProgramSectionContents");

            migrationBuilder.DropColumn(
                name: "FaqQuestionId",
                table: "ProgramSectionContents");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "ProgramSectionContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "ProgramSectionContents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
