using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 取色器
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private double zoom;
        private getpoint.POINT m ;
        getpoint.POINT g;
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            GetColor(100,100);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.04);
            timer.Tick += timer1_Tick;
            this.Topmost = true;

            double relh = new getrelsize().Getw();
            zoom = relh / SystemParameters.WorkArea.Width;
            timer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getpoint.GetCursorPos(out g);
            if (g.X != m.X || g.Y != m.Y)
            {
                m.X = g.X;
                m.X = g.Y;
                this.bt1.Content = g.X + ":" + g.Y;
                Color c = GetColor(g.X, g.Y);
                this.colorbox.Background = new SolidColorBrush(c);
            }
        }

        /// <summary>
        /// 获取指定窗口的设备场景
        /// </summary>
        /// <param name="hwnd">将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的DC</param>
        /// <returns>指定窗口的设备场景句柄，出错则为0</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// 释放由调用GetDC函数获取的指定设备场景
        /// </summary>
        /// <param name="hwnd">要释放的设备场景相关的窗口句柄</param>
        /// <param name="hdc">要释放的设备场景句柄</param>
        /// <returns>执行成功为1，否则为0</returns>
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>
        /// 在指定的设备场景中取得一个像素的RGB值
        /// </summary>
        /// <param name="hdc">一个设备场景的句柄</param>
        /// <param name="nXPos">逻辑坐标中要检查的横坐标</param>
        /// <param name="nYPos">逻辑坐标中要检查的纵坐标</param>
        /// <returns>指定点的颜色</returns>
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        //使用：
        public Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero); uint colorRef = GetPixel(hdc, x, y);
            int r = (int)(colorRef & 0x000000FF);
            int g = (int)(colorRef & 0x0000FF00) >> 8;
            int b=(int)(colorRef & 0x00FF0000) >> 16 ;
            if (mving)
            {
                r = getbgcolor0(r);
                g = getbgcolor0(g);
                b = getbgcolor0(b);
            }
            Point nm = Mouse.GetPosition(this);
            if (!mving&& nm.X > form.Margin.Left && nm.X < form.Margin.Left + form.Width && nm.Y > form.Margin.Top && nm.Y < form.Margin.Top + form.Height)
            {
                r = getbgcolor(r);
                g = getbgcolor(g);
                b = getbgcolor(b);
            }
            ReleaseDC(IntPtr.Zero, hdc);
            this.rgbbox.Text= r + "," + g + "," + b;
            this.strbox.Text ="#"+Convert.ToString(r, 16)+ Convert.ToString(g, 16)+ Convert.ToString(b, 16);
            return Color.FromArgb(255,(byte)r, (byte)g, (byte)b);
        }

       private int getbgcolor(int rl)
        {
            double drl = (double)rl;
            double r = (drl - 128/100) / 0.99;
            int re = (int)Math.Round(r, 0);
            if (re > 255)
            {
                re = 255;
            }
            return re;
        }
        private int getbgcolor0(int rl)
        {
            double drl = (double)rl;
            double r = (drl - 128 / 100 - 128 / 100) / 0.98;
            int re= (int)Math.Round(r, 0);
            if (re > 255)
            {
                re = 255;
            }
            return re;
        }
        private Point down;
        private Thickness Mov;
        private bool mving=false;
        private void form_mdown(object sender, MouseButtonEventArgs e)
        {
            down = Mouse.GetPosition(this);
           // timer.Stop();
           this.Background= new SolidColorBrush(Color.FromArgb(1, 128, 128, 128));
            mving = true;
        }

        private void form_mv(object sender, MouseEventArgs e)
        {
            if (mving)
            {
                Point nm = Mouse.GetPosition(this);
                Mov = form.Margin;
                Mov.Left += nm.X - down.X;
                Mov.Top += nm.Y - down.Y;
                down = nm;
                form.Margin = Mov;
            }
        }

        private void form_up(object sender, MouseButtonEventArgs e)
        {
            mving = false;
            this.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            // timer.Start();
        }


  
        private void min_en(object sender, MouseEventArgs e)
        {
            min_border.Visibility = Visibility.Visible;
        }

        private void min_leave(object sender, MouseEventArgs e)
        {
            min_border.Visibility = Visibility.Hidden;
        }

        private void x_en(object sender, MouseEventArgs e)
        {
            x_border.Visibility = Visibility.Visible;
        }

        private void x_leave(object sender, MouseEventArgs e)
        {
            x_border.Visibility = Visibility.Hidden;
        }

        private void min_down(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void x_down(object sender, MouseButtonEventArgs e)
        {
            if (aboutiswhow)
            {
                about.Close();
            }
            this.Close();
        }

        private void strbox_leave(object sender, StylusEventArgs e)
        {
            nonebox.Focus();
        }

        private void m_leave(object sender, MouseEventArgs e)
        {
            nonebox.Focus();
        }

        private void load(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show("L:l");.
            IntPtr Handle = new WindowInteropHelper(this).Handle;
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。  
            HotKey.RegisterHotKey(Handle, 103, HotKey.KeyModifiers.Shift | HotKey.KeyModifiers.Ctrl, System.Windows.Forms.Keys.C);
            //注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。  
            HotKey.RegisterHotKey(Handle, 104, HotKey.KeyModifiers.Shift | HotKey.KeyModifiers.Ctrl, System.Windows.Forms.Keys.X);
            HotKey.RegisterHotKey(Handle, 105, HotKey.KeyModifiers.Shift | HotKey.KeyModifiers.Ctrl, System.Windows.Forms.Keys.Z);
            HotKey.RegisterHotKey(Handle, 107, HotKey.KeyModifiers.Shift | HotKey.KeyModifiers.Ctrl, System.Windows.Forms.Keys.S);
            RegisterHotKey();
            //AccessAppSettings();
        }
        private void RegisterHotKey()
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            //获得消息源
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);

            source.AddHook(HotKeyHook);
        }

        private IntPtr HotKeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //热键处理过程
        {
            const int WM_HOTKEY = 0x0312;//如果m.Msg的值为0x0312那么表示用户按下了热键
            if (msg == WM_HOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    case 103:
                        //此处填写快捷键响应代码
                            if (strbox.Text != ""){
                                    Clipboard.SetDataObject(strbox.Text);
                                }
                         break;
                    case 104:
                        //此处填写快捷键响应代码
                        if (rgbbox.Text != "")
                        {
                            Clipboard.SetDataObject(rgbbox.Text);
                        }
                        break;
                    case 105:
                        //此处填写快捷键响应代码
                        if(this.WindowState== WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Minimized;
                        }
                        else
                        {
                            this.WindowState = WindowState.Maximized;
                        }
                        break;
          
                    case 107:
                        //此处填写快捷键响应代码
                        if (timer.IsEnabled)
                        {
                            timer.Stop();
                        }
                        else
                        {
                            timer.Start();
                        }
                        break;

                }
            }
            return IntPtr.Zero;
        }
        public static bool aboutiswhow = false;
        Window about;
        private void ico_down(object sender, MouseButtonEventArgs e)
        {
            if (!aboutiswhow)
            {
                about = new about();
                aboutiswhow = true;
                about.Show();
            }
        }
    }
}
