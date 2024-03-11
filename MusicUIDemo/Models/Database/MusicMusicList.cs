using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models.Database
{
    //[EntityTypeConfiguration(typeof(MusicMusicListEntityTypeConfiguration))]
    public class MusicMusicList
    {
        public int MusicId { get; set; }
        public int MusicListId { get; set; }

        //public class MusicMusicListEntityTypeConfiguration : IEntityTypeConfiguration<MusicMusicList>
        //{
        //    public void Configure(EntityTypeBuilder<MusicMusicList> builder)
        //    {
        //        //builder.HasData([
        //        //    new MusicMusicList() { MusicId=1,MusicListId=1},
        //        //    new MusicMusicList() { MusicId=2,MusicListId=1},
        //        //    new MusicMusicList() { MusicId=3,MusicListId=1},
        //        //    new MusicMusicList() { MusicId=4,MusicListId=1},
        //        //    new MusicMusicList() { MusicId=5,MusicListId=1},
        //        //    ]);
        //    }

        //}
    }
}
