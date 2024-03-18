using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicUIDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicListTYpe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MusicList",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 17, 2, 27, 33, 328, DateTimeKind.Local).AddTicks(763),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 3, 12, 2, 20, 31, 289, DateTimeKind.Local).AddTicks(718));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "MusicList",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "MusicList");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MusicList",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 2, 20, 31, 289, DateTimeKind.Local).AddTicks(718),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2024, 3, 17, 2, 27, 33, 328, DateTimeKind.Local).AddTicks(763));
        }
    }
}
