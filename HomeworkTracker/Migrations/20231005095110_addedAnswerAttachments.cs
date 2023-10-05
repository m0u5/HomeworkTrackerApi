using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeworkTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class addedAnswerAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachement_Answer_AnswerId",
                table: "Attachement");

            migrationBuilder.DropIndex(
                name: "IX_Attachement_AnswerId",
                table: "Attachement");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Attachement");

            migrationBuilder.CreateTable(
                name: "AnswerAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerAttachments_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerAttachments_AnswerId",
                table: "AnswerAttachments",
                column: "AnswerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerAttachments");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerId",
                table: "Attachement",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Attachement_AnswerId",
                table: "Attachement",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachement_Answer_AnswerId",
                table: "Attachement",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
