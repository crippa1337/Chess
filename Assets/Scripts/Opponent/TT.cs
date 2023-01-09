using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum Flag
{
    NONEBOUND,
    UPPERBOUND,
    LOWERBOUND,
    EXACTBOUND
}

public class TEntry
{
    public int key;
    public int depth;
    public Flag flag;
    public int score;
    public Move move;

    public TEntry()
    {
        key = 0;
        depth = 0;
        flag = Flag.NONEBOUND;
        score = Helper.noneValue;
        move = Helper.noneMove;
    }
}

public class TranspositionTable
{
    int ttSize;
    TEntry[] table;

    public TranspositionTable()
    {
        ttSize = (int)Mathf.Pow(2, 20);
        table = new TEntry[ttSize];

        for (int i = 0; i < ttSize; i++)
        {
            table[i] = new TEntry();
        }
    }

    public int FindIndex(int key)
    {
        return key % ttSize;
    }

    public TEntry FindEntry(int key)
    {
        int index = FindIndex(key);
        return table[index];
    }

    public void Store(int key, int depth, Flag flag, int score, Move move, int ply)
    {
        TEntry entry = FindEntry(key);
        
        // Replacements
        if (entry.key != key || entry.move != move)
        {
            entry.move = move;
        }

        if (entry.key != key || flag == Flag.EXACTBOUND || depth + 4 > entry.depth)
        {
            entry.depth = depth;
            entry.score = ScoreToTable(score, ply);
            entry.key = key;
            entry.flag = flag;
        }
    }

    public int ScoreToTable(int score, int plies)
    {
        if (score >= Helper.tbWinInMaxPly)
        {
            return score + plies;
        }

        else
        {
            if (score <= Helper.tbLossMaxInPly)
            {
                return score - plies;
            }

            else
            {
                return score;
            }
        }
    }

    public int ScoreFromTable(int score, int plies)
    {
        if (score >= Helper.tbWinInMaxPly)
        {
            return score - plies;
        }

        else
        {
            if (score <= Helper.tbLossMaxInPly)
            {
                return score + plies;
            }

            else
            {
                return score;
            }
        }
    }
}
