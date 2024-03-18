using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.common;
using MusicUIDemo.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models
{
    [Table("musics")]
    [Comment("音乐库")]
    [EntityTypeConfiguration(typeof(MusicEntityTypeConfiguration))]
    [method: SetsRequiredMembers]
    public class Music(string title, string filePath)
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1), Comment("歌名")]
        public required string Title { get; set; } = title;

        [Column(Order = 2), Comment("歌手")]
        public string Artist { get; set; }

        [Column(Order = 3), Comment("专辑名")]
        public string Album { get; set; }

        [Column(Order = 4), Comment("时长（秒）")]
        public double Time { get; set; }

        [Column(Order = 5), Comment("专辑图")]
        public byte[] AlbumImage { get; set; }

        [Column(Order = 6), Required, MaxLength(200), Comment("文件路径")]
        public required string FilePath { get; set; } = filePath;

        [method: SetsRequiredMembers]
        public Music(int id, string title, string artist, string album, long time, byte[] albumImage, string filePath) : this(title, filePath)
        {
            Id = id;
            Title = title;
            Artist = artist;
            Album = album;
            Time = time;
            AlbumImage = albumImage;
            FilePath = filePath;
        }

        public List<MusicList> MusicLists { get; set; } = [];
    }
    public class MusicEntityTypeConfiguration : IEntityTypeConfiguration<Music>
    {
        public void Configure(EntityTypeBuilder<Music> builder)
        {
            builder
                .HasMany(m => m.MusicLists)
                .WithMany(list => list.Musics)
                .UsingEntity<MusicMusicList>();


            //var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            //var files_path = Directory.GetFiles(folderPath);
            //List<Music> list = [];

            //foreach (var file_path in files_path)
            //{
            //    var item = MediaInfo.GetMusic(file_path);
            //    item.Id = list.Count + 1;
            //    list.Add(item);
            //}
            //builder.HasData(list);
        }
    }
}
