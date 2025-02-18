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
        /// <summary>
        /// Main constructor for UI
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            game = new Game();
            GenerateChessBoard();
            DisplayBoard();
            StartTimer();
            DisplayTurn(game.Turn);
        }


        /// <summary>
        /// Creates and starts timer for game time
        /// </summary>
        private void StartTimer() {
            elapsedTime = TimeSpan.Zero;
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += TimerTick;
            gameTimer.Start();
        }

        /// <summary>
        /// Increments the timer by one second and updates the display.
        /// </summary>
        /// <param name="sender">game timer object</param>
        /// <param name="e">The event data associated with the timer tick</param>
        private void TimerTick(object sender, EventArgs e) {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerText.Text = elapsedTime.ToString(@"mm\:ss");
        }

        /// <summary>
        /// Creates turn UI for player provided. Used when each turn is switched.
        /// </summary>
        /// <param name="player">The current player</param>
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


        /// <summary>
        /// Generates the Chess board UI by generating the grid's buttons
        /// </summary>
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

        /// <summary>
        /// Updates current grid's buttons to display the current piece locations and taken pieces
        /// </summary>
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

            WhiteTakenPanel.Children.Clear();
            BlackTakenPanel.Children.Clear();

            foreach (var taken in game.Board.WhitePieces) {
                Image takenImage = new Image {
                    Source = new BitmapImage(new Uri(taken.GetImagePath(), UriKind.Relative)),
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(5)
                };

                WhiteTakenPanel.Children.Add(takenImage);
            }

            foreach (var taken in game.Board.BlackPieces) {
                Image takenImage = new Image {
                    Source = new BitmapImage(new Uri(taken.GetImagePath(), UriKind.Relative)),
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(5)
                };

                BlackTakenPanel.Children.Add(takenImage);
            }
        }

        /// <summary>
        /// Handles logic for when a tile is selected. It determines if it is the first or second button and completes the necessary actions.
        /// If it is the first button, the legal moves are displayed for the player.
        /// If it is the second button, it determines if it is a legal move and plays it. The computer now calculates and completes its go.
        /// </summary>
        /// <param name="sender">The button pressed</param>
        /// <param name="e">The event data associated with the tile selection</param>
        /// <returns>
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

                    List<Move> legalMoves = game.Board.GetValidMoves(game.Turn);

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
                List<Move> legalMoves = game.Board.GetValidMoves(game.Turn);

                foreach (Move legalMove in legalMoves) {
                    if (legalMove.FromRow == move.FromRow && legalMove.FromCol == move.FromCol && legalMove.ToRow == move.ToRow && legalMove.ToCol == move.ToCol) {
                        isLegal = true;
                        break;
                    }
                }

                if (isLegal) {
                    game.Board.MakeMove(move);
                    game.Turn = Game.Opponent(game.Turn);
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
                            game.Turn = Game.Opponent(game.Turn);
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
                            MessageBox.Show("Computer has no legal moves left"); // TO DO: Check if checkmate or stalemate
                        }
                    }
                } else {
                    MessageBox.Show("Invalid move - try again");
                    DeselectSquare();
                    DisplayBoard();
                }
            }
        }

        /// <summary>
        /// Deselects the current square. Used when a go is completed or the move is invalid.
        /// </summary>
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