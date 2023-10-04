using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeworkTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedAttachement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Attachement",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Attachement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Attachement");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Attachement",
                newName: "id");
        }
    }
}
