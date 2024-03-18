using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicUIDemo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDefaultTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MusicList",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "Datetime()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "Datetime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MusicList",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "Datetime",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "Datetime()");
        }
    }
}
