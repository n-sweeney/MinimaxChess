namespace Chess.Pieces {
    public class Knight : Piece {
        public Knight(PieceColour colour) : base(colour, PieceType.Knight, 30) {
        }

        /// <summary>
        /// Generates all legal moves for the current knight object.
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="row">The current row</param>
        /// <param name="col">the current column</param>
        /// <returns>
        /// List of legal moves (including check moves)
        /// </returns>
        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();

            int[] rowRelative = [-2, -1, 1, 2, 2, 1, -1, -2];
            int[] colRelative = [1, 2, 2, 1, -1, -2, -2, -1];

            for (int dir = 0; dir < rowRelative.Length; dir++) {
                int newRow = row;
                int newCol = col;

                newRow += rowRelative[dir];
                newCol += colRelative[dir];

                if (!Board.IsInsideBoard(newRow, newCol)) {
                    continue;
                }

                if (board.Tiles[newRow, newCol] == null || board.Tiles[newRow, newCol].Colour != Colour) {
                    moves.Add(new Move(row, col, newRow, newCol));
                }
            }

            return moves;
        }
    }
}
