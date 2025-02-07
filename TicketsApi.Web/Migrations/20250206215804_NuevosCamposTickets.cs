using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketsApi.Web.Migrations
{
    public partial class NuevosCamposTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAsign",
                table: "TicketCabs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAsignName",
                table: "TicketCabs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAsign",
                table: "TicketCabs");

            migrationBuilder.DropColumn(
                name: "UserAsignName",
                table: "TicketCabs");
        }
    }
}
