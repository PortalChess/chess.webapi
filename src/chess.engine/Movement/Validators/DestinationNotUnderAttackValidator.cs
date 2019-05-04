﻿using System.Linq;
using chess.engine.Board;
using chess.engine.Game;

namespace chess.engine.Movement.Validators
{
    public class DestinationNotUnderAttackValidator : IMoveValidator
    {
        public bool ValidateMove(BoardMove move, IBoardState boardState)
        {
            var piece = boardState.GetItem(move.From);
            var enemyColour = piece.Item.Owner.Enemy();
            var enemyLocations = boardState.GetItems(enemyColour).Select(i => i.Location);

            var enemyPaths = new Paths();
            var enemyItems = boardState.GetItems(enemyLocations.ToArray());
            enemyPaths.AddRange(enemyItems.SelectMany(li => li.Paths));

            return !enemyPaths.ContainsMoveTo(move.To);

        }
    }
}