namespace Chess.Pieces {
    public enum PieceColour { White, Black }
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }

    public abstract class Piece {
        public PieceColour Colour { get; set; }
        public PieceType Type { get; set; }
        public int Value { get; }

        /// <summary>
        /// Main constructor class for the piece superclass
        /// </summary>
        /// <param name="colour">The colour of piece</param>
        /// <param name="type">The type of piece being created</param>
        /// <param name="value">The piece's value</param>
        public Piece(PieceColour colour, PieceType type, int value) {
            Colour = colour;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Overridable move function to determine all legal moves for a piece
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="row">The current row</param>
        /// <param name="col">the current column</param>
        /// <returns>
        /// Overridable List of legal moves (including check moves)
        /// </returns>
        public abstract List<Chess.Move> GetLegalMoves(Chess.Board board, int row, int col);

        /// <summary>
        /// Clones the current piece object by generating a new object of the corresponding subclass
        /// </summary>
        /// <returns>
        /// New (cloned) Piece object
        /// </returns>
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
