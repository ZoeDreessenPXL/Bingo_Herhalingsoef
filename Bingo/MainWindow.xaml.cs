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
using System.Windows.Threading;

namespace Bingo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random _random = new Random();
        DispatcherTimer _timer = new DispatcherTimer();
        List<int> _numbers;
        int _player1Points = 0;
        int _player2Points = 0;
        bool _oneRow;
        bool _twoRows;

        public MainWindow()
        {
            InitializeComponent();

            // Set timer to generate a number every 5 seconds
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
        }

        // Generate the bingo grid
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
                        label.MouseDoubleClick += Label_Clicked;
                        bingoGrid.Children.Add(label);
                    }
                }
            }
        }

        // Check if clicked label is a chosen number
        private void Label_Clicked(object sender, RoutedEventArgs e)
        {
            Label label = (Label)sender;
            int.TryParse(label.Content.ToString(), out int number);
            if (_numbers.Contains(number))
            {
                label.Background = Brushes.White;
                checkCardFull((Grid)label.Parent);
            }
        }

        // Checks the amount of full rows
        private void checkCardFull(Grid bingoGrid)
        {
            Label[] gridLabels = bingoGrid.Children.OfType<Label>().ToArray();
            Dictionary<int, List<int>> rows = new Dictionary<int, List<int>> { { 0, new List<int>() }, { 1, new List<int>() }, { 2, new List<int>() }, { 3, new List<int>() }, { 4, new List<int>() } };
            Dictionary<int, List<int>> cols = new Dictionary<int, List<int>> { { 0, new List<int>() }, { 1, new List<int>() }, { 2, new List<int>() }, { 3, new List<int>() }, { 4, new List<int>() } };
            rows[2].Add(2);
            cols[2].Add(2);
            int diagonal1 = 0;
            int diagonal2 = 0;
            int rowsFull = 0;

            foreach (Label label in gridLabels)
            {
                if (label.Background == Brushes.White)
                {
                    int row = Grid.GetRow(label);
                    int col = Grid.GetColumn(label);

                    rows[row].Add(col);
                    cols[col].Add(row);
                }
            }

            // keep track of a full row
            foreach (var row in rows)
            {
                if ((row.Key == 0 && row.Value.Contains(0)) || (row.Key == 1 && row.Value.Contains(1)) 
                    || (row.Key == 3 && row.Value.Contains(3)) || (row.Key == 4 && row.Value.Contains(4))) 
                {
                    diagonal1++;
                }
                if ((row.Key == 0 && row.Value.Contains(4)) || (row.Key == 1 && row.Value.Contains(3))
                    || (row.Key == 3 && row.Value.Contains(1)) || (row.Key == 4 && row.Value.Contains(0)))
                { 
                    diagonal2++;
                }

                bool[] conditions = new bool[]
                {
                    row.Value.Count == 5,
                    diagonal1 == 4,
                    diagonal2 == 4
                };

                rowsFull += conditions.Count(c => c);
            }
            // keep track of a full column
            foreach (var col in cols)
            {
                if (col.Value.Count == 5)
                {
                    rowsFull++;
                }
            }
            // Send to check for how many rows playing
            gameLevel(rowsFull, bingoGrid.Name);
        }

        // Check how many rows playing and if a player has the needed amount
        private void gameLevel(int rows, string name)
        {
            if (!_oneRow && rows >= 1)
            {
                playerWon(name);
                _oneRow = true;
            }
            else if (!_twoRows && rows >= 2)
            {
                playerWon(name);
                _twoRows = true;
            }
            else if (_twoRows && rows == 12)
            {
                _timer.Stop();
                startGameButton.Visibility = Visibility.Visible;
                chosenNumbersListBox.Items.Clear();
                lastChosenNumberTextBlock.Text = string.Empty;
                playerWon(name);
            }
        }

        // Player has won
        private void playerWon(string name)
        {
            string player;

            if (name.Contains('1'))
            {
                player = player1NameTextBlock.Text;
                _player1Points++;
                player1PointsTextBlock.Text = _player1Points.ToString();
            }
            else
            {
                player = player2NameTextBlock.Text;
                _player2Points++;
                player2PointsTextBlock.Text = _player2Points.ToString();
            }
            MessageBox.Show($"{player} heeft gewonnen!");
        }

        // Fill in the bingo cards with random numbers
        private void GeneratePlayerCard(Grid bingoGrid)
        {
            //Get all Label controls from Grid:
            Label[] gridLabels = bingoGrid.Children.OfType<Label>().ToArray();
            List<int> usedNumbers = new List<int>();

            foreach (Label label in gridLabels)
            {
                label.Background = Brushes.Transparent;
                //Get row + columns for label:
                int row = Grid.GetRow(label);
                int col = Grid.GetColumn(label);

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

        // Start game
        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePlayerCard(player1Grid);
            GeneratePlayerCard(player2Grid);
            _numbers = new List<int>();
            _oneRow = false;
            _twoRows = false;
            _timer.Start();

            startGameButton.Visibility = Visibility.Hidden;
        }

        // Generate new number
        private void Timer_Tick(object sender, EventArgs e)
        {
            int number;
            do
            {
                number = _random.Next(1, 76);
            } while (_numbers.Contains(number));
            _numbers.Add(number);
            chosenNumbersListBox.Items.Add(number.ToString());
            lastChosenNumberTextBlock.Text = number.ToString();
        }
    }
}