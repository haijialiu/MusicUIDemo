using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MusicUIDemo.Models;
using MusicUIDemo.Models.Database;
using MusicUIDemo.ViewModels;
using MusicUIDemo.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using WinRT;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MusicUIDemo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>



    public sealed partial class MainWindow : Window
    {
        private static WinProc newWndProc = null;
        private static IntPtr oldWndProc = IntPtr.Zero;
        private delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
        public static int MinWindowWidth { get; set; } = 1300;
        //public static int MaxWindowWidth { get; set; } = 1800;
        public static int MinWindowHeight { get; set; } = 850;
        //public static int MaxWindowHeight { get; set; } = 1600;
        private object Context;
        public MusicListViewModel ViewModel => (MusicListViewModel)Context;
        public ObservableCollection<MusicList> MusicLists;
        private MusicPlayer player;
        public MainWindow()
        {

            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            Context = App.Current.Services.GetService<MusicListViewModel>();
            InitializeComponent();
            Activated += MainWindow_Activated;
            RegisterWindowMinMax(this);
            ExtendsContentIntoTitleBar = true;


            GeneratorUserMusicList();
            contentFrame.Navigate(typeof(MainPage));


        }
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(@"Assets\image\music.ico");
        }
        private static void RegisterWindowMinMax(Window window)
        {
            var hwnd = GetWindowHandleForCurrentWindow(window);

            newWndProc = new WinProc(WndProc);
            oldWndProc = SetWindowLongPtr(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        private static IntPtr GetWindowHandleForCurrentWindow(object target) =>
            WinRT.Interop.WindowNative.GetWindowHandle(target);

        private static IntPtr WndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                case WindowMessage.WM_GETMINMAXINFO:
                    var dpi = GetDpiForWindow(hWnd);
                    var scalingFactor = (float)dpi / 96;

                    var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                    minMaxInfo.ptMinTrackSize.x = (int)(MinWindowWidth * scalingFactor);
                    //minMaxInfo.ptMaxTrackSize.x = (int)(MaxWindowWidth * scalingFactor);
                    minMaxInfo.ptMinTrackSize.y = (int)(MinWindowHeight * scalingFactor);
                    //minMaxInfo.ptMaxTrackSize.y = (int)(MaxWindowHeight * scalingFactor);

                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;

            }
            return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
        }

        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [Flags]
        private enum WindowLongIndexFlags : int
        {
            GWL_WNDPROC = -4,
        }

        private enum WindowMessage : int
        {
            WM_GETMINMAXINFO = 0x0024,
        }

        /// <summary>
        /// 手动生成用户歌单
        /// </summary>
        private void GeneratorUserMusicList()
        {
            var userMusicList = App.Current.Services.GetService<MusicListViewModel>().UserLists.ToList();
            userMusicList.ForEach(musicList =>
            {

                var musicListItemCommandsFlyout = new CommandBarFlyout()
                {
                    AlwaysExpanded = true,
                };
                var playBtn = new AppBarButton()
                {
                    Label = "播放",
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE768",
                    },
                    DataContext = musicList.Id,
                };
                playBtn.Click += PlayAppBarButton_Click;
                musicListItemCommandsFlyout.SecondaryCommands.Add(playBtn);
                var deleteBtn = new AppBarButton()
                {
                    Label = "删除",
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE74D",
                    },
                    DataContext = musicList.Id,
                };
                deleteBtn.Click += DeleteAppBarButton_Click;
                musicListItemCommandsFlyout.SecondaryCommands.Add(deleteBtn);
                var renameBtn = new AppBarButton()
                {
                    Label = "重命名",
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE8AC",
                    },
                    DataContext = musicList.Id,
                };
                renameBtn.Click += RenameAppBarButton_Click;
                musicListItemCommandsFlyout.SecondaryCommands.Add(renameBtn);

                MainNavition.MenuItems.Add(new NavigationViewItem()
                {
                    Content = musicList.Name,
                    Tag = "MusicList",
                    DataContext = musicList.Id,
                    ContextFlyout = MusicListItemCommandsFlyout,
                });
            });

        }

        private void MainNavition_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            NaviHeader.Text = args.InvokedItem?.ToString();
            if (args.InvokedItemContainer?.Tag?.ToString() == "MusicList")
            {
                //using var context = new DataContext();
                var id = int.Parse(args.InvokedItemContainer.DataContext.ToString());
                //var musicList = context.MusicList.Include(list => list.Musics).Single(list => list.Id == id).Musics;
                //ViewModel.SetMusics(musicList);
                contentFrame.Navigate(typeof(MusicListPage), id);
            }

        }

        private void PlayAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is AppBarButton appBarButton)
            {
                var listId = (int)appBarButton.DataContext;
                var list = ViewModel.UserLists.Where(list => list.Id == listId).FirstOrDefault().Musics;
                player.ReplacePlayList(list);
                ViewModel.ReplacePlayingList(listId);
                player.Resume();
            }
 
        }
        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RenameAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
        }


    }
    public class MusicListNavigation
    {
        public string Header;
        public List<MusicList> musicLists;
    }

}
