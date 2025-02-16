namespace Chess.Pieces {
    public enum PieceColour { White, Black }
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }

    public abstract class Piece {
        public PieceColour Colour { get; set; }
        public PieceType Type { get; set; }

        public Piece(PieceColour colour, PieceType type) {
            Colour = colour;
            Type = type;
        }

        public abstract List<Chess.Move> GetLegalMoves(Chess.Board board, int row, int col);

        // Copy piece for future moves when copying board
        public virtual Piece Clone() {
            switch (Type) {
                case PieceType.Pawn: {
                    return new Pawn(Colour);
                }

                case PieceType.Knight: {
                    return new Knight(Colour);
                }

                case PieceType.Bishop: {
                    return new Bishop(Colour);
                }

                case PieceType.Rook: {
                    return new Rook(Colour);
                }

                case PieceType.Queen: {
                    return new Queen(Colour);
                }

                case PieceType.King: {
                    return new King(Colour);
                }

                default: {
                    return null;
                }
            }
        }

        public string GetImagePath() {
            return "/Assets/Pieces/" + Colour.ToString() + "/" + Type.ToString() + ".png";
        }
    }
}
