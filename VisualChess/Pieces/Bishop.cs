namespace Chess.Pieces {
    public class Bishop : Piece {
        public Bishop(PieceColour colour) : base(colour, PieceType.Bishop, 30) {
        }

        /// <summary>
        /// Generates all legal moves for the current bishop object.
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="row">The current row</param>
        /// <param name="col">the current column</param>
        /// <returns>
        /// List of legal moves (including check moves)
        /// </returns>
        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = [];

            int[] rowRelative = [-1, -1, 1, 1];
            int[] colRelative = [-1, 1, 1, -1];

            for (int dir = 0; dir < rowRelative.Length; dir++) {
                int newRow = row;
                int newCol = col;

                while (true) {
                    newRow += rowRelative[dir];
                    newCol += colRelative[dir];

                    if (!Board.IsInsideBoard(newRow, newCol)) {
                        break;
                    }

                    if (board.Tiles[newRow, newCol] == null) {
                        moves.Add(new Move(row, col, newRow, newCol));
                    } else {
                        if (board.Tiles[newRow, newCol]?.Colour != Colour) {
                            moves.Add(new Move(row, col, newRow, newCol));
                        }

                        break;
                    }
                }
            }

            return moves;
        }
    }
}
