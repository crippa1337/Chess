using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    public static List<(Vector2, List<Vector2>)> GenerateAllMoves(int caller)
    {
        List<(Vector2, List<Vector2>)> allMoves = new List<(Vector2, List<Vector2>)>();
        IDictionary<Vector2, PieceData> pieces = caller == 1 ? Board.whitePieces : Board.blackPieces;

        for (int i = 0; i < pieces.Count; i++)
        {
            PieceData piece = pieces.ElementAt(i).Value;
            List<Vector2> moves = piece.LegalMoves(Board.pieceDict);

            if (moves.Count > 0)
            {
                allMoves.Add((piece.position, moves));
            }
        }
        
        return allMoves;
    }
}
