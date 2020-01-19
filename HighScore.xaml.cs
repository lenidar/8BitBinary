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
using System.Windows.Shapes;
using System.IO;

namespace _8BitBinary
{
    /// <summary>
    /// Interaction logic for HighScore.xaml
    /// </summary>
    public partial class HighScore : Window
    {
        public string[] HSName = new string[5];
        public int[] Score = new int[5];
        public TextBox[] HSNameDisp;
        public Label[] HSScoreDisp;
        public bool HSChange = false;

        public HighScore()
        {
            InitializeComponent();

            //initialize object array
            HSNameDisp = new TextBox[] { txtHS1, txtHS2, txtHS3, txtHS4, txtHS5 };
            HSScoreDisp = new Label[] { lblHS1, lblHS2, lblHS3, lblHS4, lblHS5 };
        }

        private void BtnCloseHSWindow_Click(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < HSName.Length; x++)
            {
                HSName[x] = HSNameDisp[x].Text;
                HSNameDisp[x].IsEnabled = false;
            }
            this.Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                for(int x = 0; x < HSName.Length; x++)
                {
                    HSNameDisp[x].Text = HSName[x];
                    HSScoreDisp[x].Content = Score[x];
                }
            }
            else
            {
                if (HSChange)
                {
                    if (File.Exists("HighScore.txt"))
                    {
                        File.Delete("HighScore.txt");
                    }

                    using (StreamWriter sw = new StreamWriter("HighScore.txt"))
                    {
                        for (int x = 0; x < HSName.Length; x++)
                        {
                            sw.WriteLine(HSName[x] + "," + Score[x]);
                        }
                    }
                    HSChange = false;
                }
            }
        }
    }
}
