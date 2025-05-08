using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSavvy.Migrations
{
    /// <inheritdoc />
    public partial class changeContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoImg",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoImg",
                table: "Contacts");
        }
    }
}
