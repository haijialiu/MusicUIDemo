using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MusicUIDemo.Views
{
    public sealed partial class PlayerController : UserControl
    {

        public MusicListViewModel viewModel = MusicListViewModel.GetIntance();
        public MusicPlayer player = null;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly TimeSpan span = TimeSpan.FromMilliseconds(100);
        private readonly ThreadPoolTimer threadPoolTimer;
        public PlayerController()
        {
            InitializeComponent();
            Unloaded += PlayControllerCloseEvent;
            player = viewModel.Player;

            threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                var current_index = FFmpegPlayer.CurrentPlayIndex();
                var current_play_time = FFmpegPlayer.PlayedTime();
                if (dispatcherQueue.HasThreadAccess)
                {

                }
                else
                {
                    bool isQueued = dispatcherQueue.TryEnqueue(() =>
                    {
                        //修改值会触发onChanged 刷新UI
                        player.CurrentPlayIndex = current_index;
                        player.CurrentTime= FFmpegPlayer.PlayedTime();
 
                    });
                }
            }, span);


        }

        private void PlayControllerCloseEvent(object sender, RoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
        }
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {

            if (player.PlayStatus)
            {
                MusicPlayer.Operate("pause", "1");
            }
            else
            {
                MusicPlayer.Operate("resume", "1");
            }
            player.PlayStatus = !player.PlayStatus;
        }
        

        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            player.CurrentPlayIndex -= 1;
            MusicPlayer.Operate("switch", player.CurrentPlayIndex.ToString());
        }
        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            player.CurrentPlayIndex += 1;
            MusicPlayer.Operate("switch", player.CurrentPlayIndex.ToString());
        }

    }
}
