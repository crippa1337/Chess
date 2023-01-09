
using UnityEngine;

public class Sorting : MonoBehaviour
{
    public static int MVVLVA(BoardData board, Move move)
    {
        int[,] mvvlva =
        {
            {0, 0, 0, 0, 0, 0, 0 },
            {0, 105, 104, 103, 102, 101, 100},
            {0, 205, 204, 203, 202, 201, 200},
            {0, 305, 304, 303, 302, 301, 300},
            {0, 405, 404, 403, 402, 401, 400},
            {0, 505, 504, 503, 502, 501, 500},
            {0, 605, 604, 603, 602, 601, 600},
        };

        PieceData victim = board.pieces[(int)move.to.x, (int)move.to.y];
        PieceData attacker = board.pieces[(int)move.from.x, (int)move.from.y];
        
        // En Passant
        int victimNum = victim == null ? 1 : PieceNumber(victim.type);
        int attackerNum = PieceNumber(attacker.type);

        return mvvlva[victimNum, attackerNum];
    }

    static int PieceNumber(PieceData.Type pieceType)
    {
        int pieceNum = pieceType switch
        {
            PieceData.Type.Pawn => 1,
            PieceData.Type.Knight => 2,
            PieceData.Type.Bishop => 3,
            PieceData.Type.Rook => 4,
            PieceData.Type.Queen => 5,
            PieceData.Type.King => 6,
            _ => 0,
        };

        return pieceNum;
    }
}
