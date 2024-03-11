using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MusicUIDemo.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public MainWindow()
        {
            InitializeComponent();
            Activated += MainWindow_Activated;
            RegisterWindowMinMax(this);
            ExtendsContentIntoTitleBar = true;

            contentFrame.Navigate(typeof(MusicListPage));
        }
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(@"Assets\image\music.ico");
        }
        private void MainNavition_SelectionChanged(NavigationView sender,
                                      NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if ((string)selectedItem.Tag == "MainPage")
            {
                NaviHeader.Text = "主页";
                contentFrame.Navigate(typeof(MainPage));
            }
            else if ((string)selectedItem.Tag == "Favo")
            {
                contentFrame.Navigate(typeof(MusicListPage));
                NaviHeader.Text = "我喜欢";
            }

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
    }

    
}
