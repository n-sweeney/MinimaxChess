using Chess;
using Chess.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MinimaxChess {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Game game;
        private Button[,] tiles = new Button[8, 8];

        private int? selectedRow = null;
        private int? selectedCol = null;
        private Button? selectedTile = null;

        private DispatcherTimer gameTimer;
        private TimeSpan elapsedTime;

        private DispatcherTimer turnTimer = new DispatcherTimer();

        public MainWindow() {
            InitializeComponent();
            game = new Game();
            GenerateChessBoard();
            DisplayBoard();
            StartTimer();
            DisplayTurn(game.Turn);
        }

        private void StartTimer() {
            elapsedTime = TimeSpan.Zero;
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += TimerTick;
            gameTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e) {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerText.Text = elapsedTime.ToString(@"mm\:ss"); // Format as MM:SS
        }

        private void DisplayTurn(PieceColour player) {

            if (player == PieceColour.White) {
                TurnText.Text = "White's Turn";
            } else {
                TurnText.Text = "Black's Turn";
            }

            turnTimer.Interval = TimeSpan.FromSeconds(1.5);
            turnTimer.Tick += (s, e) => {
                TurnBorder.Visibility = Visibility.Collapsed;
                TurnText.Visibility = Visibility.Collapsed;
                turnTimer.Stop();
            };
            turnTimer.Start();

            TurnText.Visibility = Visibility.Visible;
            TurnBorder.Visibility = Visibility.Visible;
        }

        private void GenerateChessBoard() {
            for (int i = 0; i < Chess.Board.BOARDSIZE; i++) {
                for (int j = 0; j < Chess.Board.BOARDSIZE; j++) {
                    bool isLight = (i + j) % 2 == 0;
                    string colLetter = ((char)('A' + j)).ToString();
                    string rowNumber = (Chess.Board.BOARDSIZE - i).ToString();

                    Button btn = new Button();
                    btn.Content = String.Empty;
                    btn.Name = colLetter + rowNumber;

                    if (isLight) {
                        btn.Background = new SolidColorBrush(Colors.BurlyWood);
                    } else {
                        btn.Background = new SolidColorBrush(Colors.Brown);
                    }

                    btn.Tag = Tuple.Create(i, j);

                    btn.Click += TileSelected;

                    tiles[i, j] = btn;

                    UniformGridBoard.Children.Add(btn);
                }
            }
        }

        private void DisplayBoard() {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    Piece? piece = game.Board.Tiles[i, j];

                    tiles[i, j].Content = null;
                    tiles[i, j].BorderThickness = new Thickness(0);

                    if (piece != null) {
                        Image pieceImage = new Image {
                            Source = new BitmapImage(new Uri(piece.GetImagePath(), UriKind.RelativeOrAbsolute)),
                            Width = 70,
                            Height = 70,
                            Stretch = Stretch.Uniform
                        };

                        tiles[i, j].Content = pieceImage;
                    }
                }
            }

            WhiteCapturedPanel.Children.Clear();
            BlackCapturedPanel.Children.Clear();

            foreach (var taken in game.Board.whitePieces) {
                Image takenImage = new Image {
                    Source = new BitmapImage(new Uri(taken.GetImagePath(), UriKind.Relative)),
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(5)
                };

                WhiteCapturedPanel.Children.Add(takenImage);
            }

            foreach (var taken in game.Board.blackPieces) {
                Image takenImage = new Image {
                    Source = new BitmapImage(new Uri(taken.GetImagePath(), UriKind.Relative)),
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(5)
                };

                BlackCapturedPanel.Children.Add(takenImage);
            }
        }

        private async void TileSelected(object sender, RoutedEventArgs e) {
            Button? clickedButton = sender as Button;
            if (clickedButton == null)
                return;

            Tuple<int, int> pos = (Tuple<int, int>)clickedButton.Tag;
            int row = pos.Item1;
            int col = pos.Item2;

            if (selectedRow == null || selectedCol == null) {
                Piece? piece = game.Board.Tiles[row, col];
                if (piece != null && piece.Colour == game.Turn) {
                    selectedRow = row;
                    selectedCol = col;
                    selectedTile = clickedButton;
                    selectedTile.BorderThickness = new Thickness(3);
                    selectedTile.BorderBrush = Brushes.Red;

                    var legalMoves = game.Board.GenerateAllMoves(game.Turn);

                    foreach (Move legalMove in legalMoves) {
                        if (legalMove.FromRow == selectedRow && legalMove.FromCol == selectedCol) {
                            Button tile = tiles[legalMove.ToRow, legalMove.ToCol];

                            tile.BorderThickness = new Thickness(3);
                            tile.BorderBrush = Brushes.Green;
                        }
                    }

                }
            } else {
                if (selectedRow == row && selectedCol == col) {
                    DeselectSquare();
                    DisplayBoard();
                    return;
                }

                Move move = new Move(selectedRow.Value, selectedCol.Value, row, col);

                bool isLegal = false;
                List<Move> legalMoves = game.Board.GenerateAllMoves(game.Turn);

                foreach (Move legalMove in legalMoves) {
                    if (legalMove.FromRow == move.FromRow && legalMove.FromCol == move.FromCol && legalMove.ToRow == move.ToRow && legalMove.ToCol == move.ToCol) {
                        isLegal = true;
                        break;
                    }
                }

                if (isLegal) {
                    game.Board.MakeMove(move);
                    game.Turn = game.Opponent(game.Turn);
                    DisplayBoard();
                    DeselectSquare();

                    if (!game.Board.IsKingAlive(PieceColour.White)) {
                        MessageBox.Show("Game over - Black Wins!");
                        return;
                    } else if (!game.Board.IsKingAlive(PieceColour.Black)) {
                        MessageBox.Show("Game over - White Wins!");
                        return;
                    }

                    DisplayTurn(game.Turn);
                    if (game.Turn == PieceColour.Black) {
                        Move? aiMove = game.GetBestMove(PieceColour.Black);

                        await Task.Delay(2000);
                        if (aiMove != null) {
                            game.Board.MakeMove(aiMove);
                            game.Turn = game.Opponent(game.Turn);
                            DisplayTurn(game.Turn);
                            DisplayBoard();

                            if (!game.Board.IsKingAlive(PieceColour.White)) {
                                MessageBox.Show("Game over - Black Wins!");
                                return;
                            } else if (!game.Board.IsKingAlive(PieceColour.Black)) {
                                MessageBox.Show("Game over - White Wins!");
                                return;
                            }
                        } else {
                            MessageBox.Show("Computer has no legal moves left"); // Check if checkmate or stalemate
                        }
                    }
                } else {
                    MessageBox.Show("Invalid move - try again");
                    DeselectSquare();
                    DisplayBoard();
                }
            }
        }
        private void DeselectSquare() {
            if (selectedTile != null) {
                selectedTile.BorderThickness = new Thickness(0);
            }

            selectedRow = null;
            selectedCol = null;
            selectedTile = null;
        }
    }
}