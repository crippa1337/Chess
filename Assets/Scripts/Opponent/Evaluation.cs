using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    int infinity = int.MaxValue;
    int negInfinity = int.MinValue;

    [SerializeField] Board board;
    

    public int Evaluate(IDictionary<Vector2, PieceData> pieces)
    {
        int whiteScore = 0;
        int blackScore = 0;
        // White is maximizer
        // Black is minimizer

        Board.MateType whiteCheckmate = board.GenerateEndState(1);
        Board.MateType blackCheckmate = board.GenerateEndState(-1);

        // If white is checkmated, return min
        if (whiteCheckmate == Board.MateType.Checkmate)
        {
            return negInfinity;
        }
        // If black is checkmated, return max
        else if (blackCheckmate == Board.MateType.Checkmate)
        {
            return infinity;
        }
        // If anyone is stalemated, return 0
        else if (whiteCheckmate == Board.MateType.Stalemate || blackCheckmate == Board.MateType.Stalemate)
        {
            return 0;
        }

        for (int i = 0; i < pieces.Count; i++)
        {
            PieceData piece = pieces.ElementAt(i).Value;
            PieceData.Type type = piece.type;
            Vector2 pos = piece.position;

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
            if (piece.isWhite == -1)
            {
                piecePosValue = type switch
                {
                    PieceData.Type.Pawn => PieceSquareTables.pawns[(int)pos.y, (int)pos.x],
                    PieceData.Type.Knight => PieceSquareTables.knights[(int)pos.y, (int)pos.x],
                    PieceData.Type.Bishop => PieceSquareTables.bishops[(int)pos.y, (int)pos.x],
                    PieceData.Type.Rook => PieceSquareTables.rooks[(int)pos.y, (int)pos.x],
                    PieceData.Type.Queen => PieceSquareTables.queens[(int)pos.y, (int)pos.x],
                    PieceData.Type.King => PieceSquareTables.kings[(int)pos.y, (int)pos.x],
                    _ => 0
                };

                blackScore += pieceValue + piecePosValue;
            }

            else if (piece.isWhite == 1)
            {
                piecePosValue = type switch
                {
                    PieceData.Type.Pawn => PieceSquareTables.pawns[7 - (int)pos.y, (int)pos.x],
                    PieceData.Type.Knight => PieceSquareTables.knights[7 - (int)pos.y, (int)pos.x],
                    PieceData.Type.Bishop => PieceSquareTables.bishops[7 - (int)pos.y, (int)pos.x],
                    PieceData.Type.Rook => PieceSquareTables.rooks[7 - (int)pos.y, (int)pos.x],
                    PieceData.Type.Queen => PieceSquareTables.queens[7 - (int)pos.y, (int)pos.x],
                    PieceData.Type.King => PieceSquareTables.kings[7 - (int)pos.y, (int)pos.x],
                    _ => 0
                };

                whiteScore += pieceValue + piecePosValue;
            }
            
        }
          
        return whiteScore - blackScore;
    }
}
