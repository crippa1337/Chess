using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    public static List<(Vector2, List<Vector2>)> GenerateAllMoves(BoardData board, int isWhite)
    {
        List<(Vector2, List<Vector2>)> allMoves = new List<(Vector2, List<Vector2>)>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceData piece = board.pieces[i, j];
                if (piece != null && piece.isWhite == isWhite)
                {
                    List<Vector2> moves = piece.LegalMoves(board);
                    
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
