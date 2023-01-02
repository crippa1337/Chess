using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    int infinity = int.MaxValue;
    int negInfinity = int.MinValue;

    [SerializeField] Board board;
    

    public int Evaluate(PieceData[,] pieces)
    {
        int whiteScore = 0;
        int blackScore = 0;
        // White is maximizer
        // Black is minimizer

        // If white is checkmated, return negInfinity
        if (board.CheckMates(1) == -1)
        {
            return negInfinity;
        }
        // If black is checkmated, return infinity
        else if (board.CheckMates(-1) == 1)
        {
            return infinity;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    PieceData piece = pieces[i, j];
                    PieceData.Type type = piece.type;

                    int pieceValue = type switch
                    {
                        PieceData.Type.Pawn => 100,
                        PieceData.Type.Knight => 320,
                        PieceData.Type.Bishop => 330,
                        PieceData.Type.Rook => 500,
                        PieceData.Type.Queen => 900,
                        PieceData.Type.King => 20000,
                        _ => 0
                    };

                    int piecePosValue;
                    if (piece.isWhite == 1)
                    {
                        piecePosValue = type switch
                        {
                            PieceData.Type.Pawn => PieceSquareTables.pawns[i, j],
                            PieceData.Type.Knight => PieceSquareTables.knights[i, j],
                            PieceData.Type.Bishop => PieceSquareTables.bishops[i, j],
                            PieceData.Type.Rook => PieceSquareTables.rooks[i, j],
                            PieceData.Type.Queen => PieceSquareTables.queens[i, j],
                            PieceData.Type.King => PieceSquareTables.kings[i, j],
                            _ => 0
                        };

                        whiteScore += pieceValue + piecePosValue;
                    }

                    else if (piece.isWhite == -1)
                    {
                        piecePosValue = type switch
                        {
                            PieceData.Type.Pawn => PieceSquareTables.pawns[7 - i, j],
                            PieceData.Type.Knight => PieceSquareTables.knights[7 - i, j],
                            PieceData.Type.Bishop => PieceSquareTables.bishops[7 - i, j],
                            PieceData.Type.Rook => PieceSquareTables.rooks[7 - i, j],
                            PieceData.Type.Queen => PieceSquareTables.queens[7 - i, j],
                            PieceData.Type.King => PieceSquareTables.kings[7 - i, j],
                            _ => 0
                        };

                        blackScore += pieceValue + piecePosValue;
                    }
                }
            }
        }

        return whiteScore - blackScore;
    }
}
