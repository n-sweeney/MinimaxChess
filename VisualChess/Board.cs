﻿using Chess.Pieces;

namespace Chess {
    public class Board {
        public const int BOARDSIZE = 8;
        public Piece?[,] Tiles { get; set; }

        public Board() {
            Tiles = new Piece[BOARDSIZE, BOARDSIZE];
        }

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

        public bool IsInsideBoard(int row, int col) {
            return row >= 0 && row < BOARDSIZE && col >= 0 && col < BOARDSIZE;
        }

        public void MakeMove(Move move) {
            Piece? piece = Tiles[move.FromRow, move.FromCol];

            if (piece == null) {
                return;
            }

            Tiles[move.ToRow, move.ToCol] = piece;
            Tiles[move.FromRow, move.FromCol] = null;
        }

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

        public int Evaluate(PieceColour colour) {
            int score = 0;

            for (int i = 0; i < BOARDSIZE; i++) {
                for (int j = 0; j < BOARDSIZE; j++) {
                    Piece? piece = Tiles[i, j];

                    if (piece != null) {
                        int pieceValue = GetPieceValue(piece.Type);

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

        public int GetPieceValue(PieceType type) {
            switch (type) {
                case PieceType.Pawn: {
                    return 10;
                }

                case PieceType.Knight: {
                    return 30;
                }

                case PieceType.Bishop: {
                    return 30;
                }

                case PieceType.Rook: {
                    return 50;
                }

                case PieceType.Queen: {
                    return 90;
                }

                case PieceType.King: {
                    return 900;
                }

                default: {
                    return 0;
                }
            }
        }
    }
}
