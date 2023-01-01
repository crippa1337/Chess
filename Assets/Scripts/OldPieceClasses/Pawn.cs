using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : RestrictedPiece
{

    //public override List<Vector2> LegalMoves(Piece[,] pieces)
    //{
    //    List<Vector2> moveMoves = new();
    //    List<Vector2> attackMoves = new();
    //    List<Vector2> legalMoves = new();

    //    if (!hasMoved)
    //    {
    //        if (pieces[(int)position.x, (int)position.y + (1 * isWhite)] == null)
    //        {
    //            moveMoves.Add(new(position.x, position.y + (2 * isWhite)));
    //        }
    //    }
    //    moveMoves.Add(new Vector2(position.x, position.y + (1 * isWhite)));
        
    //    foreach (Vector2 move in moveMoves)
    //    {
    //        if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7)
    //        {
    //            continue;
    //        }

    //        if (pieces[(int)move.x, (int)move.y] == null)
    //        {
    //            legalMoves.Add(move);
    //        }
    //    }

    //    attackMoves.Add(new Vector2(position.x + 1, position.y + (1 * isWhite)));
    //    attackMoves.Add(new Vector2(position.x - 1, position.y + (1 * isWhite)));
    //    foreach (Vector2 move in attackMoves)
    //    {
    //        // See if the move is out of bounds
    //        if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7)
    //        {
    //            continue;
    //        }

    //        if (pieces[(int)move.x, (int)move.y] != null && pieces[(int)move.x, (int)move.y].isWhite != isWhite)
    //        {
    //            legalMoves.Add(move);
    //        }
    //    }

    //    return legalMoves;

    //}

}
