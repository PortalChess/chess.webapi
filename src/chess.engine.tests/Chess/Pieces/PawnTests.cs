﻿using chess.engine.Chess.Pieces;
using chess.engine.Game;
using NUnit.Framework;

namespace chess.engine.tests.Chess.Pieces
{
    [TestFixture]
    public class PawnTests
    {
        [TestCase(Colours.White, 2)]
        [TestCase(Colours.Black, 7)]
        public void Should_have_correct_pawn_starting_ranks(Colours player, int rank)
        {
            Assert.That(Pawn.StartRankFor(player), Is.EqualTo(rank));
        }

        [TestCase(Colours.White, 5)]
        [TestCase(Colours.Black, 4)]
        public void Should_have_correct_enpassant_ranks(Colours player, int rank)
        {
            Assert.That(Pawn.EnPassantRankFor(player), Is.EqualTo(rank));
        }
    }
}