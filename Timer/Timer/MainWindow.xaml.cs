using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string MAX_TIME = "23:59:59";
        private readonly string DEFAULT_TIME = "00:00:00";

        private DispatcherTimer gameTimer;
        SoundPlayer player = new SoundPlayer("alarm.wav");
        private bool stopwatch;

        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// initialize timer anew (so that the tick doesn't stack)
        /// interval is every second
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Start();
            gameTimer.Tick += new EventHandler(gameTimer_Tick);
            gameTimer.Interval = new TimeSpan(0, 0, 1);
            stopwatch = (bool)StopWatchRadioButton.IsChecked;
        }

        /// <summary>
        /// stop the timer
        /// also stop alarm sound if playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click_1(object sender, RoutedEventArgs e)
        {            
            try
            {
                gameTimer.Stop();
                player.Stop();
            }
            catch (Exception exc)
            { }
            
        }

        /// <summary>
        /// timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            string timeText = (string)TimeLabel.Text;

            // if the user entered an invalid time (25:61:61) -> calculate wanted time
            timeText = makeValidTime(timeText);

            DateTime time = Convert.ToDateTime(timeText);

            System.TimeSpan second = new System.TimeSpan(0, 0, 0, 1);

            // count time upwarts
            if (stopwatch)
                time = time.Add(second);
            else // count time down
            {
                if (time.ToString("HH:mm:ss") == DEFAULT_TIME)
                    timerFinished();
                else
                    time = time.Subtract(second);
            }

            TimeLabel.Text = time.ToString("HH:mm:ss");
        }

        /// <summary>
        /// reset time to 00:00:00
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TimeLabel.Text = DEFAULT_TIME;
        }

        /// <summary>
        /// convert time string to valid time
        /// 10:20:63 => 10:21:03
        /// </summary>
        /// <param name="timeText">given time string</param>
        /// <returns>new valid time string</returns>
        private string makeValidTime(string timeText)
        {
            char[] splitChar = { ':' };
            string[] time = timeText.Split(splitChar);

            int seconds = Convert.ToInt32(time[2]);
            int minutes = Convert.ToInt32(time[1]);
            int hours = Convert.ToInt32(time[0]);


            while (seconds >= 60)
            {
                minutes++;
                seconds -= 60;
            }

            while (minutes >= 60)
            {
                hours++;
                minutes -= 60;
            }

            if (hours >= 24)
            {
                return MAX_TIME;
            }

            return "" + hours + ':' + minutes + ':' + seconds;
        }

        /// <summary>
        /// stop gametimer
        /// todo: implement alert sound
        /// </summary>
        public void timerFinished()
        {
            gameTimer.Stop();
            player.Play();
        }
    }
}
