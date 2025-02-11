namespace Chess {
    public class Move {
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }

        public Move(int fromRow, int fromCol, int toRow, int toCol) {
            FromRow = fromRow;
            FromCol = fromCol;
            ToRow = toRow;
            ToCol = toCol;
        }

        public override string ToString() {
            char fromFile = (char)('a' + FromCol);
            char toFile = (char)('a' + ToCol);
            int fromRank = 8 - FromRow;
            int toRank = 8 - ToRow;

            return $"{fromFile}{fromRank} -> {toFile}{toRank}";
        }
    }
}
