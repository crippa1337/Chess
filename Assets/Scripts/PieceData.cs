using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData
{
    public int isWhite;
    public Vector2 position;
    public Type type;
    public GameObject gameObject;

    public bool hasMoved = false;
    public bool hasCastled = false;

    public enum Type
    { 
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    // Copy piece
    public PieceData(int isWhite, Vector2 position, Type type, bool hasMoved, bool hasCastled)
    {
        this.isWhite = isWhite;
        this.position = position;
        this.type = type;
        this.hasMoved = hasMoved;
        this.hasCastled = hasCastled;
    }

    // General piece
    public PieceData(GameObject gameObject, int isWhite, Vector2 position, Type type)
    {
        this.gameObject = gameObject;
        this.isWhite = isWhite;
        this.position = position;
        this.type = type;
    }

    // Pawn & rook
    public PieceData(GameObject gameObject, int isWhite, Vector2 position, Type type, bool hasMoved)
    {
        this.gameObject = gameObject;
        this.isWhite = isWhite;
        this.position = position;
        this.type = type;
        this.hasMoved = hasMoved;
    }

    // King
    public PieceData(GameObject gameObject, int isWhite, Vector2 position, Type type, bool hasMoved, bool hasCastled)
    {
        this.gameObject = gameObject;
        this.isWhite = isWhite;
        this.position = position;
        this.type = type;
        this.hasMoved = hasMoved;
        this.hasCastled = hasCastled;
    }

    public PieceData DeepCopy()
    {
        PieceData copy = new PieceData(isWhite, position, type, hasMoved, hasCastled);
        return copy;
    }

    public List<Vector2> LegalMoves(IDictionary<Vector2, PieceData> pieces)
    {
        switch (type) {
            case Type.Pawn:
                return PawnMoves(pieces);
            case Type.Knight:
                return KnightMoves(pieces);
            case Type.Bishop:
                return DiagonalMoves(pieces);
            case Type.Rook:
                return StraightMoves(pieces);
            case Type.Queen:
                return QueenMoves(pieces);
            case Type.King:
                return KingMoves(pieces);
            default:
                return new List<Vector2>();
        }
    }

    public List<Vector2> StraightMoves(IDictionary<Vector2, PieceData> pieces)
    {
        List<Vector2> legalMoves = new();
        bool[] axes = { true, true, true, true };
        int distance = 0;

        while (true)
        {
            distance++;

            for (int i = 0; i < 4; i++)
            {
                bool axis = axes[i];
                if (!axis)
                {
                    continue;
                }

                Vector2 newMove;

                if (i % 2 == 0)
                {
                    newMove = new Vector2(position.x + distance * (i - 1), position.y);
                }
                else
                {
                    newMove = new Vector2(position.x, position.y + distance * (i - 2));
                }

                if (newMove.x < 0 || newMove.x > 7 || newMove.y < 0 || newMove.y > 7)
                {
                    axes[i] = false;
                    continue;
                }

                PieceData currentPiece = pieces[new Vector2(newMove.x, newMove.y)];

                if (currentPiece == null)
                {
                    legalMoves.Add(newMove);
                }
                else if (currentPiece.isWhite != isWhite)
                {
                    legalMoves.Add(newMove);
                    axes[i] = false;
                }
                else
                {
                    axes[i] = false;
                }
            }

            // Stop if all axes are false
            if (!axes[0] && !axes[1] && !axes[2] && !axes[3])
            {
                break;
            }
        }

        return legalMoves;
    }

    public List<Vector2> DiagonalMoves(IDictionary<Vector2, PieceData> pieces)
    {
        List<Vector2> legalMoves = new();
        bool[] axes = { true, true, true, true };
        int distance = 0;

        while (true)
        {
            distance++;

            for (int i = 0; i < 4; i++)
            {
                bool axis = axes[i];
                if (!axis)
                {
                    continue;
                }

                Vector2 newMove;

                if (i == 0)
                {
                    newMove = new Vector2(position.x + distance, position.y + distance);
                }
                else if (i == 1)
                {
                    newMove = new Vector2(position.x - distance, position.y + distance);
                }
                else if (i == 2)
                {
                    newMove = new Vector2(position.x + distance, position.y - distance);
                }
                else
                {
                    newMove = new Vector2(position.x - distance, position.y - distance);
                }

                if (newMove.x < 0 || newMove.x > 7 || newMove.y < 0 || newMove.y > 7)
                {
                    axes[i] = false;
                    continue;
                }

                PieceData currentPiece = pieces[new Vector2(newMove.x, newMove.y)];

                if (currentPiece == null)
                {
                    legalMoves.Add(newMove);
                }
                else if (currentPiece.isWhite != isWhite)
                {
                    legalMoves.Add(newMove);
                    axes[i] = false;
                }
                else
                {
                    axes[i] = false;
                }
            }

            // Stop if all axes are false
            if (!axes[0] && !axes[1] && !axes[2] && !axes[3])
            {
                break;
            }
        }

        return legalMoves;
    }

    public List<Vector2> RemoveIllegalMoves(List<Vector2> potentialMoves, IDictionary<Vector2, PieceData> pieces)
    {

        List<Vector2> legalMoves = new();

        foreach (Vector2 move in potentialMoves)
        {
            if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7)
            {
                continue;
            }

            if (pieces[new Vector2(move.x, move.y)] != null && pieces[new Vector2(move.x, move.y)].isWhite == isWhite)
            {
                continue;
            }

            legalMoves.Add(move);
        }

        return legalMoves;
    }

    public List<Vector2> PawnMoves(IDictionary<Vector2, PieceData> pieces)
    {
        List<Vector2> moveMoves = new();
        List<Vector2> attackMoves = new();
        List<Vector2> legalMoves = new();

        if (!hasMoved)
        {
            if (pieces[new Vector2(position.x, position.y + (1 * isWhite))] == null)
            {
                moveMoves.Add(new(position.x, position.y + (2 * isWhite)));
            }
        }
        moveMoves.Add(new Vector2(position.x, position.y + (1 * isWhite)));

        foreach (Vector2 move in moveMoves)
        {
            if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7)
            {
                continue;
            }

            if (pieces[new Vector2(move.x, move.y)] == null)
            {
                legalMoves.Add(move);
            }
        }

        attackMoves.Add(new Vector2(position.x + 1, position.y + (1 * isWhite)));
        attackMoves.Add(new Vector2(position.x - 1, position.y + (1 * isWhite)));
        foreach (Vector2 move in attackMoves)
        {
            // See if the move is out of bounds
            if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7)
            {
                continue;
            }

            if (pieces[(int)move.x, (int)move.y] != null && pieces[(int)move.x, (int)move.y].isWhite != isWhite)
            {
                legalMoves.Add(move);
            }
        }

        return legalMoves;

    }

    public List<Vector2> KnightMoves(PieceData[,] pieces)
    {
        List<Vector2> potentialMoves = new List<Vector2>();

        potentialMoves.Add(new Vector2(position.x + 1, position.y + 2));
        potentialMoves.Add(new Vector2(position.x + 2, position.y + 1));
        potentialMoves.Add(new Vector2(position.x + 2, position.y - 1));
        potentialMoves.Add(new Vector2(position.x + 1, position.y - 2));
        potentialMoves.Add(new Vector2(position.x - 1, position.y - 2));
        potentialMoves.Add(new Vector2(position.x - 2, position.y - 1));
        potentialMoves.Add(new Vector2(position.x - 2, position.y + 1));
        potentialMoves.Add(new Vector2(position.x - 1, position.y + 2));


        return RemoveIllegalMoves(potentialMoves, pieces);
    }

    public List<Vector2> QueenMoves(PieceData[,] pieces)
    {
        List<Vector2> legalMoves = new();

        legalMoves.AddRange(StraightMoves(pieces));
        legalMoves.AddRange(DiagonalMoves(pieces));

        return legalMoves;
    }

    public List<Vector2> KingMoves(PieceData[,] pieces)
    {
        List<Vector2> potentialMoves = new();

        potentialMoves.Add(new Vector2(position.x + 1, position.y));
        potentialMoves.Add(new Vector2(position.x + 1, position.y + 1));
        potentialMoves.Add(new Vector2(position.x, position.y + 1));
        potentialMoves.Add(new Vector2(position.x - 1, position.y + 1));
        potentialMoves.Add(new Vector2(position.x - 1, position.y));
        potentialMoves.Add(new Vector2(position.x - 1, position.y - 1));
        potentialMoves.Add(new Vector2(position.x, position.y - 1));
        potentialMoves.Add(new Vector2(position.x + 1, position.y - 1));

        if (!hasMoved && MouseManager.canCastle)
        {
            PieceData rook1 = pieces[(int)position.x + 3, (int)position.y];
            PieceData rook2 = pieces[(int)position.x - 4, (int)position.y];

            if (rook1 != null) {
                // If the king hasn't moved and the rook on the right hasn't moved, and there are no pieces between the king and the rook, add the move to the list of legal moves
                if (rook1.type == Type.Rook && !rook1.hasMoved && pieces[(int)position.x + 1, (int)position.y] == null && pieces[(int)position.x + 2, (int)position.y] == null)
                {
                    potentialMoves.Add(new Vector2(position.x + 2, position.y));
                }
            }

            if (rook2 != null) {
                // If the king hasn't moved and the rook on the left hasn't moved, and there are no pieces between the king and the rook, add the move to the list of legal moves
                if (rook2.type == Type.Rook && !rook2.hasMoved && pieces[(int)position.x - 1, (int)position.y] == null && pieces[(int)position.x - 2, (int)position.y] == null && pieces[(int)position.x - 3, (int)position.y] == null)
                {
                    potentialMoves.Add(new Vector2(position.x - 2, position.y));
                }
            }
        }

        return RemoveIllegalMoves(potentialMoves, pieces);
    }
}
