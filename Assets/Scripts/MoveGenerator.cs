using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    [SerializeField] Board gameBoard;
    
    // Legal Moves
    public List<Move> GenerateLegalMoves(BoardData board)
    {
        List<Move> allMoves = new();
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
                        for (int k = 0; k < moves.Count; k++)
                        {
                            allMoves.Add(new Move(piece.position, moves[k]));
                        }
                    }
                }
            }
        }

        List<Move> legalMoves = new();
        for (int i = 0; i < allMoves.Count; i++)
        {
            BoardData newBoard = board.DeepCopy();
            if (gameBoard.TestMove(allMoves[i], newBoard))
            {
                legalMoves.Add(allMoves[i]);
            }
        }

        return legalMoves;
    }

    // Legal captures
    public List<Move> GenerateLegalCaptures(BoardData board)
    {
        List<Move> allMoves = new();
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
                        for (int k = 0; k < moves.Count; k++)
                        {
                            allMoves.Add(new Move(piece.position, moves[k]));
                        }
                    }
                }
            }
        }

        List<Move> captureMoves = new();
        for (int i = 0; i < allMoves.Count; i++)
        {
            BoardData newBoard = board.DeepCopy();
            if (newBoard.pieces[(int)allMoves[i].to.x, (int)allMoves[i].to.y] != null)
            {
                if (gameBoard.TestMove(allMoves[i], newBoard))
                {
                    captureMoves.Add(allMoves[i]);
                }
            }
        }

        return captureMoves;
    }
}
