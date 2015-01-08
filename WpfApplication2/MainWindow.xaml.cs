using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean isPlay;
        Boolean isReplay;

        public MainWindow()
        {
            InitializeComponent();
            MyMediaPlayer.MediaEnded += new RoutedEventHandler(MyMediaPlayer_MediaEnded);
            MyMediaPlayer.Drop += new DragEventHandler(MyMediaPlayer_Drop);
            MyMediaPlayer.MediaFailed += MyMediaPlayer_MediaFailed;

            MyMediaPlayer.LoadedBehavior = MediaState.Manual;
            MyMediaPlayer.UnloadedBehavior = MediaState.Manual;

            isPlay = false;
            this.isReplay = false;
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (isPlay == false)
            {

                MyMediaPlayer.Play();
                isPlay = true;
            }
            else
            {
                isPlay = false;
                MyMediaPlayer.Pause();
            }

        }

        private void mute_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.IsMuted = !MyMediaPlayer.IsMuted;
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog toto = new OpenFileDialog();

            toto.Multiselect = false;
            bool? userClickedOk = toto.ShowDialog();

            if (userClickedOk == true)
            {
                string path = toto.FileName;
                MyMediaPlayer.Source = new Uri(path);
            }
            if (isPlay == false)
            {

                MyMediaPlayer.Play();
                isPlay = true;
            }
        }

        private void fullScreen_Click(object sender, RoutedEventArgs e)
        {
          
      
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Stop();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyMediaPlayer.Volume = (double)volumeSlider.Value;
        }

        private void replay_Click(object sender, RoutedEventArgs e)
        {
            if (this.isReplay == false)
                this.isReplay = true;
            else
                this.isReplay = false;

        }

        private void MyMediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine(e);
        }

        private void MyMediaPlayer_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string [] filePaths = (string [])(e.Data.GetData(DataFormats.FileDrop));
                MyMediaPlayer.Source = new Uri(filePaths[0]);
            }
            if (isPlay == false)
            {
                MyMediaPlayer.Play();
                isPlay = true;
            }
        }
        private void MyMediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.isReplay == true)
                {
                    MyMediaPlayer.Position = TimeSpan.Zero;
                    MyMediaPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}
