using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bingo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeGrid(player1Grid);
            InitializeGrid(player2Grid);
        }

        private void InitializeGrid(Grid bingoGrid)
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (row == 2 && col == 2)
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(@"logoPXL.png", UriKind.RelativeOrAbsolute));
                        image.Stretch = Stretch.Uniform;
                        Grid.SetColumn(image, col);
                        Grid.SetRow(image, row);
                        bingoGrid.Children.Add(image);
                    }
                    else
                    {
                        Label label = new Label();
                        Grid.SetColumn(label, col);
                        Grid.SetRow(label, row);
                        label.HorizontalAlignment = HorizontalAlignment.Stretch;
                        label.VerticalAlignment = VerticalAlignment.Stretch;
                        label.HorizontalContentAlignment = HorizontalAlignment.Center;
                        label.VerticalContentAlignment = VerticalAlignment.Center;
                        label.BorderBrush = Brushes.Black;
                        label.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
                        bingoGrid.Children.Add(label);
                    }
                }
            }
        }

        private void GeneratePlayerCard(Grid bingoGrid)
        {
            //Get all Label controls from Grid:
            Label[] gridLabels = bingoGrid.Children.OfType<Label>().ToArray();
            List<int> usedNumbers = new List<int>();

            foreach (Label label in gridLabels)
            {
                //Get row + columns for label:
                int row = Grid.GetRow(label);
                int col = Grid.GetColumn(label);

                if (row == 2 && col == 2)
                {
                    continue;
                }

                int min = 0;
                int max = 0;
                int number;

                switch (col)
                {
                    case 0: 
                        min = 1; 
                        max = 15; 
                        break;
                    case 1: 
                        min = 16; 
                        max = 30; 
                        break;
                    case 2: 
                        min = 31; 
                        max = 45; 
                        break;
                    case 3: 
                        min = 46; 
                        max = 60; 
                        break;
                    case 4: 
                        min = 61; 
                        max = 75; 
                        break;
                }
    
                do
                {
                    number = _random.Next(min, (max + 1));
                }
                while (usedNumbers.Contains(number));

                usedNumbers.Add(number);
                label.Content = number;
            }
        }

        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePlayerCard(player1Grid);
            GeneratePlayerCard(player2Grid);
        }
    }
}