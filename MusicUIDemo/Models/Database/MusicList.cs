using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models.Database
{
    public class MusicList
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Music> Musics { get; set; } = [];

        public DateTime CreatedTime { get; set; }
    }
    public class MusicListEntityTypeConfiguration : IEntityTypeConfiguration<MusicList>
    {
        public void Configure(EntityTypeBuilder<MusicList> builder)
        {
            builder
                .Property(musicList => musicList.CreatedTime)
                .HasDefaultValueSql("datetime()"); //sqlite
        }
    }
}
