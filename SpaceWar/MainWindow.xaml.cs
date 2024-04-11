using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpaceWar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveUp, moveDown;
        List <Rectangle> itemRemover = new List <Rectangle>();

        Random rand = new Random();

        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 10;
        int limit = 50;
        int score = 0;
        int enemySpeed = 7;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemyCounter -= 1;

            scoreText.Content = "Счёт: " + score;


            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveUp == true && Canvas.GetTop(player) > 0)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
            }

            if (moveDown == true && Canvas.GetTop(player) + player.Height < Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetLeft(x) > Application.Current.MainWindow.Width)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            
                            ImageBrush enemyBang = new ImageBrush();
                            enemyBang.ImageSource = new BitmapImage(new Uri("pack://application:,,,/изображения/bang.png"));

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(x);
                                y.Fill = enemyBang;
                                score += 10;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - enemySpeed);

                    if (Canvas.GetLeft(x) < 0)
                    {
                        itemRemover.Add(x);
                        score -= 5;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);  
                    
                    if (playerHitBox.IntersectsWith(enemyHitBox)) 
                    {
                        GameOver();
                    }
                }
            }

            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                moveUp = true;
            }

            if (e.Key == Key.Down)
            {
                moveDown = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                moveUp = false;
            }

            if (e.Key == Key.Down)
            {
                moveDown = false;
            }

            if (e.Key == Key.Space)
            {

                ImageBrush imageShot= new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/изображения/shot.png")));

                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 25,
                    Width = 40,
                    Fill = imageShot
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width - newBullet.Width);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) + player.Height / 2 - newBullet.Height / 2);

                MyCanvas.Children.Add(newBullet);
            }
        }

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 5);

            enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/изображения/stone.png"));

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 50,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, rand.Next(50, 500)); 
            Canvas.SetLeft(newEnemy, MyCanvas.ActualWidth);
            MyCanvas.Children.Add(newEnemy);

            
        }

        private void GameOver()
        {
            player.Visibility = Visibility.Hidden;
            gameTimer.Stop();
            GameOverText.Visibility = Visibility.Visible;
        }
    }
}
