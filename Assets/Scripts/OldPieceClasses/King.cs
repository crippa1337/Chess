using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : RestrictedPiece
{

    //public bool hasCastled = false;
    
    //public override List<Vector2> LegalMoves(Piece[,] pieces)
    //{
    //    List<Vector2> potentialMoves = new();
        
    //    potentialMoves.Add(new Vector2(position.x + 1, position.y));
    //    potentialMoves.Add(new Vector2(position.x + 1, position.y + 1));
    //    potentialMoves.Add(new Vector2(position.x, position.y + 1));
    //    potentialMoves.Add(new Vector2(position.x - 1, position.y + 1));
    //    potentialMoves.Add(new Vector2(position.x - 1, position.y));
    //    potentialMoves.Add(new Vector2(position.x - 1, position.y - 1));
    //    potentialMoves.Add(new Vector2(position.x, position.y - 1));
    //    potentialMoves.Add(new Vector2(position.x + 1, position.y - 1));

    //    if (!hasMoved && MouseManager.canCastle)
    //    {
    //        // If the king hasn't moved and the rook on the right hasn't moved, and there are no pieces between the king and the rook, add the move to the list of legal moves
    //        if (pieces[(int)position.x + 3, (int)position.y] is Rook rook && !rook.hasMoved && pieces[(int)position.x + 1, (int)position.y] == null && pieces[(int)position.x + 2, (int)position.y] == null)
    //        {
    //            potentialMoves.Add(new Vector2(position.x + 2, position.y));
    //        }

    //        // If the king hasn't moved and the rook on the left hasn't moved, and there are no pieces between the king and the rook, add the move to the list of legal moves
    //        if (pieces[(int)position.x - 4, (int)position.y] is Rook rook2 && !rook2.hasMoved && pieces[(int)position.x - 1, (int)position.y] == null && pieces[(int)position.x - 2, (int)position.y] == null && pieces[(int)position.x - 3, (int)position.y] == null)
    //        {
    //            potentialMoves.Add(new Vector2(position.x - 2, position.y));
    //        }
    //    }

    //    return RemoveIllegalMoves(potentialMoves, pieces);
    //}
}
