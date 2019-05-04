﻿using chess.engine.Board;
using chess.engine.Entities;
using chess.engine.Movement;

namespace chess.engine.Actions
{
    public class PawnPromotionAction : BoardAction
    {
        public PawnPromotionAction(IBoardActionFactory factory, IBoardState boardState) : base(factory, boardState)
        {
        }

        public override void Execute(ChessMove move)
        {
            if (BoardState.IsEmpty(move.From)) return;

            var piece = BoardState.GetItem(move.From).Item;
            var forPlayer = piece.Player;

            BoardState.Remove(move.From);

            if (!BoardState.IsEmpty(move.To))
            {
                BoardState.Remove(move.To);
            }
            BoardState.PlaceEntity(move.To, ChessPieceEntityFactory.Create(move.PromotionPiece, forPlayer));
        }
    }
}