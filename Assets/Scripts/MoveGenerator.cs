using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    [SerializeField] Board gameBoard;

    public List<(Vector2, List<Vector2>)> GenerateAllMoves(BoardData board)
    {
        List<(Vector2, List<Vector2>)> allMoves = new List<(Vector2, List<Vector2>)>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceData piece = board.pieces[i, j];
                if (piece != null && piece.isWhite == board.sideToMove)
                {
                    List<Vector2> moves = piece.LegalMoves(board);
                    
                    if (moves.Count != 0)
                    {
                        allMoves.Add((piece.position, moves));
                    }
                }
            }
        }

        List<(Vector2, List<Vector2>)> legalMoves = new List<(Vector2, List<Vector2>)>();
        foreach (var (position, moves) in allMoves)
        {
            List<Vector2> newMoves = new List<Vector2>();
            foreach (var move in moves)
            {
                BoardData newBoard = board.DeepCopy();
                if (gameBoard.TestMove(position, move, newBoard))
                {
                    newMoves.Add(move);
                }
            }

            if (newMoves.Count != 0)
            {
                legalMoves.Add((position, newMoves));
            }
        }

        return legalMoves;
    }
}
