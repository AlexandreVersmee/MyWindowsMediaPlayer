using Microsoft.Win32;
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


namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.LoadedBehavior = MediaState.Manual;
            MyMediaPlayer.Play();
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
        }

        private void fullScreen_Click(object sender, RoutedEventArgs e)
        {
        //  MyMediaPlayer.
        }
    }
}
