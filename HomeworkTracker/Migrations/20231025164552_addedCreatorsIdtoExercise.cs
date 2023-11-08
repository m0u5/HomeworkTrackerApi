using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeworkTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class addedCreatorsIdtoExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorsId",
                table: "Exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorsId",
                table: "Exercise");
        }
    }
}
