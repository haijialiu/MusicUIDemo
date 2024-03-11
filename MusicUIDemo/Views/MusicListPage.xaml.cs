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
        public MusicListPage()
        {
            InitializeComponent();
            ViewModel = MusicListViewModel.GetIntance();
            music_list.ItemsSource = ViewModel.MusicItems;
        }
        public MusicListViewModel ViewModel { get; set; }




        private void Play_Btn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = music_list.Items.IndexOf(item);
            MusicPlayer.Operate("switch", index.ToString());
            ViewModel.Player.PlayStatus = true;

        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var musicIndex = music_list.SelectedIndex;
            MusicPlayer.Operate("switch", musicIndex.ToString());
            ViewModel.Player.PlayStatus = true;
        }

        private void Remove_Btn_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = music_list.Items.IndexOf(item);
            ViewModel.MusicItems.RemoveAt(index);
            MusicPlayer.Operate("remove", index.ToString());
        }

    }
}
