using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    [SerializeField] Board board;

    public int Evaluate(BoardData oldBoard, int caller)
    {
        int whiteScore = 0;
        int blackScore = 0;
        // White is maximizer
        // Black is minimizer

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (oldBoard.pieces[i, j] != null)
                {
                    PieceData piece = oldBoard.pieces[i, j];
                    PieceData.Type type = piece.type;

                    int piecePosValue;
                    if (piece.isWhite == -1)
                    {
                        piecePosValue = type switch
                        {
                            PieceData.Type.Pawn => PieceSquareTables.pawns[j, i],
                            PieceData.Type.Knight => PieceSquareTables.knights[j, i],
                            PieceData.Type.Bishop => PieceSquareTables.bishops[j, i],
                            PieceData.Type.Rook => PieceSquareTables.rooks[j, i],
                            PieceData.Type.Queen => PieceSquareTables.queens[j, i],
                            PieceData.Type.King => PieceSquareTables.kings[j, i],
                            _ => 0
                        };
                        blackScore += piece.value + piecePosValue;
                    }
                    
                    else if (piece.isWhite == 1)
                    {
                        piecePosValue = type switch
                        {
                            PieceData.Type.Pawn => PieceSquareTables.pawns[7 - j, i],
                            PieceData.Type.Knight => PieceSquareTables.knights[7 - j, i],
                            PieceData.Type.Bishop => PieceSquareTables.bishops[7 - j, i],
                            PieceData.Type.Rook => PieceSquareTables.rooks[7 - j, i],
                            PieceData.Type.Queen => PieceSquareTables.queens[7 - j, i],
                            PieceData.Type.King => PieceSquareTables.kings[7 - j, i],
                            _ => 0
                        };

                        whiteScore += piece.value + piecePosValue;
                    }
                }
            }
        }

        return (whiteScore - blackScore) * caller;
    }
}
