using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MouseMacro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer timer;
        private int timeBetweenTimerField;
        private bool isRunning;
        private DateTimeOffset start;
        private DateTimeOffset stop;
        private Random randomX = new Random(Guid.NewGuid().GetHashCode());
        private Random randomY = new Random(Guid.NewGuid().GetHashCode());
        private readonly int MOVE_TIMER = 3;
        private Random randomColor = new Random();

        public MainWindow()
        {
            InitializeComponent();
            TimeBetweenTimer = MOVE_TIMER;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int TimeBetweenTimer
        {
            get { return timeBetweenTimerField; }
            set 
            {
                timeBetweenTimerField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeBetweenTimer)));
            }
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set 
            { 
                isRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            }
        }

        
        public DateTimeOffset Start
        {
            get { return start; }
            set 
            { 
                start = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Start)));
            }
        }

        public DateTimeOffset Stop
        {
            get { return stop; }
            set 
            { 
                stop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stop)));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRunning)
            {
                Start = DateTimeOffset.Now;
                IsRunning = true;
                timer = new DispatcherTimer(DispatcherPriority.Background);
                timer.Interval = TimeSpan.FromSeconds(1);
                int posX = 0, posY = 0;

                timer.Tick += (x, y) =>
                {
                    TimeBetweenTimer--;
                    if (TimeBetweenTimer < 0)
                    {
                        posX = randomX.Next((int)ActualWidth);
                        posY = randomY.Next((int)ActualHeight);

                        //MessageBox.Show("X: " + posX, " Y: " + posY);

                        var rectangle = new Rectangle() { Width = 30, Height = 30, Fill = new SolidColorBrush(Color.FromArgb((byte)randomColor.Next(256), (byte)randomColor.Next(256), (byte)randomColor.Next(256), (byte)randomColor.Next(256))) };
                        Canvas.SetLeft(rectangle, posX);
                        Canvas.SetTop(rectangle, posY);
                        drawingCanvas.Children.Add(rectangle);

                        TimeBetweenTimer = MOVE_TIMER;
                        Win32.POINT p = new Win32.POINT(posX, posY);
                        Win32.ClientToScreen(new WindowInteropHelper(this).Handle, ref p);
                        Win32.SetCursorPos(p.x, p.y);

                        SendKeys.SendWait("{F12}");
                    }
                };
                timer.IsEnabled = true;
                timer.Start();
            }
            else
            {
                timer.Stop();
                TimeBetweenTimer = MOVE_TIMER;
                Stop = DateTimeOffset.Now;
                IsRunning = false;
            }
        }
    }

    public class Win32
    {
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }
    }
}
