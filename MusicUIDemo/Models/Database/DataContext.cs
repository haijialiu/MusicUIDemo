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
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\89422\source\repos\MusicUIDemo\MusicUIDemo\Models\Database\localmusic.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            var files_path = Directory.GetFiles(folderPath);

            List<Music> list = [];

            foreach (var file_path in files_path)
            {

                var item = MediaInfo.GetMusic(file_path);
                item.Id = list.Count + 1;
                list.Add(item);

            }

            modelBuilder.Entity<Music>().HasData(list);
        }

    }
}
