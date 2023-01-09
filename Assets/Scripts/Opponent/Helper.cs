using UnityEngine;

public static class Helper
{
    public static int infinity = 999999;
    public static int negInfinity = -infinity;
    public static int noneValue = infinity + 1;
    public static int mateValue = infinity - 1;
    public static Move noneMove = new Move(new Vector2(-1, -1), new Vector2(-1, -1));

    public static int maxDepth = 60;
    public static int mateInPly = mateValue - maxDepth;
    public static int matedInPly = -matedInPly;

    public static int tbWin = mateInPly;
    public static int tbLoss = -tbWin;
    public static int tbWinInMaxPly = tbWin - maxDepth;
    public static int tbLossMaxInPly = -tbWinInMaxPly;


    public static int MateIn(int ply)
    {
        return mateValue - ply;
    }

    public static int MatedIn(int ply)
    {
        return ply - mateValue;
    }
}
