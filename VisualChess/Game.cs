using Chess.Pieces;

namespace Chess {
    public class Game {
        public Board Board { get; set; }
        public PieceColour Turn { get; set; }
        public int Depth { get; set; }

        public Game() {
            Board = new Board();
            Board.Initialise();
            Turn = PieceColour.White;
            Depth = 3;
        }

        public int Minimax(Board board, int depth, bool isMaximizing, PieceColour aiColour) {
            if (depth == 0 || !board.IsKingAlive(PieceColour.White) || !board.IsKingAlive(PieceColour.Black)) {
                return board.Evaluate(aiColour);
            }

            PieceColour currentColour;

            if (isMaximizing) {
                currentColour = aiColour;
            } else {
                currentColour = Opponent(aiColour);
            }

            List<Move> moves = board.GenerateAllMoves(currentColour);

            if (moves.Count == 0) {
                return board.Evaluate(aiColour);
            }

            if (isMaximizing) {
                int maxEval = int.MinValue;

                foreach (var move in moves) {
                    Board newBoard = board.Clone();
                    newBoard.MakeMove(move);
                    int eval = Minimax(newBoard, depth - 1, false, aiColour);
                    maxEval = Math.Max(maxEval, eval);
                }

                return maxEval;
            } else {
                int minEval = int.MaxValue;

                foreach (var move in moves) {
                    Board newBoard = board.Clone();
                    newBoard.MakeMove(move);
                    int eval = Minimax(newBoard, depth - 1, true, aiColour);
                    minEval = Math.Min(minEval, eval);
                }

                return minEval;
            }
        }

        public Move GetBestMove(PieceColour aiColour) {
            List<Move> moves = Board.GenerateAllMoves(aiColour);
            Move bestMove = null;
            int bestEval = int.MinValue;

            foreach (var move in moves) {
                Board newBoard = Board.Clone();
                newBoard.MakeMove(move);

                int eval = Minimax(newBoard, Depth - 1, false, aiColour);
                if (eval > bestEval) {
                    bestEval = eval;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        public PieceColour Opponent(PieceColour colour) {
            if (colour == PieceColour.White) {
                return PieceColour.Black;
            }

            return PieceColour.White;
        }

        public Move ParseMove(string input) {
            if (input.Length != 4) {
                return null;
            }

            int fromCol = input[0] - 'a';
            int fromRow = 8 - (input[1] - '0');
            int toCol = input[2] - 'a';
            int toRow = 8 - (input[3] - '0');

            if (!Board.IsInsideBoard(fromRow, fromCol) || !Board.IsInsideBoard(toRow, toCol)) {
                return null;
            }

            return new Move(fromRow, fromCol, toRow, toCol);
        }

        public void Play() {
            while (true) {
                //Board.Print();

                if (!Board.IsKingAlive(PieceColour.White)) {
                    Console.WriteLine("Black wins");
                    break;
                }

                if (!Board.IsKingAlive(PieceColour.Black)) {
                    Console.WriteLine("White wins");
                    break;
                }

                if (Turn == PieceColour.White) {
                    Console.WriteLine("Player's move (e.g. e2e4): ");
                    string input = Console.ReadLine();
                    Move move = ParseMove(input);
                    if (move == null) {
                        Console.WriteLine("Invalid move");
                        continue;
                    }

                    List<Move> legalMoves = Board.GenerateAllMoves(Turn);
                    bool isLegal = false;

                    foreach (var legal in legalMoves) {
                        if (legal.FromRow == move.FromRow && legal.FromCol == move.FromCol &&
                            legal.ToRow == move.ToRow && legal.ToCol == move.ToCol) {
                            isLegal = true;
                            break;
                        }
                    }

                    if (!isLegal) {
                        Console.WriteLine("Illegal move");
                        continue;
                    }

                    Board.MakeMove(move);
                    Turn = Opponent(Turn);
                } else {
                    Console.WriteLine("AI is AIing");
                    Move bestMove = GetBestMove(Turn);

                    if (bestMove == null) {
                        Console.WriteLine("No legal moves for AI - Player wins");
                        break;
                    }

                    Console.WriteLine("AI plays: " + bestMove);
                    Board.MakeMove(bestMove);
                    Turn = Opponent(Turn);
                }
            }
        }
    }
}
