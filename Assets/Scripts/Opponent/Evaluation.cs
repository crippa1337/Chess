using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    readonly int posInfinity = int.MaxValue;
    readonly int negInfinity = int.MinValue;

    [SerializeField] Board board;

    public int Evaluate(BoardData oldBoard, int caller)
    {
        int whiteScore = 0;
        int blackScore = 0;
        // White is maximizer
        // Black is minimizer

        Board.MateType endState = board.GenerateEndState(oldBoard, caller);
        if (endState == Board.MateType.Checkmate)
        {
            return caller == 1 ? negInfinity : posInfinity;
        }
        else if (endState == Board.MateType.Stalemate)
        {
            return 0;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (oldBoard.pieces[i, j] != null)
                {
                    PieceData piece = oldBoard.pieces[i, j];
                    PieceData.Type type = piece.type;

                    int pieceValue = type switch
                    {
                        PieceData.Type.Pawn => 100,
                        PieceData.Type.Knight => 320,
                        PieceData.Type.Bishop => 330,
                        PieceData.Type.Rook => 500,
                        PieceData.Type.Queen => 900,
                        _ => 0
                    };

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
                        blackScore += pieceValue + piecePosValue;
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

                        whiteScore += pieceValue + piecePosValue;
                    }
                }
            }
        }

        return (whiteScore - blackScore) * caller;
    }
}
