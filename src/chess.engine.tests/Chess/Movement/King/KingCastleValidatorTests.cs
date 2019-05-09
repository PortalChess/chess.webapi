﻿using board.engine.Movement;
using chess.engine.Chess.Movement.ChessPieces.King;
using chess.engine.Extensions;
using chess.engine.tests.Builders;
using NUnit.Framework;

namespace chess.engine.tests.Chess.Movement.King
{
    [TestFixture]
    public class KingCastleValidatorTests : ValidatorTestsBase
    {
        private KingCastleValidator _validator;
        private readonly BoardMove _whiteInvalidKingCastle = new BoardMove("D1".ToBoardLocation(), "G1".ToBoardLocation(), (int)ChessMoveTypes.CastleKingSide);
        private readonly BoardMove _whiteInvalidQueenCastle = new BoardMove("D1".ToBoardLocation(), "B1".ToBoardLocation(), (int)ChessMoveTypes.CastleKingSide);
        private readonly BoardMove _whiteKingSideCastle = new BoardMove("E1".ToBoardLocation(), "G1".ToBoardLocation(), (int)ChessMoveTypes.CastleKingSide);
        private readonly BoardMove _whiteQueenSideCastle = new BoardMove("E1".ToBoardLocation(), "C1".ToBoardLocation(), (int)ChessMoveTypes.CastleKingSide);

        [SetUp]
        public void Setup()
        {
            _validator = new KingCastleValidator();
        }
        [Test]
        public void ValidateMove_fails_unless_king_is_in_starting_position()
        {
            var board = new ChessBoardBuilder()
                .Board("    k   " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "   K   R"
                );

            var boardState = new ChessGameBuilder().BuildGame(board.ToGameSetup()).BoardState;
            Assert.True(_validator.ValidateMove(_whiteInvalidKingCastle, boardState), "Invalid castle move allowed");
            Assert.True(_validator.ValidateMove(_whiteInvalidQueenCastle, boardState), "Invalid castle move allowed");
        }
        [Test]
        public void ValidateMove_fails_unless_rook_is_in_starting_position()
        {
            var board = new ChessBoardBuilder()
                .Board("    k   " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "R       " +
                       "    K R "
                );

            var boardState = new ChessGameBuilder().BuildGame(board.ToGameSetup()).BoardState;
            Assert.False(_validator.ValidateMove(_whiteQueenSideCastle, boardState), "Invalid queen side castle move allowed");
            Assert.False(_validator.ValidateMove(_whiteKingSideCastle, boardState), "Invalid king side castle move allowed");
        }
        [Test]
        public void ValidateMove_fails_if_no_clear_path()
        {
            var board = new ChessBoardBuilder()
                .Board("    k   " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "    K NR"
                );

            var boardState = new ChessGameBuilder().BuildGame(board.ToGameSetup()).BoardState;
            Assert.False(_validator.ValidateMove(_whiteKingSideCastle, boardState), "Invalid king side castle move allowed");
        }
        [Test]
        public void ValidateMove_fails_if_path_under_attack()
        {
            var board = new ChessBoardBuilder()
                .Board("    k   " +
                       "        " +
                       "        " +
                       "        " +
                       "     r  " +
                       "        " +
                       "        " +
                       "    K  R"
                );

            var boardState = new ChessGameBuilder().BuildGame(board.ToGameSetup()).BoardState;

            Assert.False(_validator.ValidateMove(_whiteKingSideCastle, boardState), "Invalid king side castle move allowed");
        }
    }
}