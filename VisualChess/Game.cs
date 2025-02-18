using Chess.Pieces;

namespace Chess {
    public class Game {
        public Board Board { get; set; }
        public PieceColour Turn { get; set; }
        public int Depth { get; set; }

        /// <summary>
        /// Main Game constructor
        /// </summary>
        public Game() {
            Board = new Board();
            Board.Initialise();
            Turn = PieceColour.White;
            Depth = 3;
        }

        /// <summary>
        /// Authenticates a user based on a username and password.
        /// </summary>
        /// <param name="board">The current depth's board</param>
        /// <param name="depth">the current remaining depth to explore</param>
        /// <param name="isMaximizing">If to maximise (computer's turn) or not (player's turn)</param>
        /// <param name="aiColour">The colour of the computer</param>
        /// <returns>
        /// The current board's best case score.
        /// </returns>
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

            List<Move> moves = board.GetValidMoves(currentColour);

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

        /// <summary>
        /// Determines the best move for the computer to make based on the results of the minimax algorithm
        /// </summary>
        /// <param name="aiColour">The computers colour to maximise</param>
        /// <returns>
        /// Best move (if exists) for computer to make
        /// </returns>
        public Move? GetBestMove(PieceColour aiColour) {
            List<Move> moves = Board.GetValidMoves(aiColour);
            Move? bestMove = null;
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

        /// <summary>
        /// Helper function to return the opposite colour for a provided colour
        /// </summary>
        /// <param name="colour">Current colour</param>
        /// <returns>
        /// Opponent Colour
        /// </returns>
        public static PieceColour Opponent(PieceColour colour) {
            if (colour == PieceColour.White) {
                return PieceColour.Black;
            }

            return PieceColour.White;
        }
    }
}
