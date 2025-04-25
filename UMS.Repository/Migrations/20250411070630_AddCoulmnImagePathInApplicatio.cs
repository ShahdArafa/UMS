using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCoulmnImagePathInApplicatio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Applications");
        }
    }
}
