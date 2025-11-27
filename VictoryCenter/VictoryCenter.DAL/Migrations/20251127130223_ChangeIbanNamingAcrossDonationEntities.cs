using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIbanNamingAcrossDonationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Iban",
                table: "UahBankDetails",
                newName: "UkrainianIban");

            migrationBuilder.RenameColumn(
                name: "Iban",
                table: "ForeignBankDetails",
                newName: "UkrainianIban");

            migrationBuilder.RenameColumn(
                name: "Iban",
                table: "CorrespondentBankDetails",
                newName: "ForeignIban");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UkrainianIban",
                table: "UahBankDetails",
                newName: "Iban");

            migrationBuilder.RenameColumn(
                name: "UkrainianIban",
                table: "ForeignBankDetails",
                newName: "Iban");

            migrationBuilder.RenameColumn(
                name: "ForeignIban",
                table: "CorrespondentBankDetails",
                newName: "Iban");
        }
    }
}
