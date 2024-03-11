using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicUIDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValue: new DateTime(2024, 3, 12, 2, 20, 31, 289, DateTimeKind.Local).AddTicks(718))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "musics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true, comment: "歌名"),
                    Artist = table.Column<string>(type: "TEXT", nullable: true, comment: "歌手"),
                    Album = table.Column<string>(type: "TEXT", nullable: true, comment: "专辑名"),
                    Time = table.Column<double>(type: "REAL", nullable: false, comment: "时长（秒）"),
                    AlbumImage = table.Column<byte[]>(type: "BLOB", nullable: true, comment: "专辑图"),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "文件路径")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_musics", x => x.Id);
                },
                comment: "音乐库");

            migrationBuilder.CreateTable(
                name: "MusicMusicLists",
                columns: table => new
                {
                    MusicId = table.Column<int>(type: "INTEGER", nullable: false),
                    MusicListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicMusicLists", x => new { x.MusicId, x.MusicListId });
                    table.ForeignKey(
                        name: "FK_MusicMusicLists_MusicList_MusicListId",
                        column: x => x.MusicListId,
                        principalTable: "MusicList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicMusicLists_musics_MusicId",
                        column: x => x.MusicId,
                        principalTable: "musics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicMusicLists_MusicListId",
                table: "MusicMusicLists",
                column: "MusicListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicMusicLists");

            migrationBuilder.DropTable(
                name: "MusicList");

            migrationBuilder.DropTable(
                name: "musics");
        }
    }
}
