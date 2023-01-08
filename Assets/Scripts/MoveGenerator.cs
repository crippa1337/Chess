using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    [SerializeField] Board gameBoard;

    // Legal Moves
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

    // Legal captures
    public List<(Vector2, List<Vector2>)> GenerateAllCaptures(BoardData board)
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

        List<(Vector2, List<Vector2>)> captureMoves = new List<(Vector2, List<Vector2>)>();
        foreach (var (position, moves) in allMoves)
        {
            List<Vector2> currentCaptures = new List<Vector2>();
            foreach (var move in moves)
            {
                if (board.pieces[(int)move.x, (int)move.y] != null)
                {
                    currentCaptures.Add(move);
                }
            }

            if (currentCaptures.Count != 0)
            {
                captureMoves.Add((position, currentCaptures));
            }
        }

        List<(Vector2, List<Vector2>)> legalMoves = new List<(Vector2, List<Vector2>)>();
        foreach (var (position, moves) in captureMoves)
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
