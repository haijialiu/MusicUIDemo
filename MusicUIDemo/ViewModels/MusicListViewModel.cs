using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


namespace MusicUIDemo.ViewModels
{

    public class MusicListViewModel
    {

        private static readonly MusicListViewModel instance= new();
        public static MusicListViewModel GetIntance() => instance;
        
        private MusicListViewModel() 
        {

            var context = new DataContext();

            var musics = context.Musics;


            //var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            //var files_path = Directory.GetFiles(folderPath);
            //int index = 1;

            //foreach (var file_path in files_path)
            //{
            //    var item = MediaInfo.GetMusic(file_path);
            //    item.Id = index;
            //    index++;
            //    musics.Add(item);
            //}
            //var musiclist = context.MusicList;
            //musiclist.Add(new Models.Database.MusicList() { Id = 1,Title="default",Name="默认歌单",CreatedTime=DateTime.Now  });
            //var musicMusiclist = context.MusicMusicLists;
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 1, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 2, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 3, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 4, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 5, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 6, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 7, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 8, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 9, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 10, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 11, MusicListId = 1 });
            //musicMusiclist.Add(new MusicMusicList() { MusicId = 12, MusicListId = 1 });

            //context.SaveChanges();

            var musicList = context.MusicList.Include(list => list.Musics).Single(list => list.Id == 1);
            musicList.Musics.ForEach(Musics.Add);

            



            Player = new MusicPlayer
            {
                PlayList = Musics
            };
            MusicPlayer.ReplacePlayList(musicList.Musics);
           

        }
        private static BitmapImage ByteToBitmapImage(byte[] data)
        {
            if (data == null) return null;
            MemoryStream ms = new(data);
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(ms.AsRandomAccessStream());
            return bitmapImage;
        }
        public MusicPlayer Player { get; set; }
        private readonly List<Music> MusicList = [];

        public ObservableCollection<Music> Musics { get; } = [];
        
        public List<Music> DefaultMusicList { get { return MusicList; } }



    }
}
