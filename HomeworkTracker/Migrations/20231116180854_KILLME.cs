using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeworkTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class KILLME : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Answer_AttachableId",
                table: "AttachedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Exercise_AttachableId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_AttachableId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "AttachableType",
                table: "AttachedFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttachableId",
                table: "AttachedFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerId",
                table: "AttachedFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExerciseId",
                table: "AttachedFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_AnswerId",
                table: "AttachedFiles",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_ExerciseId",
                table: "AttachedFiles",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Answer_AnswerId",
                table: "AttachedFiles",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Exercise_ExerciseId",
                table: "AttachedFiles",
                column: "ExerciseId",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Answer_AnswerId",
                table: "AttachedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AttachedFiles_Exercise_ExerciseId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_AnswerId",
                table: "AttachedFiles");

            migrationBuilder.DropIndex(
                name: "IX_AttachedFiles_ExerciseId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "AttachedFiles");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "AttachedFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttachableId",
                table: "AttachedFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachableType",
                table: "AttachedFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_AttachableId",
                table: "AttachedFiles",
                column: "AttachableId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Answer_AttachableId",
                table: "AttachedFiles",
                column: "AttachableId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachedFiles_Exercise_AttachableId",
                table: "AttachedFiles",
                column: "AttachableId",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
