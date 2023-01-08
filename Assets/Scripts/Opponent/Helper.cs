
public static class Helper
{
    public static int infinity = 999999;
    public static int negInfinity = -infinity;
    public static int noneValue = infinity + 1;
    public static int mateValue = infinity - 1;

    public static int mateIn(int ply)
    {
        return mateValue - ply;
    }

    public static int matedIn(int ply)
    {
        return ply - mateValue;
    }
}
