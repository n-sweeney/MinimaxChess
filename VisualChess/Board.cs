using Chess.Pieces;

namespace Chess {
    public class Board {
        public const int BOARDSIZE = 8;
        public Piece?[,] Tiles { get; set; }
        public List<Piece> WhitePieces { get; set; }
        public List<Piece> BlackPieces { get; set; }

        public Board() {
            Tiles = new Piece[BOARDSIZE, BOARDSIZE];
            WhitePieces = new List<Piece>();
            BlackPieces = new List<Piece>();
        }

        /// <summary>
        /// Initialises the chess board and places the pieces in the correct places.
        /// </summary>
        public void Initialise() {
            // Clear board
            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Tiles[i, j] = null;
                }
            }

            // Computer Black Pieces
            Tiles[0, 0] = new Rook(PieceColour.Black);
            Tiles[0, 1] = new Knight(PieceColour.Black);
            Tiles[0, 2] = new Bishop(PieceColour.Black);
            Tiles[0, 3] = new Queen(PieceColour.Black);
            Tiles[0, 4] = new King(PieceColour.Black);
            Tiles[0, 5] = new Bishop(PieceColour.Black);
            Tiles[0, 6] = new Knight(PieceColour.Black);
            Tiles[0, 7] = new Rook(PieceColour.Black);

            for (int j = 0; j < BOARDSIZE; j++) {
                Tiles[1, j] = new Pawn(PieceColour.Black);
            }

            // Player White pieces
            Tiles[7, 0] = new Rook(PieceColour.White);
            Tiles[7, 1] = new Knight(PieceColour.White);
            Tiles[7, 2] = new Bishop(PieceColour.White);
            Tiles[7, 3] = new Queen(PieceColour.White);
            Tiles[7, 4] = new King(PieceColour.White);
            Tiles[7, 5] = new Bishop(PieceColour.White);
            Tiles[7, 6] = new Knight(PieceColour.White);
            Tiles[7, 7] = new Rook(PieceColour.White);

            for (int j = 0; j < BOARDSIZE; j++) {
                Tiles[6, j] = new Pawn(PieceColour.White);
            }
        }

        /// <summary>
        /// Helper function used to determine if (row, col) is a valid tile in the board.
        /// </summary>
        /// <param name="row">The row to check</param>
        /// <param name="col">The column to check</param>
        public static bool IsInsideBoard(int row, int col) {
            return row >= 0 && row < BOARDSIZE && col >= 0 && col < BOARDSIZE;
        }

        /// <summary>
        /// Completes a given move of a piece to a new tile. If a piece is taken it is added to the taken pieces.
        /// </summary>
        /// <param name="move">The move to complete on the board</param>
        public void MakeMove(Move move) {
            Piece? piece = Tiles[move.FromRow, move.FromCol];

            if (piece == null) {
                return;
            }

            Piece? takenPiece = Tiles[move.ToRow, move.ToCol];

            if (takenPiece != null) {
                if (takenPiece.Colour == PieceColour.White) {
                    AddPieceToList(WhitePieces, takenPiece);
                } else {
                    AddPieceToList(BlackPieces, takenPiece);
                }
            }

            Tiles[move.ToRow, move.ToCol] = piece;
            Tiles[move.FromRow, move.FromCol] = null;
        }

        /// <summary>
        /// Creates a clone of the current Tiles board and pieces implemented by this object.
        /// </summary>
        /// <returns>
        /// a new board object which is a copy of the current self.
        /// </returns>
        public Board Clone() {
            Board newBoard = new Board();
            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    if (Tiles[i, j] != null) {
                        newBoard.Tiles[i, j] = Tiles[i, j].Clone();
                    } else {
                        newBoard.Tiles[i, j] = null;
                    }
                }
            }

            return newBoard;
        }

        /// <summary>
        /// Generates all moves available for a given colour.
        /// </summary>
        /// <param name="colour">The PieceColour to generate moves for</param>
        /// <returns>
        /// List of all available moves (including illegal check moves)
        /// </returns>
        public List<Move> GenerateAllMoves(PieceColour colour) {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Piece? piece = Tiles[i, j];
                    if (piece != null && piece.Colour == colour) {
                        moves.AddRange(piece.GetLegalMoves(this, i, j));
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Refines moves generated from GenerateAllMoves to provide all legal moves - e.g. moves that do not result in check.
        /// </summary>
        /// <param name="colour">The PieceColour to generate moves for</param>
        /// <returns>
        /// List of all legal moves that do not result in check
        /// </returns>
        public List<Move> GetValidMoves(PieceColour colour) {
            List<Move> legalMoves = new List<Move>();
            List<Move> allMoves = GenerateAllMoves(colour);

            foreach (Move move in allMoves) {
                Board boardClone = Clone();
                boardClone.MakeMove(move);

                if (!boardClone.IsInCheck(colour)) {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        /// <summary>
        /// Check for a provided colour if their king is present or not.
        /// </summary>
        /// <param name="colour">The King's PieceColour</param>
        /// <returns>
        /// True, if king is alive, otherwise False.
        /// </returns>
        public bool IsKingAlive(PieceColour colour) {
            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Piece? piece = Tiles[i, j];
                    if (piece != null && piece.Type == PieceType.King && piece.Colour == colour) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluate the current board's score for a given colour.
        /// </summary>
        /// <param name="colour">PieceColour to evaluate</param>
        /// <returns>
        /// The current board's total piece score
        /// </returns>
        public int Evaluate(PieceColour colour) {
            int score = 0;

            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Piece? piece = Tiles[i, j];

                    if (piece != null) {
                        int pieceValue = piece.Value;

                        if (piece.Colour == colour) {
                            score += pieceValue;
                        } else {
                            score -= pieceValue;
                        }
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Adds piece to a list of pieces in value order.
        /// </summary>
        /// <param name="pieces">The list to append piece to</param>
        /// <param name="piece">The piece to append to the list.</param>
        /// <returns>
        /// The altered list pieces with piece inserted.
        /// </returns>
        private List<Piece> AddPieceToList(List<Piece> pieces, Piece piece) {
            List<Piece> newOrder = new List<Piece>();

            for (int i = 0; i < pieces.Count; i++) {
                if (piece.Value >= pieces[i].Value) {
                    pieces.Insert(i, piece);
                    return pieces;
                }
            }

            pieces.Add(piece);

            return pieces;
        }

        /// <summary>
        /// Determines if a provided colour is in check on the current board. This is when the king can be taken in a legal move.
        /// </summary>
        /// <param name="colour">The colour to determine if in check.</param>
        /// <returns>
        /// True, if colour is in check, otherwise False.
        /// </returns>
        public bool IsInCheck(PieceColour colour) {
            int kingRow = -1, kingCol = -1;
            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Piece? piece = Tiles[i, j];

                    if (piece != null && piece.Type == PieceType.King && piece.Colour == colour) {
                        kingRow = i;
                        kingCol = j;
                        break;
                    }
                }
            }

            if (kingRow == -1 || kingCol == -1) {
                return true;
            }

            PieceColour opponent = Game.Opponent(colour);
            List<Move> opponentMoves = GenerateAllMoves(opponent);

            foreach (Move move in opponentMoves) {
                if (move.ToRow == kingRow && move.ToCol == kingCol)
                    return true;
            }

            return false;
        }

    }
}
