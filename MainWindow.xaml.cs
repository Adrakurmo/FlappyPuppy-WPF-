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
        // ----- player Starting cor ---------------------------------------- 
        int circleY;
        int circleX = 15;
        int r = 4;
        List<Tuple<int, int>> currentPosition = new List<Tuple<int, int>>();
        // ----- first tower Starting cor ----------------------------------- 
        Tuple<int, int> stTowerTopLeft;
        Tuple<int, int> stTowerBottomRight;


        public MainWindow()
        {
            // player strat p (15,50)
            // r = 2.5 lets say
            // (x - 15)^2 + (y - 50)^2 = 2.5^2

            InitializeComponent();
            IniVar();
            createDrawingGrids();
            spawnPlayer();
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }
        private void IniVar()
        {
            Random rand = new Random();
            int randomNumber = rand.Next(0, 66);
            circleY = height / 2;
            stTowerTopLeft = new Tuple<int, int>(0, (width / 100 * 90));
            stTowerBottomRight = new Tuple<int, int>(randomNumber, width);
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
        private void spawnPlayer()
        {
            foreach (UIElement element in MainGrid.Children)
            {
                System.Windows.Shapes.Rectangle rectangle = null;

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

                for (int i = 0; i < 25; ++i)
                {
                    changeTowerCords();
                    if (!((circleY - r) < 0))
                    { 
                        --circleY;
                        spawnPlayer();
                    }
                    else
                    {
                        ++circleY;
                        spawnPlayer();
                    }

                    await Task.Delay(refreshTime);
                }
                isJumping = false;
                falling();
            }
        }

        private async void falling()
        {
            await Task.Delay(100);
            while (!isJumping)
            {
                ++circleY;
                changeTowerCords();
                spawnPlayer();
                await Task.Delay(refreshTime);
            }
        }
        private void changeTowerCords()
        {
            stTowerTopLeft = new Tuple<int, int>(0, stTowerTopLeft.Item2 - 1);
            stTowerBottomRight = new Tuple<int, int>(stTowerBottomRight.Item1, stTowerBottomRight.Item2 - 1);

        }

    }
}