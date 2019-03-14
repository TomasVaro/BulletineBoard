using Microsoft.EntityFrameworkCore.Migrations;

namespace K4_L7_BulletinBoard.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Cathegory_CategoryID",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cathegory",
                table: "Cathegory");

            migrationBuilder.RenameTable(
                name: "Cathegory",
                newName: "Category");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Category_CategoryID",
                table: "Post",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Category_CategoryID",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Cathegory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cathegory",
                table: "Cathegory",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Cathegory_CategoryID",
                table: "Post",
                column: "CategoryID",
                principalTable: "Cathegory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
