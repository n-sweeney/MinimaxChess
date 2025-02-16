using Chess;
using Chess.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public MainWindow() {
            InitializeComponent();
            game = new Game();
            GenerateChessBoard();
            DisplayBoard();

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

                    if (piece != null) {
                        string url = piece.GetImagePath();

                        Image pieceImage = new Image {
                            Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute)),
                            Width = 70,
                            Height = 70,
                            Stretch = Stretch.Uniform
                        };

                        tiles[i, j].Content = pieceImage;
                    }
                }
            }
        }

        private void TileSelected(object sender, RoutedEventArgs e) {
            Button? clickedButton = sender as Button;
            if (clickedButton == null)
                return;

            var pos = (Tuple<int, int>)clickedButton.Tag;
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
                }
            } else {
                if (selectedRow == row && selectedCol == col) {
                    DeselectSquare();
                    return;
                }

                Move move = new Move(selectedRow.Value, selectedCol.Value, row, col);

                bool isLegal = false;
                var legalMoves = game.Board.GenerateAllMoves(game.Turn);

                foreach (var legalMove in legalMoves) {
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

                    if (!game.Board.IsKingAlive(PieceColour.White) || !game.Board.IsKingAlive(PieceColour.Black)) {
                        MessageBox.Show("Game over");
                        return;
                    }

                    if (game.Turn == PieceColour.Black) {
                        Move aiMove = game.GetBestMove(PieceColour.Black);
                        if (aiMove != null) {
                            game.Board.MakeMove(aiMove);
                            game.Turn = game.Opponent(game.Turn);
                            DisplayBoard();

                            if (!game.Board.IsKingAlive(PieceColour.White) || !game.Board.IsKingAlive(PieceColour.Black)) {
                                MessageBox.Show("Game over");
                                return;
                            }
                        } else {
                            MessageBox.Show("Computer has no legal moves left");
                        }
                    }
                } else {
                    MessageBox.Show("Invalid move");
                    DeselectSquare();
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