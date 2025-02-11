namespace Chess.Pieces {
    public class Rook : Piece {
        public Rook(PieceColour colour) : base(colour, PieceType.Rook, GetRookSymbol(colour)) {
        }

        private static string GetRookSymbol(PieceColour colour) {
            if (colour == PieceColour.White) {
                return "ROOK";
            }

            return "rook";
        }

        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();

            int[] rowRelative = new int[] { -1, 0, 1, 0 };
            int[] colRelative = new int[] { 0, 1, 0, -1 };

            for (int dir = 0; dir < rowRelative.Length; dir++) {
                int newRow = row;
                int newCol = col;

                while (true) {
                    newRow += rowRelative[dir];
                    newCol += colRelative[dir];

                    if (!board.IsInsideBoard(newRow, newCol)) {
                        break;
                    }

                    if (board.Tiles[newRow, newCol] == null) {
                        moves.Add(new Move(row, col, newRow, newCol));
                    } else {
                        if (board.Tiles[newRow, newCol].Colour != Colour) {
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
