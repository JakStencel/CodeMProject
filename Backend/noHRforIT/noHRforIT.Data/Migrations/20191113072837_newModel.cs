using Microsoft.EntityFrameworkCore.Migrations;

namespace noHRforIT.Data.Migrations
{
    public partial class newModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDTO",
                table: "UserDTO");

            migrationBuilder.RenameTable(
                name: "UserDTO",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserDTO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDTO",
                table: "UserDTO",
                column: "Id");
        }
    }
}
