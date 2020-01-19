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
using System.Windows.Threading;
using System.Media;
using System.IO;

namespace _8BitBinary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //initialize public arrays
        public Label[] arrRes;
        public Label[] arrVal;
        public CheckBox[] arrChkSwtch;
        public Object[] blinkable;

        //other variables
        public int tally = 0;
        public int num = 0;
        public Random rnd = new Random();
        public int score = 0;
        public int roundTime = 30;
        public int time = 0;
        public DispatcherTimer timer; //for game timer
        public DispatcherTimer blinker; //for animations? im not even sure if i should use this
        public bool timerTick = true;
        public int toBlink = 0;
        public bool blinkTick = true;
        public int blinkTime = 2;
        public int blinkAdd = 0; //address of thing to blink
        public SoundPlayer sound1;
        public SoundPlayer sound2;
        public bool gameMode = true;
        public bool testMode = false;

        //High Score window display
        public HighScore hsDisp = new HighScore();

        public MainWindow()
        {
            InitializeComponent();
            //put contents of initial array into arrays :)
            arrRes = new Label[] { lbl1Val, lbl2Val, lbl4Val, lbl8Val, lbl16Val, lbl32Val, lbl64Val, lbl128Val };
            arrVal = new Label[] { lbl1, lbl2, lbl4, lbl8, lbl16, lbl32, lbl64, lbl128 };
            arrChkSwtch = new CheckBox[] { chkbxBinary1, chkbxBinary2, chkbxBinary4, chkbxBinary8, chkbxBinary16, chkbxBinary32, chkbxBinary64, chkbxBinary128 };
            blinkable = new Object[] { lblScore, lblNum };

            //timer initialization
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500); //tick is triggered 2 times every second
            timer.Tick += Timer_Tick;

            //blink initialization
            blinker = new DispatcherTimer();
            blinker.Interval = new TimeSpan(0, 0, 0, 0, 500); //tick is triggered 2 times every second
            blinker.Tick += Blinker_Tick;
            sound1 = new SoundPlayer(Properties.Resources.Beep);
            sound2 = new SoundPlayer(Properties.Resources.FancyBeep);

            //radio button initialization
            rbDec.IsChecked = true;

            //High Score Button initialized
            btnChkHS.IsEnabled = false;
            checkForHighScore();
        }

        private void Blinker_Tick(object sender, EventArgs e)
        {
            blinkTick = !blinkTick;
            if (blinkTick)
            {
                //sound2.Play();
                blinkTime--;
                if (blinkTime == 0)
                {
                    blinker.Stop();
                }
            }

            switch (blinkAdd)
            {
                case 0: //blink both score and number
                    lblScore.Opacity = blinkTick ? 100 : 0;
                    lblNum.Opacity = blinkTick ? 100 : 0;
                    break;
                case 1: //blink only score
                    //sound1.Play();
                    lblScore.Opacity = blinkTick ? 100 : 0;
                    break;
                case 2: //blink only number
                    lblNum.Opacity = blinkTick ? 100 : 0;
                    break;
                default: //blink nothing
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timerTick = !timerTick;
            if (timerTick)
            {
                //sound2.Stop();
                sound2.Play();
                time--;
                lblTimer.Content = (time).ToString().PadLeft(3, '0');
                if (time == 0)
                {
                    timer.Stop(); //stops timer
                    gameEnd();
                }
            }
            if(time <= 5) 
            {
                //plays sound 1 additional time and blinks the time
                sound2.Play();
                lblTimer.Foreground = Brushes.Red; 
                lblTimer.Opacity = timerTick ? 100 : 0; //for blink effect
            }
        }

        private void btnGameStart_Click(object sender, RoutedEventArgs e)
        {
            //starts the game mode
            if (!gameMode)
            {
                changeMode(gameMode);
            }
            init();
            btnGameStart.IsEnabled = false;
            btnTestStart.IsEnabled = false;
            rbDec.IsEnabled = false;
            rbHex.IsEnabled = false;
            //MessageBox.Show("Game State: " + gameMode.ToString() + " Test State: " + testMode.ToString());
        }

        private void init()
        {
            if (btnGameStart.IsEnabled) 
            {
                //if start button is still enabled, it means its the start of a new game so score is reset
                score = 0;
                lblScore.Content = score.ToString().PadLeft(3, '0');
                roundTime = 30;
            }
            //initializes both Reset Labels and Checkbox Switches
            for(int x = 0; x < arrRes.Length;x++)
            {
                //resets binary numbers
                arrRes[x].Content = "0";
                //reset checkbox statuses
                arrChkSwtch[x].IsChecked = false;
                //Checkboxes get enabled if they are disabled
                if (!arrChkSwtch[x].IsEnabled)
                {
                    //enables checkboxes if they are disabled
                    arrChkSwtch[x].IsEnabled = true;
                }
            }

            //tally is reverted to 0
            tally = 0;
            lblTallyTest.Content = tally.ToString(); //used for checking only

            //checks if testing
            if (!testMode)
            {
                //generate number to convert
                num = rnd.Next(1, 256);
                if (rbDec.IsChecked.Value)
                {
                    //display dec number
                    lblNum.Content = num.ToString().PadLeft(3, '0');
                }
                else
                {
                    //display hex number
                    lblNum.Content = decToHexConverter(num).PadLeft(2,'0');
                }

                //set timer
                //resets timer to max number
                time = roundTime;
                //reverts timer color to black
                lblTimer.Opacity = 100;
                lblTimer.Foreground = Brushes.Black;
                lblTimer.Content = roundTime.ToString().PadLeft(3, '0');
                timer.Start();
            }
        }

        private void btnEvent(int dir)
        {
            if(arrChkSwtch[dir].IsChecked.Value)
            {
                //adds to total
                tally += int.Parse(arrVal[dir].Content.ToString());
                arrRes[dir].Content = "1";
            }
            else
            {
                //subtracts from total
                tally += (int.Parse(arrVal[dir].Content.ToString()) * -1);
                arrRes[dir].Content = "0";
            }

            lblTallyTest.Content = tally.ToString(); //used for checking only
            if (gameMode)
            {
                gameChk();
            }
            else
            {
                if (rbDec.IsChecked.Value)
                {
                    lblNum.Content = tally.ToString().PadLeft(3,'0');
                }
                else
                {
                    lblNum.Content = decToHexConverter(tally).PadLeft(2,'0');
                }
            }
        }

        private void gameChk()
        {
            //checks if tally and the generated number matches
            if(tally == num)
            {
                //blink animation
                addToBlink(0,2);
                //stops timer if match
                timer.Stop();
                //add to score
                score++;
                //if roundTime is greater than 5 seconds subtract 1 second upon reset
                if (roundTime > 5)
                {
                    roundTime--;
                }
                //update score
                lblScore.Content = score.ToString().PadLeft(3, '0');
                init();
            }
        }

        private void gameEnd()
        {
            //blinker
            addToBlink(1, 5);
            //re-enables start button
            btnGameStart.IsEnabled = true;
            btnTestStart.IsEnabled = true;
            rbDec.IsEnabled = true;
            rbHex.IsEnabled = true;
            //disables checkboxes
            for (int x = 0; x < arrRes.Length; x++)
            {
                arrRes[x].Content = "0";
                arrChkSwtch[x].IsChecked = false;
                arrChkSwtch[x].IsEnabled = false;
            }
            lblNum.Content = "---";
            lblTimer.Content = "---";

            //only register high score if score greater than 0
            if (score > 0)
            {
                // check if HS registers
                for (int x = 0; x < hsDisp.Score.Length; x++)
                {
                    if (score >= hsDisp.Score[x])
                    {
                        // disregard forloop at this point because we're gonna remove everything at the bottom of it
                        for (int y = hsDisp.Score.Length - 1; y > x; y--)
                        {
                            if (y - 1 >= 0)
                            {
                                hsDisp.Score[y] = hsDisp.Score[y - 1];
                                hsDisp.HSName[y] = hsDisp.HSName[y - 1];
                            }
                        }
                        btnChkHS.IsEnabled = true;
                        hsDisp.Score[x] = score;
                        hsDisp.HSNameDisp[x].IsEnabled = true;
                        hsDisp.HSChange = true;
                        hsDisp.Show();
                        break;
                    }
                }
            }
        }

        private void addToBlink(int add, int dur)
        {
            //duration of blink
            blinkTime = dur;
            //what to blink
            blinkAdd = add;
            //start blinking
            blinker.Start();
        }

        #region Checkbox Events
        private void chkbxBinary128_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary128.Tag.ToString()));
        }

        private void chkbxBinary64_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary64.Tag.ToString()));
        }

        private void chkbxBinary32_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary32.Tag.ToString()));
        }

        private void chkbxBinary16_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary16.Tag.ToString()));
        }

        private void chkbxBinary8_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary8.Tag.ToString()));
        }

        private void chkbxBinary4_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary4.Tag.ToString()));
        }

        private void chkbxBinary2_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary2.Tag.ToString()));
        }

        private void chkbxBinary1_Click(object sender, RoutedEventArgs e)
        {
            btnEvent(int.Parse(chkbxBinary1.Tag.ToString()));
        }
        #endregion

        private void btnTestStart_Click(object sender, RoutedEventArgs e)
        {
            //this mode of the application just allows you to check which binary 
            //corresponds to which decimal number
            changeMode(gameMode);

            if (testMode)
            {
                btnTestStart.Content = "Test End";
                btnGameStart.IsEnabled = false;
                init();
            }
            else
            {
                btnTestStart.Content = "Test Start";
                btnGameStart.IsEnabled = true;
                gameEnd();
            }
        }

        private void rbDec_Checked(object sender, RoutedEventArgs e)
        {
            lblMode.Content = "Decimal";
        }

        private void rbHex_Checked(object sender, RoutedEventArgs e)
        {
            lblMode.Content = "Hexadecimal";
        }

        private string decToHexConverter(int dec)
        {
            int[] decToHex = new int[] { 0, 0 };
            char[] charOfHex = new char[] { '0', '0' };
            string tempHex = string.Empty;

            decToHex[0] = dec / 16;
            decToHex[1] = dec - (decToHex[0] * 16);
            for (int x = 0; x < decToHex.Length; x++)
            {
                switch (decToHex[x])
                {
                    case 10:
                        charOfHex[x] = 'A';
                        break;
                    case 11:
                        charOfHex[x] = 'B';
                        break;
                    case 12:
                        charOfHex[x] = 'C';
                        break;
                    case 13:
                        charOfHex[x] = 'D';
                        break;
                    case 14:
                        charOfHex[x] = 'E';
                        break;
                    case 15:
                        charOfHex[x] = 'F';
                        break;
                    default:
                        charOfHex[x] = (char)(decToHex[x] + 48);
                        break;
                }
            }
            tempHex = new string(charOfHex);
            return tempHex;
        }

        private void changeMode(bool state)
        {
            gameMode = !state;
            //MessageBox.Show("Game State: " + gameMode.ToString());
            testMode = !gameMode;
            //MessageBox.Show("Test State: " + testMode.ToString());

        }

        private void BtnChkHS_Click(object sender, RoutedEventArgs e)
        {
            hsDisp.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if(hsDisp.IsVisible)
            {
                hsDisp.Hide();
            }
        }

        private void checkForHighScore()
        {
            if (File.Exists("HighScore.txt"))
            {
                //read and store data
                btnChkHS.IsEnabled = true;
                using (StreamReader sr = File.OpenText("HighScore.txt"))
                {
                    string s = "";
                    int counter = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] temp = s.Split(',');
                        hsDisp.HSName[counter] = temp[0];
                        hsDisp.Score[counter] = int.Parse(temp[1]);
                        counter++;
                    }
                }

                for(int x = 0; x < hsDisp.HSName.Length; x++)
                {
                    hsDisp.HSNameDisp[x].Text = hsDisp.HSName[x];
                    hsDisp.HSScoreDisp[x].Content = hsDisp.Score[x];
                }
            }
        }
    }
}
