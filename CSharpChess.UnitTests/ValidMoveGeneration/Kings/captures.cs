﻿using System.Linq;
using CSharpChess.Extensions;
using CSharpChess.MoveGeneration;
using CSharpChess.TheBoard;
using CSharpChess.UnitTests.Helpers;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace CSharpChess.UnitTests.ValidMoveGeneration.Kings
{
    [TestFixture]
    public class captures : BoardAssertions
    {
        [Test]
        public void can_take()
        {
            // TODO: Will need to split this up once checks are implemented.
            const string asOneChar = ".......k" +
                                     "........" +
                                     "........" +
                                     "...K...." +
                                     "..ppp..." +
                                     "........" +
                                     "........" +
                                     "........";

            var board = BoardBuilder.CustomBoard(asOneChar, Chess.Colours.White);
            var expectedTakes = BoardLocation.List("D4", "C4", "E4");

            var generator = new KingMoveGenerator();
            var chessMoves = generator.All(board, BoardLocation.At("D5")).Takes().ToList();

            AssertMovesContainsExpectedWithType(chessMoves, expectedTakes, MoveType.Take);
        }
    }
}