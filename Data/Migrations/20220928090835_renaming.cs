using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NemeTschek.Data.Migrations
{
    public partial class renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimalPeopleCount",
                table: "Event",
                newName: "MinPeople");

            migrationBuilder.RenameColumn(
                name: "MaximumPeopleCount",
                table: "Event",
                newName: "MaxPeople");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinPeople",
                table: "Event",
                newName: "MinimalPeopleCount");

            migrationBuilder.RenameColumn(
                name: "MaxPeople",
                table: "Event",
                newName: "MaximumPeopleCount");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
