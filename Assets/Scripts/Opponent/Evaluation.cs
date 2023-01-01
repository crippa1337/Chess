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

                    int pieceValue;
                    switch (type)
                    {
                        case PieceData.Type.Pawn:
                            pieceValue = 100;
                            break;
                        case PieceData.Type.Knight:
                            pieceValue = 320;
                            break;
                        case PieceData.Type.Bishop:
                            pieceValue = 330;
                            break;
                        case PieceData.Type.Rook:
                            pieceValue = 500;
                            break;
                        case PieceData.Type.Queen:
                            pieceValue = 900;
                            break;
                        case PieceData.Type.King:
                            pieceValue = 20000;
                            break;
                        default:
                            pieceValue = 0;
                            break;
                    }
              

                    if (piece.isWhite == 1)
                    {
                        whiteScore += pieceValue;
                    }
                    else if (piece.isWhite == -1)
                    {
                        blackScore += pieceValue;
                    }
                }
            }
        }

        return whiteScore - blackScore;
    }
}
