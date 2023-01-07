using System.Collections.Generic;
using UnityEngine;

public enum Flag
{
    NONEBOUND,
    UPPERBOUND,
    LOWERBOUND,
    EXACTBOUND
}

public class TEntry
{
    int VALUE_NONE = 999999 + 1;

    public int key;
    public int depth;
    public Flag flag;
    public int score;
    public (Vector2, Vector2) move;

    public TEntry()
    {
        key = 0;
        depth = 0;
        flag = Flag.NONEBOUND;
        score = VALUE_NONE;
        move = (new Vector2(-1, -1), new Vector2(-1, -1));
    }
}
