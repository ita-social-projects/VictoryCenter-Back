using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramProgramCategories_ProgramCategories_CategoriesId",
                table: "ProgramProgramCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramProgramCategories_Programs_ProgramsId",
                table: "ProgramProgramCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgramProgramCategories",
                table: "ProgramProgramCategories");

            migrationBuilder.RenameTable(
                name: "ProgramProgramCategories",
                newName: "ProgramsProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramProgramCategories_ProgramsId",
                table: "ProgramsProgramCategories",
                newName: "IX_ProgramsProgramCategories_ProgramsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgramsProgramCategories",
                table: "ProgramsProgramCategories",
                columns: new[] { "CategoriesId", "ProgramsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_ProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories",
                column: "CategoriesId",
                principalTable: "ProgramCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_Programs_ProgramsId",
                table: "ProgramsProgramCategories",
                column: "ProgramsId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_ProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_Programs_ProgramsId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgramsProgramCategories",
                table: "ProgramsProgramCategories");

            migrationBuilder.RenameTable(
                name: "ProgramsProgramCategories",
                newName: "ProgramProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramsProgramCategories_ProgramsId",
                table: "ProgramProgramCategories",
                newName: "IX_ProgramProgramCategories_ProgramsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgramProgramCategories",
                table: "ProgramProgramCategories",
                columns: new[] { "CategoriesId", "ProgramsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramProgramCategories_ProgramCategories_CategoriesId",
                table: "ProgramProgramCategories",
                column: "CategoriesId",
                principalTable: "ProgramCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramProgramCategories_Programs_ProgramsId",
                table: "ProgramProgramCategories",
                column: "ProgramsId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
