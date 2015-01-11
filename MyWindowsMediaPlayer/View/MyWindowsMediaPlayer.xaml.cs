using System;
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
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Xml.Serialization;
using TagLib;

namespace MyWindowsMediaPlayer
{
    /*Globale*/
    #region EType

    public enum EType { Picture, Video, Music };
    #endregion

    /*Fenetre Principale*/
    public partial class MainWindow : Window
    {
        /*Variables*/
        #region _isPlaying _isReplay _index ...

        private Boolean _isPlaying, _isReplay, _isRandom, _isMute;
        private Boolean _userIsDraggingSlider = false;
        private string  _filter = "Video (*.avi, *.mp4, *.wmv)|*.avi;*.mp4;*.wmv |Audio (*.mp3)|*.mp3; |Pictures (*.jpg, *.bmp, *.png)|*.jpg;*.bmp;*.png ";
        private int     _index = 0;
        private ObservableCollection<Media> _listPlayList { set; get; }
        private ObservableCollection<Media> _listBibli { set; get; }
        #endregion

        /*Initialisation*/
        #region MainWindow

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
            this._listPlayList = new ObservableCollection<Media>();
            this._listBibli = new ObservableCollection<Media>();
            this._isPlaying = false;
            this._isRandom = false;
            this._isMute = false;
            this._isReplay = false;
        }
        #endregion

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
                this.PLayList.Items.Clear();
                ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOff.png", UriKind.Relative));
            }
            else
            {
                PLayList.Visibility = Visibility.Visible;
                foreach (Media m in _listPlayList)
                {
                    this.PLayList.Items.Add(m.FileName);
                }
                ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOn.png", UriKind.Relative));
            }
        }
        private void ToggleLibrary(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Library.Visibility == Visibility.Visible)
                {
                    Library.Visibility = Visibility.Hidden;
                    ImgLibrary.Source = new BitmapImage(new Uri(@"../Images/LibraryOff.png", UriKind.Relative));
                }
                else
                {
                    Library.Visibility = Visibility.Visible;
                    //Récupère les fichiers du dossier music
                    string[] filePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "*.mp3", SearchOption.AllDirectories);

                    string Title;
                    string Artist;
                    EType Type;
                    TimeSpan Duree;
                    string Album;
                    string FileName;
                    DateTime Date;
                    long SizeDoc;


                    //string comment;
                    List<Media> items = new List<Media>();


                    // a renomer par dossier
                    _listBibli.Clear();
                    foreach (string s in filePaths)
                    {
                        FileInfo f = new FileInfo(s);
                        TagLib.File tagFile = TagLib.File.Create(s);

                        Artist = "";
                        if (tagFile.Tag.AlbumArtists.Length > 0)
                            Artist = tagFile.Tag.AlbumArtists[0];
                        Album = tagFile.Tag.Album;
                        Title = tagFile.Tag.Title;
                        FileName = System.IO.Path.GetFileNameWithoutExtension(f.Name);
                        Duree = tagFile.Properties.Duration;
                        SizeDoc = f.Length;
                        Date = f.CreationTime;
                        Type = EType.Music;
                        Media med = new Media(s, Album, Title, Duree, Artist, SizeDoc, Date, FileName, Type);
                        _listBibli.Add(med);
                        items.Add(med);

                    }
                    Library.ItemsSource = items;

                    //Récupère les fiches du dossier Photos
                    filePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "*.jpg", SearchOption.AllDirectories);
                    foreach (string s in filePaths)
                    {
                        FileInfo f = new FileInfo(s);
                        TagLib.File tagFile = TagLib.File.Create(s);

                        Artist = "";
                        if (tagFile.Tag.AlbumArtists.Length > 0)
                            Artist = tagFile.Tag.AlbumArtists[0];
                        Album = tagFile.Tag.Album;
                        Title = tagFile.Tag.Title;
                        FileName = System.IO.Path.GetFileNameWithoutExtension(f.Name);
                        Duree = tagFile.Properties.Duration;
                        SizeDoc = f.Length;
                        Date = f.CreationTime;
                        Type = EType.Picture;
                        Media med = new Media(s, Album, Title, Duree, Artist, SizeDoc, Date, FileName, Type);
                        _listBibli.Add(med);
                        items.Add(med);
                        Debug.WriteLine(med.Type);
                    }



                    Library.ItemsSource = items;
                    //Récupère les fichiers du dossier Videos
                    filePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "*.mp4", SearchOption.AllDirectories);
                    foreach (string s in filePaths)
                    {
                        FileInfo f = new FileInfo(s);

                        TagLib.File tagFile = TagLib.File.Create(s);

                        Artist = "";
                        if (tagFile.Tag.AlbumArtists.Length > 0)
                            Artist = tagFile.Tag.AlbumArtists[0];
                        Album = tagFile.Tag.Album;
                        Title = tagFile.Tag.Title;
                        FileName = System.IO.Path.GetFileNameWithoutExtension(f.Name);
                        Duree = tagFile.Properties.Duration;
                        SizeDoc = f.Length;
                        Date = f.CreationTime;
                        Type = EType.Video;
                        Media med = new Media(s, Album, Title, Duree, Artist, SizeDoc, Date, FileName, Type);
                        _listBibli.Add(med);

                        items.Add(med);
                        Debug.WriteLine(med.Type);
                    }

                    Library.ItemsSource = items;


                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Library.ItemsSource);
                    PropertyGroupDescription groupDescription = new PropertyGroupDescription("Type");
                    view.GroupDescriptions.Add(groupDescription);

                    ImgLibrary.Source = new BitmapImage(new Uri(@"../Images/LibraryOn.png", UriKind.Relative));
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp);
            }
        }

       

        #endregion


        /* Playlist Function */
        #region Playlist Function


        #endregion


        /* Media Element Functions */
        #region MediaFailed Drop MediaEnded MediaOpened

        /*In case of fail when we open media*/
        private void MyMediaPlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Not supported File");
            System.Diagnostics.Debug.WriteLine(e);
        }

        /*Drop media in MediaElement*/
        private void MyMediaPlayerDrop(object sender, DragEventArgs e)
        {
            this.WindowState = WindowState.Maximized;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                MyMediaPlayer.Source = new Uri(filePaths[0]);
            }
            if (!this._isPlaying)
            {
                MyMediaPlayer.Play();
                this.changeImageButton(BtnPlay, "../Images/Pause.png");
                BtnPlay.ToolTip = "Suspendre";
                this._isPlaying = true;
            }
        }

        /*Event end media for replay*/
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

        /*Init time for slider when we open media*/
        private void MyMediaPlayerMediaOpened(object sender, RoutedEventArgs e)
        {
            if (MyMediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = MyMediaPlayer.NaturalDuration.TimeSpan;
                SliderProgress.Maximum = ts.TotalSeconds;
                SliderProgress.SmallChange = 1;
                SliderProgress.LargeChange = Math.Min(10, ts.Seconds / 10);
            }
        }

        #endregion

        /* Slider Time Function */
        #region timerTick

        /*Maj time of media*/
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

        private void PreviousAction(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Stop();
            _index += -1;
            if (_index <= 0)
                _index = 0;

            if (_listPlayList.Count > 0)
            {
                Media tmp = _listPlayList.ElementAt<Media>(_index);

                MyMediaPlayer.Source = new Uri(tmp.path);
                MyMediaPlayer.Play();
            }            
        }

        private void NextAction(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Stop();

            if (this._isRandom == true)
            {
                Random random = new Random();
                _index = random.Next(0, _listPlayList.Count);
            }
            else
            {
                _index += 1;
                if (_index >= _listPlayList.Count)
                {
                    _index = _listPlayList.Count - 1;
                }
            }
            if (_listPlayList.Count > 0)
            {
                Media tmp = _listPlayList.ElementAt<Media>(_index);

                MyMediaPlayer.Source = new Uri(tmp.path);
                MyMediaPlayer.Play();
            }
        }


        private void RepeatClick(object sender, RoutedEventArgs e)
        {
            if (!this._isReplay)
            {
                this.changeImageButton(BtnRepeat, "../Images/RepeatOn.png");
                this._isReplay = true;
            }
            else
            {
                this.changeImageButton(BtnRepeat, "../Images/RepeatOff.png");
                this._isReplay = false;
            }
        }

        private void RandomClick(object sender, RoutedEventArgs e)
        {
            if (!this._isRandom)
            {
                this.changeImageButton(BtnRandom, "../Images/RandomOn.png");
                this._isRandom = true;
            }
            else
            {
                this.changeImageButton(BtnRandom, "../Images/RandomOff.png");
                this._isRandom = false;
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
            if (!this._isMute)
            {
                MyMediaPlayer.IsMuted = !MyMediaPlayer.IsMuted;
                this.changeImageButton(BtnSound, "../Images/Mute.png");
                BtnSound.ToolTip = "Muet";
                this._isMute = true;
            }
            else 
            {
                MyMediaPlayer.IsMuted = !MyMediaPlayer.IsMuted;
                this.changeImageButton(BtnSound, "../Images/Volume.png");
                BtnSound.ToolTip = "Volume";
                this._isMute = false;
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
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowState = WindowState.Maximized;
                MyMediaPlayer.Play();
                BtnPlay.ToolTip = "Suspendre";
                this._isPlaying = true;
            }
        }
        
        private void StopClick(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Stop();
            this.changeImageButton(BtnPlay, "../Images/Play.png");
            BtnPlay.ToolTip = "Lire";
            this._isPlaying = false;
            SliderProgress.Value = 0;
            MyMediaPlayer.Close();
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

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        #endregion

        /* Tools */
        #region changeImageButton

        /*Change image of button*/
        private void changeImageButton(Button button, string path)
        {
            button.Content = new Image
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
            };
        }
        #endregion


        /*Add current file in playlist*/        
        private void AddInPlayList(object sender, RoutedEventArgs e)
        {
            if (MyMediaPlayer.Source != null)
            {
                string  currentpath = MyMediaPlayer.Source.LocalPath;

                try
                {
                    TagLib.File tagFile = TagLib.File.Create(currentpath);
                    FileInfo f = new FileInfo(currentpath);
                    string filename = System.IO.Path.GetFileNameWithoutExtension(f.Name);
                    string album = tagFile.Tag.Album;
                    string titre = tagFile.Tag.Title;
                    TimeSpan duration = tagFile.Properties.Duration;
                    string artist = "";

                    if (tagFile.Tag.AlbumArtists.Length > 0)
                        artist = tagFile.Tag.AlbumArtists[0];

                    long size = f.Length;
                    DateTime creat = f.CreationTime;
                    Media med = new Media(currentpath, album, titre, duration, artist, size, creat, filename, EType.Music);

                    _listPlayList.Add(med);
                    this.PLayList.Items.Clear();
                    foreach (Media m in _listPlayList)
                        this.PLayList.Items.Add(m.FileName);
                    this.PLayList.Visibility = Visibility.Visible;
                    ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOn.png", UriKind.Relative));
                }
                catch (Exception exp)
                {
                    System.Windows.Forms.MessageBox.Show("Not supported Media");
                }
            }
        }

        /*Save the current playlist*/
        private void SavePlayList(object sender, RoutedEventArgs e)
        {
            SaveFileDialog  box = new SaveFileDialog();
            box.FileName = "Document";
            box.DefaultExt = ".xml";
            box.Filter = "Playlist file (.xml)|*.xml";
            Nullable<bool> result = box.ShowDialog();

            if (result == true)
            {
                string          filename = box.FileName;
                XmlSerializer   xs = new XmlSerializer(typeof(ObservableCollection<Media>));

                using (StreamWriter wr = new StreamWriter(filename))
                    xs.Serialize(wr, _listPlayList);
            }
        }

        /*Open a playlist already saved */
        private void OpenPlaylist(object sender, RoutedEventArgs e)
        {
            OpenFileDialog  file = new OpenFileDialog();
            string          filterXml = "Document Xml (*.xml)|*.xml;";
            bool?           userClickedOk = file.ShowDialog();
            file.Filter = filterXml;
            file.Multiselect = false;

            if (userClickedOk == true)
            {
                string ext = System.IO.Path.GetExtension(file.FileName);

                if (ext.Equals(".xml") == true)
                {
                    XmlSerializer   xs = new XmlSerializer(typeof(ObservableCollection<Media>));

                    using (StreamReader rd = new StreamReader(file.FileName))
                    {
                        ObservableCollection<Media> p = xs.Deserialize(rd) as ObservableCollection<Media>;

                        this.PLayList.Items.Clear();
                        _listPlayList.Clear();
                        foreach (Media m in p)
                        {
                            _listPlayList.Add(m);
                            this.PLayList.Items.Add(m.FileName);
                        }
                        this.PLayList.Visibility = Visibility.Visible;
                        ImgPlayList.Source = new BitmapImage(new Uri(@"../Images/PlayListOn.png", UriKind.Relative));
                    }
                }
            }

        }

        /*Sort column by asc*/
        private void SortBibli(string column)
        {
            if (column.Equals("Title") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.Title select m);
            else if (column.Equals("Artist") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.Artist select m);
            else if (column.Equals("Duree") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.Duree select m);
            else if (column.Equals("Album") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.Album select m);
            else if (column.Equals("FileName") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.FileName select m);
            else if (column.Equals("Date") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.Date select m);
            else if (column.Equals("SizeDoc") == true)
                _listBibli = new ObservableCollection<Media>(from m in _listBibli orderby m.SizeDoc select m);
        }

        /*Sort bibli*/
        private void ClickToSort(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(GridViewColumnHeader))
            {
                GridViewColumnHeader    headerClicked = e.OriginalSource as GridViewColumnHeader;
                string                  column = headerClicked.Column.Header as string;
                List<Media>             items = new List<Media>();

                this.SortBibli(column);
                foreach(Media m in _listBibli)
                    items.Add(m);
                Library.ItemsSource = items;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Library.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Type");
                view.GroupDescriptions.Add(groupDescription);
            }
        }

        /*Kick element out of the current playlist*/
        private void KickOfList(object sender, MouseButtonEventArgs e)
        {
            if (this.PLayList.SelectedItem != null)
            {
                int i = PLayList.SelectedIndex;

                if (i >= 0)
                {
                    _listPlayList.RemoveAt(i);
                    this.PLayList.Items.Clear();
                    foreach (Media m in _listPlayList)
                        this.PLayList.Items.Add(m.FileName);
                }

            }
        }

        /*Play selected element from playlist*/
        private void PlayOfList(object sender, MouseButtonEventArgs e)
        {
            if (this.PLayList.SelectedItem != null)
            {
                int     i = PLayList.SelectedIndex;
                Media   elem = _listPlayList.ElementAt(i);
                MyMediaPlayer.Source = new Uri(elem.path);

                if (!this._isPlaying)
                {
                    this.ResizeMode = ResizeMode.CanResize;
                    this.WindowState = WindowState.Maximized;
                    MyMediaPlayer.Play();
                    BtnPlay.ToolTip = "Suspendre";
                    this._isPlaying = true;
                }
            }
        }

        /*Play selected element from bibli*/
        private void PlayOfBibli(object sender, MouseButtonEventArgs e)
        {
            if (this.Library.SelectedItem != null)
            {
                int i = Library.SelectedIndex;
                Media elem = _listBibli.ElementAt(i);
//                MyMediaPlayer.Source = new Uri(elem.path);

/*                if (!this._isPlaying)
                {
                    this.ResizeMode = ResizeMode.CanResize;
                    this.WindowState = WindowState.Maximized;
                    MyMediaPlayer.Play();
                    BtnPlay.ToolTip = "Suspendre";
                    this._isPlaying = true;
                }*/
            }
        }
    }

}

