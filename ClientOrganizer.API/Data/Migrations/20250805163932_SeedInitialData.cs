using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientOrganizer.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Address", "NipNb", "Email" },
                values: new object[,]
                {
                        { 1, "ERKSoft", "Parkowa, Wysoka", "7521405418", "rafal.kiedzierski@gmail.com" },
                        { 2, "Beta LLC", "456 Side Ave", "9876543210", "beta@example.com" },
                        { 3, "Gamma Sp. z o.o.", "789 Market Sq", "1122334455", "gamma@example.com" }
                });

            migrationBuilder.InsertData(
                table: "FinancialData",
                columns: new[] { "Id", "ClientId", "Month", "Year", "IncomeTax", "Vat", "InsuranceAmount" },
                values: new object[,]
                {
                        { 1, 1, 1, 2025, 1000.00m, 230.00m, 500.00m },
                        { 2, 1, 2, 2025, 1100.00m, 240.00m, 510.00m },
                        { 3, 1, 3, 2025, 1200.00m, 250.00m, 520.00m },
                        { 4, 1, 4, 2025, 1300.00m, 260.00m, 530.00m },

                        { 5, 2, 1, 2025, 900.00m, 210.00m, 400.00m },
                        { 6, 2, 2, 2025, 950.00m, 220.00m, 410.00m },
                        { 7, 2, 3, 2025, 1000.00m, 230.00m, 420.00m },
                        { 8, 2, 4, 2025, 1050.00m, 240.00m, 430.00m },

                        { 9, 3, 1, 2025, 1100.00m, 240.00m, 600.00m },
                        { 10, 3, 2, 2025, 1150.00m, 245.00m, 610.00m },
                        { 11, 3, 3, 2025, 1200.00m, 250.00m, 620.00m },
                        { 12, 3, 4, 2025, 1250.00m, 255.00m, 630.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FinancialData",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });
        }
    }
}
