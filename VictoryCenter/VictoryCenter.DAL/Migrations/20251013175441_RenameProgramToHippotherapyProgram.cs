using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameProgramToHippotherapyProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Images_ImageId",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_ProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_Programs_ProgramsId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programs",
                table: "Programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgramCategories",
                table: "ProgramCategories");

            migrationBuilder.RenameTable(
                name: "Programs",
                newName: "HippotherapyPrograms");

            migrationBuilder.RenameTable(
                name: "ProgramCategories",
                newName: "HippotherapyProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_Programs_ImageId",
                table: "HippotherapyPrograms",
                newName: "IX_HippotherapyPrograms_ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HippotherapyPrograms",
                table: "HippotherapyPrograms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HippotherapyProgramCategories",
                table: "HippotherapyProgramCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HippotherapyPrograms_Images_ImageId",
                table: "HippotherapyPrograms",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_HippotherapyProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories",
                column: "CategoriesId",
                principalTable: "HippotherapyProgramCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_HippotherapyPrograms_ProgramsId",
                table: "ProgramsProgramCategories",
                column: "ProgramsId",
                principalTable: "HippotherapyPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HippotherapyPrograms_Images_ImageId",
                table: "HippotherapyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_HippotherapyProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_HippotherapyPrograms_ProgramsId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HippotherapyPrograms",
                table: "HippotherapyPrograms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HippotherapyProgramCategories",
                table: "HippotherapyProgramCategories");

            migrationBuilder.RenameTable(
                name: "HippotherapyPrograms",
                newName: "Programs");

            migrationBuilder.RenameTable(
                name: "HippotherapyProgramCategories",
                newName: "ProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_HippotherapyPrograms_ImageId",
                table: "Programs",
                newName: "IX_Programs_ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programs",
                table: "Programs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgramCategories",
                table: "ProgramCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Images_ImageId",
                table: "Programs",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
    }
}
