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
        int circleY = 50;
        int circleX = 15;
        int r = 4;
        bool GameStarted = false;
        bool isJumping = false;
        int width = 100;
        int height = 100;

        List<Tuple<int,int>> currentPosition = new List<Tuple<int,int>>();
        public MainWindow()
        {
            // player strat p (15,50)
            // r = 2.5 lets say
            // (x - 15)^2 + (y - 50)^2 = 2.5^2

            InitializeComponent();
            createDrawingGrids();
            spawnPlayer();
            PreviewKeyDown += MainWindow_PreviewKeyDown;
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
                    if ((Math.Pow((Grid.GetRow(element) - circleY), 2)) + (Math.Pow((Grid.GetColumn(element) - circleX), 2)) <= Math.Pow(r, 2))
                    {
                        currentPosition.Add(new Tuple<int, int>(Grid.GetColumn(element), Grid.GetRow(element)));
                        rectangle = (System.Windows.Shapes.Rectangle)element;
                        rectangle.Fill = Brushes.Black;
                    }
                    else
                    {
                        ((System.Windows.Shapes.Rectangle)element).Fill = airColor;
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
                    --circleY;
                    if (!((circleY - r) <= 0))
                        spawnPlayer();
                    await Task.Delay(15);
                }
                isJumping = false;
                falling();
            }
        }

        async void falling()
        {
            await Task.Delay(100);
            while (!isJumping)
            {
                ++circleY;
                spawnPlayer();
                await Task.Delay(15);
            }
        }


    }
}