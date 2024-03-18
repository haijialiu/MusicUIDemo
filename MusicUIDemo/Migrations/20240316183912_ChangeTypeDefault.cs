using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicUIDemo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MusicList",
                type: "TEXT",
                nullable: true,
                defaultValue: "'user'",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValue: "user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "MusicList",
                type: "TEXT",
                nullable: true,
                defaultValue: "user",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValue: "'user'");
        }
    }
}
