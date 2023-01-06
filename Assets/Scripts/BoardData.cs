using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardData
{
    public PieceData[,] pieces;
    public Vector2 white_kingpos;
    public Vector2 black_kingpos;
    public Vector2 enPassant;
    public int fiftyMoveCounter;

    public BoardData(PieceData[,] pieces, Vector2 white_kingpos, Vector2 black_kingpos, Vector2 enPassant, int fiftyMoveCounter)
    {
        this.pieces = pieces;
        this.white_kingpos = white_kingpos;
        this.black_kingpos = black_kingpos;
        this.enPassant = enPassant;
        this.fiftyMoveCounter = fiftyMoveCounter;
    }

    public BoardData DeepCopy()
    {
        PieceData[,] pieces = new PieceData[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (this.pieces[i, j] == null) continue;
                pieces[i, j] = this.pieces[i, j].DeepCopy();
            }
        }
        return new BoardData(pieces, white_kingpos, black_kingpos, enPassant, fiftyMoveCounter);
    }
}
