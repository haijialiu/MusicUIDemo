using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicUIDemo.Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models
{
    [EntityTypeConfiguration(typeof(MusicListEntityTypeConfiguration))]
    public class MusicList
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        [Required, Column(Order = 1)]
        public string Title { get; set; }
        [Required, Column(Order = 2)]
        public string Name { get; set; }
        [Required,Column(Order = 3)]
        public string Type { get; set; }
        [Column(Order = 4)]
        public DateTime CreatedTime { get; set; }

        public List<Music> Musics { get; set; } = [];
    }
    public class MusicListEntityTypeConfiguration : IEntityTypeConfiguration<MusicList>
    {
        public void Configure(EntityTypeBuilder<MusicList> builder)
        {
            builder
                .Property(musicList => musicList.CreatedTime)
                .HasDefaultValueSql("Datetime(CURRENT_TIMESTAMP,'localtime')");
            builder.Property(musicList => musicList.Type)
                .HasDefaultValue("user");

            builder
                .HasMany(musicList => musicList.Musics)
                .WithMany(music => music.MusicLists)
                .UsingEntity<MusicMusicList>();

            //builder.HasData(new MusicList()
            //{
            //    Id = 1,
            //    Title = "default",
            //    Name = "默认列表",
            //});
        }
    }
}
