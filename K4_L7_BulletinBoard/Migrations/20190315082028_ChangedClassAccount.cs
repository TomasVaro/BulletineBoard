using Microsoft.EntityFrameworkCore.Migrations;

namespace K4_L7_BulletinBoard.Migrations
{
    public partial class ChangedClassAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Account",
                newName: "Username");

            migrationBuilder.AlterColumn<int>(
                name: "Like",
                table: "Post",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Account",
                newName: "UserName");

            migrationBuilder.AlterColumn<int>(
                name: "Like",
                table: "Post",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
