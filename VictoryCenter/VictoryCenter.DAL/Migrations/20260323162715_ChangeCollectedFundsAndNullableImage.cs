using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCollectedFundsAndNullableImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CollectedFundsBlocks_ImageId",
                table: "CollectedFundsBlocks");

            migrationBuilder.DropIndex(
                name: "IX_ChangedLivesBlocks_ImageId",
                table: "ChangedLivesBlocks");

            migrationBuilder.DropColumn(
                name: "CollectedAmount",
                table: "CollectedFundsBlocks");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "CollectedFundsBlocks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "ChangedLivesBlocks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_CollectedFundsBlocks_ImageId",
                table: "CollectedFundsBlocks",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChangedLivesBlocks_ImageId",
                table: "ChangedLivesBlocks",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CollectedFundsBlocks_ImageId",
                table: "CollectedFundsBlocks");

            migrationBuilder.DropIndex(
                name: "IX_ChangedLivesBlocks_ImageId",
                table: "ChangedLivesBlocks");

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "CollectedFundsBlocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollectedAmount",
                table: "CollectedFundsBlocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "ImageId",
                table: "ChangedLivesBlocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectedFundsBlocks_ImageId",
                table: "CollectedFundsBlocks",
                column: "ImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangedLivesBlocks_ImageId",
                table: "ChangedLivesBlocks",
                column: "ImageId",
                unique: true);
        }
    }
}
