using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSavvy.Migrations
{
    public partial class changStar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Trước tiên: nếu dữ liệu không đảm bảo là số thì nên kiểm tra trước ở SQL
            // ALTER TABLE đổi kiểu dữ liệu cột Star từ nvarchar sang int
            migrationBuilder.AlterColumn<int>(
                name: "Star",
                table: "Ratings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khôi phục lại kiểu dữ liệu từ int về nvarchar nếu rollback
            migrationBuilder.AlterColumn<string>(
                name: "Star",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
