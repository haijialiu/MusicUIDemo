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
            string dbPath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), "localmusic.sqlite");
            string connectionString = $"Data Source={dbPath}";

            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicMusicList>();
        }
    }
}