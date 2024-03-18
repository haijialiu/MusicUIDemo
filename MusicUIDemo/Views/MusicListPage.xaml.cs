using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MusicUIDemo.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicListPage : Page
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private MusicListViewModel ViewModel => (MusicListViewModel)DataContext;
        private MusicPlayer player;
        public MusicListPage()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            InitializeComponent();
            //music_list.ItemsSource = ViewModel.MusicItems;
            //music_list.ItemsSource = ViewModel.Musics;
        }
        public MusicListPage(int musicListIndex)
        {
            InitializeComponent();
            //music_list.ItemsSource = ViewModel.Musics;
            music_list.ItemsSource = ViewModel.MusicLists[musicListIndex].Musics;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is int id)
            { 
                music_list.ItemsSource = ViewModel.MusicLists.Where(list => list.Id == id).FirstOrDefault().Musics;
                //music_list.ItemsSource = ViewModel.MusicLists[id-1].Musics;
            }
        }


        private void Play_Btn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = music_list.Items.IndexOf(item);
            MusicPlayer.Operate("switch", index.ToString());
            player.PlayStatus = true;

        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var musicIndex = music_list.SelectedIndex;
            MusicPlayer.Operate("switch", musicIndex.ToString());
            player.PlayStatus = true;
        }

        private void Remove_Btn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = music_list.Items.IndexOf(item);
            throw new NotImplementedException();
            //ViewModel.Musics.RemoveAt(index);
            //MusicPlayer.Operate("remove", index.ToString());
        }

        public static string TimeFormat(int seconds)
        {
            return string.Format("{0}:{1}", seconds/60,seconds%60);
        }




    }
}
