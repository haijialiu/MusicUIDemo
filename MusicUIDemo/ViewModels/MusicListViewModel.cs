using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed class MusicListViewModel : ObservableRecipient
    {
        //private static readonly MusicListViewModel instance= new();
        //public static MusicListViewModel GetIntance() => instance;

        public MusicListViewModel()
        {
            using var context = new DataContext();
            var musics = context.Musics;

            //include负责把相关的关联数据查询进来
            var musicList = context.MusicList.Include(list => list.Musics);
            //用户列表
            UserLists = new ObservableCollection<MusicList>(musicList.Where(list => list.Type == "user"));

            MusicLists = new ObservableCollection<MusicList>(musicList);

            var playingList = musicList.Single(list => list.Title == "playing");
            PlayingList = new ObservableCollection<Music>(context.Entry(playingList).Collection(list => list.Musics).Query());
            Player = App.Current.Services.GetRequiredService<MusicPlayer>();
            Player.ReplacePlayList(playingList.Musics);
        }

        public void ReplacePlayingList(int listId)
        {
            using var context = new DataContext();
            PlayingList.Clear();
            context.MusicList.Include(list => list.Musics).Single(list => list.Id == listId).Musics.ForEach(PlayingList.Add);
        }

        private MusicPlayer Player { get; }

        //全部列表
        public ObservableCollection<MusicList> MusicLists { get; } = [];

        //用户自定义的列表
        public ObservableCollection<MusicList> UserLists { get; set; } = [];

        //当前播放列表
        public ObservableCollection<Music> PlayingList { get; private set; } = [];

        //初始化数据库
        private static void Init()
        {
            using var context = new DataContext();

            var musics = context.Musics;
            var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            var files_path = Directory.GetFiles(folderPath);
            int index = 1;

            foreach (var file_path in files_path)
            {
                var item = MediaInfo.GetMusic(file_path);
                item.Id = index;
                index++;
                musics.Add(item);
            }
            var musiclist = context.MusicList;
            musiclist.Add(new Models.MusicList() { Id = 1, Title = "local", Name = "本地歌曲", CreatedTime = DateTime.Now });
            var musicMusiclist = context.MusicMusicLists;
            musicMusiclist.Add(new MusicMusicList() { MusicId = 1, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 2, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 3, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 4, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 5, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 6, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 7, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 8, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 9, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 10, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 11, MusicListId = 1 });
            musicMusiclist.Add(new MusicMusicList() { MusicId = 12, MusicListId = 1 });
            context.SaveChanges();
        }
    }
}