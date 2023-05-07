using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeededCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "countries",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Jamaica");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "countries",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Bahamas");
        }
    }
}
