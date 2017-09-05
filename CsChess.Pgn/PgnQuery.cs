using System;
using System.Linq;
using CSharpChess;
using CSharpChess.System;
using CSharpChess.System.Extensions;
using CSharpChess.TheBoard;
using static CSharpChess.Chess;


namespace CsChess.Pgn
{
    public class PgnQuery
    {
        public MoveType MoveType { get; private set; }
        private Colours _turn;
        public PieceNames PromotionPiece { get; private set; }
        public ChessPiece Piece { get; private set; }
        public ChessFile FromFile { get; private set; } = ChessFile.None;
        public int FromRank { get; private set; }

        public ChessFile ToFile { get; private set; } = ChessFile.None;
        public int ToRank { get; private set; }

        public bool QueryResolved => !Validations.InvalidRank(FromRank)
                                     && !Validations.InvalidFile(FromFile)
                                     && !Validations.InvalidRank(ToRank)
                                     && !Validations.InvalidFile(ToFile)
                                     || GameOver;

        public bool GameOver { get; private set; }
        public ChessGameResult GameResult { get; private set; }
        public string PgnText { get; private set; } = string.Empty;

        private ChessFile ParseFile(char file)
        {
            ChessFile test;
            if (Enum.TryParse(file.ToString().ToUpper(), out test))
            {
                return test;
            }

            throw new ArgumentOutOfRangeException(nameof(file), $"Invalid file: {file}");
        }

        private int ParseRank(char rank)
        {
            int test;
            if (!int.TryParse(rank.ToString().ToUpper(), out test))
            {
                throw new ArgumentOutOfRangeException(nameof(rank), "Invalid rank");
            }

            if (Validations.InvalidRank(test))
            {
                throw new ArgumentOutOfRangeException(nameof(rank), "Invalid rank");
            }

            return test;
        }

        public void WithToFile(char file) => ToFile = ParseFile(file);
        public void WithFromFile(char file) => FromFile = ParseFile(file);
        public void WithToRank(char rank) => ToRank = ParseRank(rank);
        public void WithFromRank(char rank) => FromRank = ParseRank(rank);
        public void WithColour(Colours turn) => _turn = turn;
        public void WithPiece(ChessPiece chessPiece) => Piece = chessPiece;
        public void WithMoveType(MoveType moveType) => MoveType = moveType;

        public void WithResult(string move)
        {
            ToFile = ChessFile.None;
            ToRank = 0;
            FromFile = ChessFile.None;
            FromRank = 0;
            Piece = ChessPiece.NullPiece;
            GameResult = PgnResult.Parse(move);
            GameOver = true;
        }

        public void ResolveQuery(ChessBoard chessBoard)
        {
            var bl = FindPieceThatCanMoveTo(chessBoard, _turn, Piece.Name, BoardLocation.At(ToFile, ToRank));

            FromFile = bl.File;
            FromRank = bl.Rank;
        }

        private BoardLocation FindPieceThatCanMoveTo(ChessBoard board, Colours turn, PieceNames pieceName, BoardLocation move)
        {
            var boardPiecesQuery = board.Pieces.Where(p => p.Piece.Is(turn, pieceName));

            if (FromFile != ChessFile.None)
            {
                boardPiecesQuery = boardPiecesQuery.Where(p => p.Location.File == FromFile);
            }

            if (FromRank != 0)
            {
                boardPiecesQuery = boardPiecesQuery.Where(p => p.Location.Rank == FromRank);
            }

            boardPiecesQuery = boardPiecesQuery.Where(p => p.PossibleMoves.ContainsMoveTo(move));

            var boardPieces = boardPiecesQuery.ToList();
            if (boardPieces.None())
            {
                throw new Exception($"No {turn} {pieceName} found that can move to {move}");
            }

            var piece = boardPieces.First();

            if (boardPieces.Count() > 1)
            {

                boardPieces = boardPieces.Where(p => p.PossibleMoves.Any(pm => pm.MoveType == MoveType)).ToList();

                if (boardPieces.Count() != 1)
                {
                    // Cover the case where two pieces can move to a square but one of these
                    // pieces is actually pinned on discovered check
                    boardPieces =
                        boardPieces.Where(bp => bp.PossibleMoves.Any(pm => !Validations.MovesLeaveOwnSideInCheck(board, pm))).ToList();
                }

                if (boardPieces.Count() != 1)
                {
                    throw new PgnException($"Ambigous move {move}; possibles pieces; {boardPieces.ToStrings()}");
                }

                piece = boardPieces.First();
            }

            if (piece == null)
            {
//                Console.WriteLine(board.ToAsciiBoard());
                throw new PgnException($"No {pieceName} that can {MoveType} to {move} found");
            }

            return piece.Location;
        }

        public override string ToString()
        {
            if (GameOver) return GameResult.ToString();
            return $"{_turn} {CreateMove()}";
        }

        private ChessMove CreateMove()
        {
            var from = new BoardLocation(FromFile, FromRank);
            var to = new BoardLocation(ToFile, ToRank);
            var move = new ChessMove(from, to, MoveType,PromotionPiece);
            return move;
        }

        public void WithPromotion(char promotionPiece)
        {
            PromotionPiece = GetPromotionPiece(promotionPiece.ToString());
        }
        private static Chess.PieceNames GetPromotionPiece(string piece)
        {
            switch (piece.ToUpper())
            {
                case "R": return Chess.PieceNames.Rook;
                case "B": return Chess.PieceNames.Bishop;
                case "N": return Chess.PieceNames.Knight;
                case "Q": return Chess.PieceNames.Queen;
            }

            throw new ArgumentException($"'{piece}' is not a valid promotion", nameof(piece));

        }
        public string ToMove()
        {
            return $"{CreateMove()}";
        }
    }

    internal class PgnException : Exception
    {
        public PgnException(string s)
        {
            
        }
    }
}