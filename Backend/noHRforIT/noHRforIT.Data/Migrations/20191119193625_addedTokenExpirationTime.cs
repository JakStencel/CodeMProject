using Microsoft.EntityFrameworkCore.Migrations;

namespace noHRforIT.Data.Migrations
{
    public partial class addedTokenExpirationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TokenExpirationTime",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenExpirationTime",
                table: "AspNetUsers");
        }
    }
}
