using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo01.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeatStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TripSeatInventories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TripSeatInventories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
