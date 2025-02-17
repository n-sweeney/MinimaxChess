﻿namespace Chess.Pieces {
    public class King : Piece {
        public King(PieceColour colour) : base(colour, PieceType.King, 900) {
        }

        /// <summary>
        /// Generates all legal moves for the current king object.
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="row">The current row</param>
        /// <param name="col">the current column</param>
        /// <returns>
        /// List of legal moves (including check moves)
        /// </returns>
        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();
            for (int rowRelative = -1; rowRelative <= 1; rowRelative++) {
                for (int colRelative = -1; colRelative <= 1; colRelative++) {
                    if (rowRelative == 0 && colRelative == 0) { // Current tile
                        continue;
                    }

                    int newRow = row + rowRelative;
                    int newCol = col + colRelative;

                    if (Board.IsInsideBoard(newRow, newCol)) {
                        if (board.Tiles[newRow, newCol] == null || board.Tiles[newRow, newCol].Colour != Colour) {
                            moves.Add(new Move(row, col, newRow, newCol));
                        }
                    }
                }
            }

            return moves;
        }
    }
}
