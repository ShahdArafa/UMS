using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddtableNotificationPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationPreference_Users_UserId",
                table: "NotificationPreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationPreference",
                table: "NotificationPreference");

            migrationBuilder.RenameTable(
                name: "NotificationPreference",
                newName: "NotificationPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationPreference_UserId",
                table: "NotificationPreferences",
                newName: "IX_NotificationPreferences_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationPreferences",
                table: "NotificationPreferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationPreferences_Users_UserId",
                table: "NotificationPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationPreferences_Users_UserId",
                table: "NotificationPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationPreferences",
                table: "NotificationPreferences");

            migrationBuilder.RenameTable(
                name: "NotificationPreferences",
                newName: "NotificationPreference");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreference",
                newName: "IX_NotificationPreference_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationPreference",
                table: "NotificationPreference",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationPreference_Users_UserId",
                table: "NotificationPreference",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
