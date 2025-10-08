using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class changeProgramCategoryToHypotherapyProgramCategory : Migration
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
                newName: "HypotherapyPrograms");

            migrationBuilder.RenameTable(
                name: "ProgramCategories",
                newName: "HypotherapyProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_Programs_ImageId",
                table: "HypotherapyPrograms",
                newName: "IX_HypotherapyPrograms_ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HypotherapyPrograms",
                table: "HypotherapyPrograms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HypotherapyProgramCategories",
                table: "HypotherapyProgramCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HypotherapyPrograms_Images_ImageId",
                table: "HypotherapyPrograms",
                column: "ImageId",
                principalSchema: "media",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_HypotherapyProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories",
                column: "CategoriesId",
                principalTable: "HypotherapyProgramCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramsProgramCategories_HypotherapyPrograms_ProgramsId",
                table: "ProgramsProgramCategories",
                column: "ProgramsId",
                principalTable: "HypotherapyPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HypotherapyPrograms_Images_ImageId",
                table: "HypotherapyPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_HypotherapyProgramCategories_CategoriesId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramsProgramCategories_HypotherapyPrograms_ProgramsId",
                table: "ProgramsProgramCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HypotherapyPrograms",
                table: "HypotherapyPrograms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HypotherapyProgramCategories",
                table: "HypotherapyProgramCategories");

            migrationBuilder.RenameTable(
                name: "HypotherapyPrograms",
                newName: "Programs");

            migrationBuilder.RenameTable(
                name: "HypotherapyProgramCategories",
                newName: "ProgramCategories");

            migrationBuilder.RenameIndex(
                name: "IX_HypotherapyPrograms_ImageId",
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
