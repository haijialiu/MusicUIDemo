using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models
{
    //播放器
    public class MusicPlayer : BindableBase
    {
        //播放列表
        public ObservableCollection<MusicItem> PlayList { get; set; } = null;
        //当前播放歌曲的序号
        private int currentIndex = 0;

        private MusicListViewModel viewModel = MusicListViewModel.GetIntance();

        public int CurrentPlayIndex
        {
            get => currentIndex;
            set
            {
                if (value < 0)
                {
                    if (PlayList.Count == 0)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = PlayList.Count - 1;
                    }
                }
                else if (value >= PlayList.Count)
                {
                    value = 0;
                }
                SetProperty(ref currentIndex, value);
                //OnPropertyChanged(nameof(CurrentMusic));
                CurrentMusic = PlayList[value];
            }
        }
        private bool status = false;
        public bool PlayStatus 
        {
            get => status;
            set => SetProperty(ref status, value); 
        }
        //当前播放的音乐，其实就是PlayList[CurrentPlayIndex]
        private MusicItem currentMusic;
        public MusicItem CurrentMusic 
        {
            get => currentMusic;
            set => SetProperty(ref currentMusic, value);
        }
        private TimeOnly playedTime;

        public TimeOnly PlayedTime
        {
            get => playedTime;
            set => SetProperty(ref playedTime, value);
        }
        private double playedSeconds;
        public double PlayedSeconds
        {
            get => playedSeconds;
            set
            {

                if (value > 0)
                {
                    playedSeconds = value;
                    OnPropertyChanged(nameof(PlayedSeconds));
                    int minute = (int)(value / 60);
                    int hour = (int)(value / 3600);
                    int second = (int)(value % 60);
                    PlayedTime = new TimeOnly(hour, minute, second);
                }

            }
        }

        //TODO: 音量调节(暂未实现）
        public int Volume { get; set; }
        //播放模式 0 单曲 1 顺序播放 2 列表循环
        private int mode = 1;
        public int PlayMode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        //当前播放时间
        private TimeOnly currentTime=new(0,0,0);
        public TimeOnly CurrentPlayTime
        {
            get => currentTime;
            set => SetProperty(ref currentTime, value);
        }
        //这首歌一共多久
        private TimeOnly totalTime;
        public TimeOnly TotalTime
        {
            get => totalTime;
            set =>  SetProperty(ref totalTime, value);
        }
        private static readonly FFmpegPlayer FFmpegPlayer;
        static MusicPlayer()
        {
            FFmpegPlayer.InitPlayer();
        }
        public static void ReplacePlayList(List<MusicItem> list)
        {
            FFmpegPlayer.InputMusic(list);
        }
        public static void Operate(string action, string value)
        {
           FFmpegPlayer.OperatePlayer(action, value);
        }
        public MusicPlayer ()
        {
           
        }

    }
}
