﻿namespace Chess.Pieces {
    public class Bishop : Piece {
        public Bishop(PieceColour colour) : base(colour, PieceType.Bishop, 30) {
        }

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

                    if (!board.IsInsideBoard(newRow, newCol)) {
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
