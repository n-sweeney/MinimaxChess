namespace Chess.Pieces {
    public class King : Piece {
        public King(PieceColour colour) : base(colour, PieceType.King, GetKingSymbol(colour)) {
        }
        private static string GetKingSymbol(PieceColour colour) {
            if (colour == PieceColour.White) {
                return "KING";
            }

            return "king";
        }

        public override List<Move> GetLegalMoves(Board board, int row, int col) {
            List<Move> moves = new List<Move>();
            for (int rowRelative = -1; rowRelative <= 1; rowRelative++) {
                for (int colRelative = -1; colRelative <= 1; colRelative++) {
                    if (rowRelative == 0 && colRelative == 0) { // Current tile
                        continue;
                    }

                    int newRow = row + rowRelative;
                    int newCol = col + colRelative;

                    if (board.IsInsideBoard(newRow, newCol)) {
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
