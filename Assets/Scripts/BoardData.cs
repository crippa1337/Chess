using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardData
{
    public PieceData[,] pieces;
    public Vector2 white_kingpos;
    public Vector2 black_kingpos;
    public Vector2 enPassant;
    // White Kingside, White Queenside, Black Kingside, Black Queenside
    public bool[] castling = new bool[4];
    public int fiftyMoveCounter;

    public BoardData(PieceData[,] pieces, Vector2 white_kingpos, Vector2 black_kingpos, Vector2 enPassant, bool[] castling, int fiftyMoveCounter)
    {
        this.pieces = pieces;
        this.white_kingpos = white_kingpos;
        this.black_kingpos = black_kingpos;
        this.enPassant = enPassant;
        this.castling = castling;
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

        bool[] castling = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            castling[i] = this.castling[i];
        }

        Vector2 white_kingpos = this.white_kingpos;
        Vector2 black_kingpos = this.black_kingpos;
        Vector2 enPassant = this.enPassant;
        int fiftyMoveCounter = this.fiftyMoveCounter;

        return new BoardData(pieces, white_kingpos, black_kingpos, enPassant, castling, fiftyMoveCounter);
    }
}
