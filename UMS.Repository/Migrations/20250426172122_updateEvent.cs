using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "eventPosts");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "eventPosts",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "eventPosts");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "eventPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
