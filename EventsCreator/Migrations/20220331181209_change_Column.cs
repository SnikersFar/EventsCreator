using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsCreator.Migrations
{
    public partial class change_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                newName: "RefreshTokenTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenTime",
                table: "Users",
                newName: "RefreshTokenExpiryTime");
        }
    }
}
