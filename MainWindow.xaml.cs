using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FlappyPuppy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool GameStarted = false;
        bool isJumping = false;
        // ----- amount of % mini-grids [10k / 10k too many D:]--------------
        int width = 100;
        int height = 100;
        // ----- amout of ms before every refresh loop activation ----------- 
        int refreshTime = 15;
        // ----- amount of *rectangles* up ---------------------------------- 
        int jumpHeight = 20;
        // ----- frozen time between going up and falling down -------------- 
        int fallingDelayTime = 0;
        // ----- player Starting cor ---------------------------------------- 
        int circleY;
        int circleX = 15;
        int r = 2;
        int points = 0;
        // ----- first tower Starting cor ----------------------------------- 
        Tuple<int, int> stTowerTopLeft;
        Tuple<int, int> stTowerBottomRight;
        Tuple<int, int> secTowerTopLeft;
        Tuple<int, int> secTowerBottomRight;

        HashSet<Tuple<int, int, int>> colors_Player_Tower_Sky;

        Brush PlayersColor;
        Brush TowersColor;
        Brush TerrainColor = Brushes.AliceBlue;


        public MainWindow()
        {
            InitializeComponent();
            IniVar();
            createDrawingGrids();
            drawObjects();
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }
        private void IniVar()
        {
            GameStarted = false;
            isJumping = false;
            width = width < 100 ? 100 : width;
            height = height < 100 ? 100 : height;
            circleY = height / 2;
            SetNewColors();
            CreateNewTowerCoords();
        }
        private void SetNewColors()
        {
            Random rand = new Random();
            colors_Player_Tower_Sky = new HashSet<Tuple<int, int, int>>();
            while (colors_Player_Tower_Sky.Count < 2)
            {
                int c1 = rand.Next(0, 256);
                int c2 = rand.Next(0, 256);
                int c3 = rand.Next(0, 256);
                colors_Player_Tower_Sky.Add(new Tuple<int, int, int>(c1, c2, c3));
            }
            PlayersColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)colors_Player_Tower_Sky.First().Item1, (byte)colors_Player_Tower_Sky.First().Item2, (byte)colors_Player_Tower_Sky.First().Item3));
            TowersColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)colors_Player_Tower_Sky.Skip(1).First().Item1, (byte)colors_Player_Tower_Sky.Skip(1).First().Item2, (byte)colors_Player_Tower_Sky.Skip(1).First().Item3));
            //TerrainColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)colors_Player_Tower_Sky.Skip(2).First().Item1, (byte)colors_Player_Tower_Sky.Skip(2).First().Item2, (byte)colors_Player_Tower_Sky.Skip(2).First().Item3));
        }
        private void CreateNewTowerCoords()
        {
            Random rand = new Random();
            int randomNumber = rand.Next(0, 66);
            int towerWidthTopLeft = (width / 100 * 90);
            int twtlADD = width / 10;

            stTowerTopLeft = new Tuple<int, int>(0, towerWidthTopLeft + twtlADD);
            stTowerBottomRight = new Tuple<int, int>(height / 100 * randomNumber, width + twtlADD);

            int secH = 100 - (70 - randomNumber);
            secTowerTopLeft = new Tuple<int, int>(height / 100 * secH, towerWidthTopLeft + twtlADD);
            secTowerBottomRight = new Tuple<int, int>(height,width + twtlADD);
        }
        private void createDrawingGrids()
        {
            for (int i = 0; i < height; ++i) 
            {
                RowDefinition row = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition column = new ColumnDefinition() { Width = new GridLength (1, GridUnitType.Star) };

                MainGrid.RowDefinitions.Add(row);
                MainGrid.ColumnDefinitions.Add(column);
                for (int j = 0; j < width; ++j)
                {
                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    rectangle.Fill = TerrainColor;
                    MainGrid.Children.Add(rectangle);
                }
            }
        }
        private void drawObjects()
        {

            foreach (UIElement element in MainGrid.Children)
            {
                System.Windows.Shapes.Rectangle? rectangle = null;

                if (element is System.Windows.Shapes.Rectangle)
                {
                    rectangle = (System.Windows.Shapes.Rectangle)element;
                    if ((Math.Pow((Grid.GetRow(element) - circleY), 2)) + (Math.Pow((Grid.GetColumn(element) - circleX), 2)) <= Math.Pow(r, 2))
                    {
                        rectangle.Fill = PlayersColor;
                        continue;
                    }
                    else if (stTowerTopLeft.Item1 <= Grid.GetRow(element) && Grid.GetRow(element) <= stTowerBottomRight.Item1
                        && stTowerTopLeft.Item2 <= Grid.GetColumn(element) && Grid.GetColumn(element) <= stTowerBottomRight.Item2)
                    {
                        rectangle.Fill = TowersColor;
                        continue;
                    }
                    else if (secTowerTopLeft.Item1 <= Grid.GetRow(element) && Grid.GetRow(element) <= secTowerBottomRight.Item1
                         && secTowerTopLeft.Item2 <= Grid.GetColumn(element) && Grid.GetColumn(element) <= secTowerBottomRight.Item2)
                    {
                        rectangle.Fill = TowersColor;
                        continue;
                    }
                    else
                    {
                        rectangle.Fill = TerrainColor;
                    }
                }
            }
        }
        private async void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space && isJumping == false)
            {
                gameOverTextBlock.Visibility = Visibility.Hidden;
                GameStarted = true;
                isJumping = true;
                int i = 0;

                while (i < jumpHeight && GameStarted)
                {
                    changeTowerCords();
                    if (!((circleY - r) < 0))
                    { 
                        --circleY;
                        drawObjects();
                    }
                    else
                    {
                        ++circleY;
                        drawObjects();
                    }
                    ++i;
                    await Task.Delay(refreshTime);
                }
                isJumping = false;
                falling();
            }
        }

        private async void falling()
        {
            if (!GameStarted) return;

            await Task.Delay(fallingDelayTime);
            while (!isJumping && GameStarted)
            {
                ++circleY;
                changeTowerCords();
                drawObjects();
                await Task.Delay(refreshTime);
                if ((circleY) > height) 
                {
                     GameOver();
                }
            }
        }
        private void changeTowerCords()
        {
            stTowerTopLeft = new Tuple<int, int>(0, stTowerTopLeft.Item2 - 1);
            stTowerBottomRight = new Tuple<int, int>(stTowerBottomRight.Item1, stTowerBottomRight.Item2 - 1);

            secTowerTopLeft = new Tuple<int, int>(secTowerTopLeft.Item1, secTowerTopLeft.Item2 - 1);
            secTowerBottomRight = new Tuple<int, int>(secTowerBottomRight.Item1, secTowerBottomRight.Item2 - 1);

            // T1x <= x AND T2x >= x
            if (stTowerTopLeft.Item2 <= circleX && stTowerBottomRight.Item2 >= circleX)
            {
                // T1y >= y OR T2y <= y
                if(stTowerBottomRight.Item1 >= circleY || secTowerTopLeft.Item1 <= circleY)
                {
                    GameOver();
                }
            }

            if (stTowerBottomRight.Item2 < 0)
            { 
                CreateNewTowerCoords();
                ++points;
                pointsTextBlock.Text = points + "";

                switch(points)
                {
                    case 5:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 10:
                        --refreshTime;
                        SetNewColors();
                        TerrainColor = Brushes.CadetBlue;
                        break;
                    case 15:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 20:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 25:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 30:
                        SetNewColors();
                        --refreshTime;
                        TerrainColor = Brushes.DarkBlue;
                        break;
                    case 35:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 40:
                        SetNewColors();
                        --refreshTime;
                        break;
                    case 45:
                        SetNewColors();
                        --refreshTime;
                        TerrainColor = Brushes.PaleVioletRed;
                        break;
                }
            }

        }

        private async void GameOver()
        {
            points = 0;
            gameOverTextBlock.Visibility = Visibility.Visible;
            gameOverTextBlock.Text = "Game Over \n your score: " + pointsTextBlock.Text; ;
            pointsTextBlock.Text = "0";
            IniVar();
            drawObjects();
            TerrainColor = Brushes.AliceBlue;
            await Task.Delay(1000);
        }

    }
}