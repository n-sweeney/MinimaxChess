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
    }
}
