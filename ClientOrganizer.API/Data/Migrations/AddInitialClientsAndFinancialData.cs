using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientOrganizer.API.Migrations
{
    public partial class AddInitialClientsAndFinancialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name", "Address", "NipNb", "Email" },
                values: new object[,]
                {
                    { 1, "Acme Corp", "123 Main St", "1234567890", "acme@example.com" },
                    { 2, "Beta LLC", "456 Side Ave", "9876543210", "beta@example.com" },
                    { 3, "Gamma Sp. z o.o.", "789 Market Sq", "1122334455", "gamma@example.com" }
                });

            migrationBuilder.InsertData(
                table: "FinancialData",
                columns: new[] { "Id", "ClientId", "Month", "Year", "IncomeTax", "VAT", "InsuranceAmount" },
                values: new object[,]
                {
                    { 1, 1, 6, 2025, 1000.00m, 230.00m, 500.00m },
                    { 2, 1, 7, 2025, 1200.00m, 250.00m, 500.00m },
                    { 3, 2, 6, 2025, 900.00m, 210.00m, 400.00m },
                    { 4, 2, 7, 2025, 950.00m, 220.00m, 400.00m },
                    { 5, 3, 7, 2025, 1100.00m, 240.00m, 600.00m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FinancialData",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });
        }
    }
}
