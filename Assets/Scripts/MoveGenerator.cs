using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    public List<(Vector2, List<Vector2>)> GenerateAllMoves(PieceData[,] pieces, int isWhite)
    {
        List<(Vector2, List<Vector2>)> allMoves = new List<(Vector2, List<Vector2>)>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhite)
                {
                    PieceData piece = pieces[i, j];
                    List<Vector2> moves = piece.LegalMoves(pieces);
                    
                    if (moves.Count != 0)
                    {
                        allMoves.Add((piece.position, moves));
                    }
                }
            }
        }
        return allMoves;
    }
}
