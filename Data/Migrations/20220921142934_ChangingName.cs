using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NemeTschek.Data.Migrations
{
    public partial class ChangingName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoLocation",
                table: "AspNetUsers",
                newName: "PhotoDirectory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoDirectory",
                table: "AspNetUsers",
                newName: "PhotoLocation");
        }
    }
}
