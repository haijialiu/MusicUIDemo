using Microsoft.EntityFrameworkCore;
using MusicUIDemo.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models.Database
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Music> Musics { get; set; }
        public DbSet<MusicList> MusicList { get; set; }
        public DbSet<MusicMusicList> MusicMusicLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\89422\source\repos\MusicUIDemo\MusicUIDemo\Models\Database\localmusic.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicMusicList>();
        }

    }
}
