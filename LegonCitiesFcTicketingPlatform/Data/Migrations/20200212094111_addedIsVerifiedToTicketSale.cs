using Microsoft.EntityFrameworkCore.Migrations;

namespace LegonCitiesFcTicketingPlatform.Data.Migrations
{
    public partial class addedIsVerifiedToTicketSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "TicketsSales",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "TicketsSales");
        }
    }
}
