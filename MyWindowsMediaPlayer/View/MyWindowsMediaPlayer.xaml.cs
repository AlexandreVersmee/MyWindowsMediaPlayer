﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace MyWindowsMediaPlayer
{

    public partial class MainWindow : Window
    {
        private Boolean _isPlaying;
        private Boolean _isReplay;
        private Boolean _isRandom;
        private Boolean _userIsDraggingSlider = false;
        private string _filter = "Video (*.avi, *.mp4, *.wmv)|*.avi;*.mp4;*.wmv |Audio (*.mp3)|*.mp3 |Pictures (*.jpg, *.bmp, *.png)|*.jpg;*.bmp;*.png ";

        public MainWindow()
        {
            InitializeComponent();
            MyMediaPlayer.MediaEnded += new RoutedEventHandler(MyMediaPlayerMediaEnded);
            MyMediaPlayer.Drop += new DragEventHandler(MyMediaPlayerDrop);
            MyMediaPlayer.MediaFailed += MyMediaPlayerMediaFailed;
            MyMediaPlayer.MediaOpened += MyMediaPlayerMediaOpened;
         
            MyMediaPlayer.LoadedBehavior = MediaState.Manual;
            MyMediaPlayer.UnloadedBehavior = MediaState.Manual;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timerTick;
            timer.Start();

            this._isPlaying = false;
            this._isRandom = false;
            this._isReplay = false;
        }

        /* Menu */
        #region Menu
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void TogglePlayList(object sender, RoutedEventArgs e)
        {
            if (PLayList.Visibility == Visibility.Visible)
            {
                PLayList.Visibility = Visibility.Hidden;
                ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOff.png", UriKind.Relative));
            }
            else
            {
                PLayList.Visibility = Visibility.Visible;
                ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOn.png", UriKind.Relative));
            }
        }

        #endregion


        /* Playlist Function */
        #region Playlist Function




        #endregion


        /* Media Element Functions */
        #region Media Element Functions
        private void MyMediaPlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e);
        }
        private void MyMediaPlayerDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                MyMediaPlayer.Source = new Uri(filePaths[0]);
            }
            if (!this._isPlaying)
            {
                MyMediaPlayer.Play();
                BtnPlay.ToolTip = "Suspendre";
            }
        }
        private void MyMediaPlayerMediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._isReplay == true)
                {
                    MyMediaPlayer.Position = TimeSpan.Zero;
                    MyMediaPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        private void MyMediaPlayerMediaOpened(object sender, RoutedEventArgs e)
        {
            if (MyMediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = MyMediaPlayer.NaturalDuration.TimeSpan;
                SliderProgress.Maximum = ts.TotalSeconds;
                SliderProgress.SmallChange = 1;
                SliderProgress.LargeChange = Math.Min(10, ts.Seconds / 10);
            }
        //            MyMediaPlayer.Visibility = Visibility.Visible;
        }

        #endregion

        /* Slider Time Function */

        #region Slider Time Function
        private void timerTick(object sender, EventArgs e)
        {
            if ((MyMediaPlayer.Source != null) && (MyMediaPlayer.NaturalDuration.HasTimeSpan) && (!_userIsDraggingSlider))
            {
                SliderProgress.Minimum = 0;
                SliderProgress.Maximum = MyMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SliderProgress.Value = MyMediaPlayer.Position.TotalSeconds;
            }
        }
        #endregion

        /* Buttons Functions  */
        #region Buttons Functions

        private void RepeatClick(object sender, RoutedEventArgs e)
        {
            if (!BtnRepeat.IsDefault)
            {
                this.changeImageButton(BtnRepeat, "../Images/RepeatOn.png");
                BtnRepeat.IsDefault = true;
                this._isReplay = true;
            }
            else
            {
                this.changeImageButton(BtnRepeat, "../Images/RepeatOff.png");
                BtnRepeat.IsDefault = false;
                this._isReplay = false;
            }
        }

        private void RandomClick(object sender, RoutedEventArgs e)
        {
            if (!BtnRandom.IsDefault)
            {
                this.changeImageButton(BtnRandom, "../Images/RandomOff.png");
                BtnRandom.IsDefault = true;
                this._isRandom = true;
            }
            else
            {
                this.changeImageButton(BtnRandom, "../Images/RandomOn.png");
                BtnRandom.IsDefault = false;
                this._isRandom = true;
            }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            if (!this._isPlaying)
            {
                MyMediaPlayer.Play();
                this.changeImageButton(BtnPlay, "../Images/Pause.png");
                BtnPlay.ToolTip = "Suspendre";
                this._isPlaying = true;
            }
            else
            {
                this.changeImageButton(BtnPlay, "../Images/Play.png");
                BtnPlay.ToolTip = "Lire";
                this._isPlaying = false;
                MyMediaPlayer.Pause();
            }
        }
        
        private void MuteClick(object sender, RoutedEventArgs e)
        {
            if (!this._isPlaying)
            {
                MyMediaPlayer.IsMuted = !MyMediaPlayer.IsMuted;
                this.changeImageButton(BtnSound, "../Images/Mute.png");
                BtnSound.ToolTip = "Muet";
                this._isPlaying = true;
            }
            else 
            {
                MyMediaPlayer.IsMuted = !MyMediaPlayer.IsMuted;
                this.changeImageButton(BtnSound, "../Images/Volume.png");
                BtnSound.ToolTip = "Volume";
                this._isPlaying = false;
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog toto = new OpenFileDialog();

            toto.Filter = _filter;
            toto.FilterIndex = 1;
                

            toto.Multiselect = false;
            bool? userClickedOk = toto.ShowDialog();

            if (userClickedOk == true)
            {
                string path = toto.FileName;
                MyMediaPlayer.Source = new Uri(path);
            }
            if (!this._isPlaying)
            {
                MyMediaPlayer.Play();
                BtnPlay.ToolTip = "Suspendre";
                this._isPlaying = true;
            }
        }
        
        private void StopClick(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Stop();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyMediaPlayer.Volume = (double)SliderVolume.Value;
        }

        private void FullScreenClick(object sender, RoutedEventArgs e)
        { }

        private void SliderProgressDragStarted(object sender, DragStartedEventArgs e)
        {
            _userIsDraggingSlider = true;
        }

        private void SliderProgressDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _userIsDraggingSlider = false;
            MyMediaPlayer.Position = TimeSpan.FromSeconds(SliderProgress.Value);
        }

        private void SliderProgressValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(SliderProgress.Value).ToString(@"hh\:mm\:ss");
        }
        #endregion

        /* Tools */
        #region Tools
        private void changeImageButton(Button button, string path)
        {
            button.Content = new Image
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
            };
        }
        #endregion

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
