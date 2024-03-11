using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.Models.Database;
using MusicUIDemo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;


namespace MusicUIDemo.ViewModels
{

    public class MusicListViewModel
    {

        private static readonly MusicListViewModel instance= new();
        public static MusicListViewModel GetIntance() => instance;
        
        private MusicListViewModel() 
        {

            using (var context = new DataContext())
            {
                var musics = context.Musics.ToList();

                foreach (var music in musics)
                {
                    MusicItems.Add(new MusicItem()
                    {
                        Id = music.Id,
                        Title = music.Title,
                        Album = music.Album,
                        FilePath = music.FilePath,
                        AlbumImage = ByteToBitmapImage(music.AlbumImage),
                        Time = new(music.Time),
                    });
                }



            }
            
            //var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            //var files_path = Directory.GetFiles(folderPath);


            //foreach (var file_path in files_path)
            //{
            //    var item = MediaInfo.GetMusicItem(file_path);
            //    var music = MediaInfo.GetMusic(file_path);
            //    MusicItems.Add(item);
            //    musicList.Add(item);
            //}


            Player = new MusicPlayer
            {
                PlayList = MusicItems
            };
            MusicPlayer.ReplacePlayList(DefaultMusicList);
           

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
        private readonly List<MusicItem> musicList = [];
        public ObservableCollection<MusicItem> MusicItems { get; } = [];
        
        public List<MusicItem> DefaultMusicList { get { return musicList; } }



    }
}
