namespace Chess.Pieces {
    public class Pawn : Piece {
        public Pawn(PieceColour colour) : base(colour, PieceType.Pawn, 10) { }

        /// <summary>
        /// Generates all legal moves for the current pawn object.
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="row">The current row</param>
        /// <param name="col">the current column</param>
        /// <returns>
        /// List of legal moves (including check moves)
        /// </returns>
        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();
            int direction;
            int startRow;

            if (Colour == PieceColour.White) {
                direction = -1;
                startRow = 6;
            } else {
                direction = 1;
                startRow = 1;
            }

            int nextRow = row + direction;

            // Single forward
            if (Board.IsInsideBoard(nextRow, col) && board.Tiles[nextRow, col] == null) {
                moves.Add(new Move(row, col, nextRow, col));
                // Double forward (only from starting)
                int doubleRow = row + 2 * direction;
                if (row == startRow && Board.IsInsideBoard(doubleRow, col) && board.Tiles[doubleRow, col] == null) {
                    moves.Add(new Move(row, col, doubleRow, col));
                }
            }

            // Diagonal captures
            foreach (int dc in new int[] { -1, 1 }) {
                int newCol = col + dc;

                if (Board.IsInsideBoard(nextRow, newCol) && board.Tiles[nextRow, newCol] != null && board.Tiles[nextRow, newCol].Colour != Colour) {
                    moves.Add(new Move(row, col, nextRow, newCol));
                }
            }

            return moves;
        }
    }
}
