using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSavvy.Migrations
{
    /// <inheritdoc />
    public partial class Fix_RatingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_ProductId",
                table: "Ratings");
        }
    }
}
