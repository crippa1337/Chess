using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RNG
{
    static ulong seed = 1070372ul;

    public static ulong Next()
    {
        seed ^= seed >> 12;
        seed ^= seed << 25;
        seed ^= seed >> 27;

        return seed * 2685821657736338717ul;
    }
}

public class Zobrist
{
    // 12 pieces, 2 colors, 64 squares
    ulong[] ZobristKeys = new ulong[768];

    // One for each file
    ulong[] ZobristEnPassantKeys = new ulong[8];
    
    // White Kingside, White Queenside, Black Kingside, Black Queenside
    ulong[] ZobristCastleKeys = new ulong[4];
    ulong[] ZobristSideToMoveKey = new ulong[1];

    public Zobrist()
    {
        for (int i = 0; i < 768; i++)
        {
            ZobristKeys[i] = RNG.Next();
        }

        for (int i = 0; i < 8; i++)
        {
            ZobristEnPassantKeys[i] = RNG.Next();
        }

        for (int i = 0; i < 4; i++)
        {
            ZobristCastleKeys[i] = RNG.Next();
        }

        ZobristSideToMoveKey[0] = RNG.Next();
    }

    public ulong Hash(BoardData board)
    {
        ulong key = 0;

        // Pieces - - - ZobristKeys[]
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board.pieces[i, j] != null)
                {
                    PieceData piece = board.pieces[i, j];
                    int pieceKind = piece.type switch
                    {
                        PieceData.Type.Pawn => piece.isWhite == 1 ? 1 : 0,
                        PieceData.Type.Knight => piece.isWhite == 1 ? 3: 2,
                        PieceData.Type.Bishop => piece.isWhite == 1 ? 5: 4,
                        PieceData.Type.Rook => piece.isWhite == 1 ? 7 : 6,
                        PieceData.Type.Queen => piece.isWhite == 1 ? 9: 8,
                        PieceData.Type.King => piece.isWhite == 1 ? 11 : 10,
                        _ => Helper.noneValue
                    };

                    int offset = 64 * pieceKind + 8 * i + j;

                    key ^= ZobristKeys[offset];
                };
            }
        }

        // En Passant - - - ZobristEnPassantKeys[]
        int epFile = (int)board.enPassant.x;


        return key;
    }
}
