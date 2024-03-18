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
        Brush airColor = Brushes.AliceBlue;

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
        int fallingDelayTime = 50;
        // ----- player Starting cor ---------------------------------------- 
        int circleY;
        int circleX = 15;
        int r = 2;
        List<Tuple<int, int>> currentPosition = new List<Tuple<int, int>>();
        // ----- first tower Starting cor ----------------------------------- 
        Tuple<int, int> stTowerTopLeft;
        Tuple<int, int> stTowerBottomRight;
        Tuple<int, int> secTowerTopLeft;
        Tuple<int, int> secTowerBottomRight;


        public MainWindow()
        {
            // player strat p (15,50)
            // r = 2.5 lets say
            // (x - 15)^2 + (y - 50)^2 = 2.5^2

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
            CreateNewTowerCoords();
        }
        private void CreateNewTowerCoords()
        {
            Random rand = new Random();
            int randomNumber = rand.Next(0, 66);
            int towerWidthTopLeft = (width / 100 * 90);

            stTowerTopLeft = new Tuple<int, int>(0, towerWidthTopLeft);
            stTowerBottomRight = new Tuple<int, int>(height / 100 * randomNumber, width);

            int secH = 100 - (70 - randomNumber);
            secTowerTopLeft = new Tuple<int, int>(height / 100 * secH, towerWidthTopLeft);
            secTowerBottomRight = new Tuple<int, int>(height,width);
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
                    rectangle.Fill = airColor;
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
                        currentPosition.Add(new Tuple<int, int>(Grid.GetColumn(element), Grid.GetRow(element)));
                        rectangle.Fill = Brushes.MediumPurple;
                    }
                    else if (stTowerTopLeft.Item1 <= Grid.GetRow(element) && Grid.GetRow(element) <= stTowerBottomRight.Item1
                        && stTowerTopLeft.Item2 <= Grid.GetColumn(element) && Grid.GetColumn(element) <= stTowerBottomRight.Item2)
                    {
                        rectangle.Fill = Brushes.RosyBrown;
                    }
                    else if (secTowerTopLeft.Item1 <= Grid.GetRow(element) && Grid.GetRow(element) <= secTowerBottomRight.Item1
                         && secTowerTopLeft.Item2 <= Grid.GetColumn(element) && Grid.GetColumn(element) <= secTowerBottomRight.Item2)
                    {
                        rectangle.Fill = Brushes.IndianRed;
                    }
                    else
                    {
                        rectangle.Fill = airColor;
                    }
                }
            }
        }
        private async void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && isJumping == false)
            {
                GameStarted = true;
                isJumping = true;

                for (int i = 0; i < jumpHeight; ++i)
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
            while (!isJumping)
            {
                ++circleY;
                changeTowerCords();
                drawObjects();
                await Task.Delay(refreshTime);
                if ((circleY) > height) 
                {
                    IniVar();
                    return;
                }
            }
        }
        private void changeTowerCords()
        {
            stTowerTopLeft = new Tuple<int, int>(0, stTowerTopLeft.Item2 - 1);
            stTowerBottomRight = new Tuple<int, int>(stTowerBottomRight.Item1, stTowerBottomRight.Item2 - 1);

            secTowerTopLeft = new Tuple<int, int>(secTowerTopLeft.Item1, secTowerTopLeft.Item2 - 1);
            secTowerBottomRight = new Tuple<int, int>(secTowerBottomRight.Item1, secTowerBottomRight.Item2 - 1);

            if (stTowerBottomRight.Item2 < 0) CreateNewTowerCoords();

        }

    }
}