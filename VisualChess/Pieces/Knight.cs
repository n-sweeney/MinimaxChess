namespace Chess.Pieces {
    public class Knight : Piece {
        public Knight(PieceColour colour) : base(colour, PieceType.Knight, GetKnightSymbol(colour)) {
        }
        private static string GetKnightSymbol(PieceColour colour) {
            if (colour == PieceColour.White) {
                return "KNIGHT";
            }

            return "knight";
        }
        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();

            int[] rowRelative = new int[] { -2, -1, 1, 2, 2, 1, -1, -2 };
            int[] colRelative = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 };

            for (int dir = 0; dir < rowRelative.Length; dir++) {
                int newRow = row;
                int newCol = col;

                newRow += rowRelative[dir];
                newCol += colRelative[dir];

                if (!board.IsInsideBoard(newRow, newCol)) {
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
