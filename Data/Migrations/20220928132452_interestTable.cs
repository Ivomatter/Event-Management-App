using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NemeTschek.Data.Migrations
{
    public partial class interestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserInterest_Interest_InterestsId",
                table: "ApplicationUserInterest");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInterests_Interest_InterestId",
                table: "UsersInterests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interest",
                table: "Interest");

            migrationBuilder.RenameTable(
                name: "Interest",
                newName: "Interests");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interests",
                table: "Interests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserInterest_Interests_InterestsId",
                table: "ApplicationUserInterest",
                column: "InterestsId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInterests_Interests_InterestId",
                table: "UsersInterests",
                column: "InterestId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserInterest_Interests_InterestsId",
                table: "ApplicationUserInterest");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInterests_Interests_InterestId",
                table: "UsersInterests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interests",
                table: "Interests");

            migrationBuilder.RenameTable(
                name: "Interests",
                newName: "Interest");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interest",
                table: "Interest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserInterest_Interest_InterestsId",
                table: "ApplicationUserInterest",
                column: "InterestsId",
                principalTable: "Interest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInterests_Interest_InterestId",
                table: "UsersInterests",
                column: "InterestId",
                principalTable: "Interest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
