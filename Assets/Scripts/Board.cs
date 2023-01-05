using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    //public static PieceData[,] pieces = new PieceData[8, 8];
    public static IDictionary<Vector2, PieceData> pieceDict = new Dictionary<Vector2, PieceData>();
    public static IDictionary<Vector2, PieceData> whitePieces = new Dictionary<Vector2, PieceData>();
    public static IDictionary<Vector2, PieceData> blackPieces = new Dictionary<Vector2, PieceData>();
    
    public GameObject[,] tiles = new GameObject[8, 8];
    public static Vector2 white_kingpos = new Vector2(4, 0);
    public static Vector2 black_kingpos = new Vector2(4, 7);

    [SerializeField] GameObject Tile;
    float TileScale;
    [SerializeField] Color FirstColor;
    [SerializeField] Color SecondColor;

    [Space(20)]
    [Header("Pieces")]
    [Header("White")]
    [SerializeField] GameObject WhiteKing;
    [SerializeField] GameObject WhiteQueen;
    [SerializeField] GameObject WhiteRook;
    [SerializeField] GameObject WhiteBishop;
    [SerializeField] GameObject WhiteKnight;
    [SerializeField] GameObject WhitePawn;
    
    [Space(3)]
    [Header("Black")]
    [SerializeField] GameObject BlackKing;
    [SerializeField] GameObject BlackQueen;
    [SerializeField] GameObject BlackRook;
    [SerializeField] GameObject BlackBishop;
    [SerializeField] GameObject BlackKnight;
    [SerializeField] GameObject BlackPawn;

    [Space(20)]
    [Header("Other")]
    AudioSource audioSource;
    [SerializeField] AudioClip captureSound;
    [SerializeField] AudioClip moveSound;


    // Start is called before the first frame update
    void Start()
    {
        InitBoard();
        InitStartingPositions();
        audioSource = GetComponent<AudioSource>();
    }

    public bool MovePiece(Vector2 from, Vector2 to, int isWhite, PieceData[,] pieces)
    {
        PieceData[,] testPieces = DeepCopyAllPieces(pieces);
        if (!TestMove(from, to, isWhite, testPieces)) return false;
        PieceData fromPiece = pieces[(int)from.x, (int)from.y];

        if (fromPiece.type == PieceData.Type.King || fromPiece.type == PieceData.Type.Rook || fromPiece.type == PieceData.Type.Pawn)
        {
            fromPiece.hasMoved = true;

            // Update king positions
            if (fromPiece.type == PieceData.Type.King)
            {
                if (fromPiece.isWhite == 1)
                {
                    white_kingpos = to;
                }
                else if (fromPiece.isWhite == -1)
                {
                    black_kingpos = to;
                }

                if (Math.Abs(from.x - to.x) == 2)
                {
                    fromPiece.hasCastled = true;
                    // Castling
                    if (to.x == 6)
                    {
                        // Kingside
                        // Move Rook
                        pieces[5, (int)from.y] = pieces[7, (int)from.y];
                        pieces[5, (int)from.y].position = new Vector2(5, from.y);
                        pieces[7, (int)from.y] = null;
                        pieces[5, (int)from.y].hasMoved = true;
                    }
                    else if (to.x == 2)
                    {
                        // Queenside
                        // Move Rook
                        pieces[3, (int)from.y] = pieces[0, (int)from.y];
                        pieces[3, (int)from.y].position = new Vector2(3, from.y);
                        pieces[0, (int)from.y] = null;
                        pieces[3, (int)from.y].hasMoved = true;
                    }
                }
            }
        }

        if (pieces[(int)to.x, (int)to.y] != null)
        {
            StartCoroutine(TakeCoroutine(pieces[(int)to.x, (int)to.y].gameObject));
            audioSource.PlayOneShot(captureSound);
        } else
        {
            audioSource.PlayOneShot(moveSound);
        }

        // move piece
        pieces[(int)to.x, (int)to.y] = fromPiece;
        // update piece pos
        pieces[(int)to.x, (int)to.y].position = to;
        // delete old pos piece
        pieces[(int)from.x, (int)from.y] = null;

        return true;
    }

    public bool TestMove(Vector2 from, Vector2 to, int isWhite, PieceData[,] testPieces)
    {
        PieceData fromPiece = testPieces[(int)from.x, (int)from.y];

        if (fromPiece == null) return false;
        if (fromPiece.isWhite != isWhite) return false;
        if (!fromPiece.LegalMoves(testPieces).Contains(to)) return false;

        if (fromPiece.type == PieceData.Type.King || fromPiece.type == PieceData.Type.Rook || fromPiece.type == PieceData.Type.Pawn)
        {
            fromPiece.hasMoved = true;

            // Update king positions
            if (fromPiece.type == PieceData.Type.King)
            {
                if (fromPiece.isWhite == 1)
                {
                    white_kingpos = to;
                }
                else if (fromPiece.isWhite == -1)
                {
                    black_kingpos = to;
                }

                if (Math.Abs(from.x - to.x) == 2)
                {
                    // Castling

                    if (to.x == 6)
                    {
                        // Kingside
                        // Move Rook
                        testPieces[5, (int)from.y] = testPieces[7, (int)from.y];
                        testPieces[5, (int)from.y].position = new Vector2(5, from.y);
                        testPieces[7, (int)from.y] = null;
                        testPieces[5, (int)from.y].hasMoved = true;
                    }
                    else if (to.x == 2)
                    {
                        // Queenside
                        // Move Rook
     
                        testPieces[3, (int)from.y] = testPieces[0, (int)from.y];
                        testPieces[3, (int)from.y].position = new Vector2(3, from.y);
                        testPieces[0, (int)from.y] = null;
                        testPieces[3, (int)from.y].hasMoved = true;
                    }
                }

            }
        }

        // move piece
        testPieces[(int)to.x, (int)to.y] = fromPiece;
        // update piece pos
        testPieces[(int)to.x, (int)to.y].position = to;
        // delete old pos piece
        testPieces[(int)from.x, (int)from.y] = null;

        if (isWhite == 1 && CheckChecks(testPieces, 1))
        {
            UndoKingTest(fromPiece, from);
            return false;
        }
        else if (isWhite == -1 && CheckChecks(testPieces, -1))
        {
            UndoKingTest(fromPiece, from);
            return false;
        }

        UndoKingTest(fromPiece, from);
        return true;
    }

    void UndoKingTest(PieceData fromPiece, Vector2 from)
    {
        if (fromPiece.type == PieceData.Type.King && fromPiece.isWhite == 1)
        {
            white_kingpos = from;
        }
        else if (fromPiece.type == PieceData.Type.King && fromPiece.isWhite == -1)
        {
            black_kingpos = from;
        }
    }

    void InitBoard()
    {
        TileScale = Tile.transform.localScale.x;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tiles[i, j] = Instantiate(Tile, transform.position + new Vector3((i - 3.5f) * TileScale, (j - 3.5f) * TileScale), Quaternion.identity, transform);
                
                tiles[i, j].GetComponent<Tile>().position = new Vector2(i, j);

                tiles[i, j].name = "Tile " + i + " " + j;

                if ((i + j) % 2 == 0)
                {
                    tiles[i, j].GetComponent<SpriteRenderer>().color = SecondColor;
                }
                else
                {
                    tiles[i, j].GetComponent<SpriteRenderer>().color = FirstColor;
                }

            }
        }
    }

    public void InitStartingPositions()
    {
        // Destroy and clear all pieces
        foreach (PieceData piece in pieceDict.Values)
        {
            Destroy(piece.gameObject);
        }

        pieceDict.Clear();

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            //pieces[i, 1] = Instantiate(WhitePawn, tiles[i, 1].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
            pieceDict.Add(new Vector2(i, 1), Instantiate(WhitePawn, tiles[i, 1].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
            //pieces[i, 6] = Instantiate(BlackPawn, tiles[i, 6].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
            pieceDict.Add(new Vector2(i, 6), Instantiate(BlackPawn, tiles[i, 6].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
        }

        // Kings
       //[4, 0] = Instantiate(WhiteKing, tiles[4, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(4, 0), Instantiate(WhiteKing, tiles[4, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[4, 7] = Instantiate(BlackKing, tiles[4, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(4, 7), Instantiate(BlackKing, tiles[4, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);

        // Queens
       //[3, 0] = Instantiate(WhiteQueen, tiles[3, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(3, 0), Instantiate(WhiteQueen, tiles[3, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[3, 7] = Instantiate(BlackQueen, tiles[3, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(3, 7), Instantiate(BlackQueen, tiles[3, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);

        // Rooks
       //[0, 7] = Instantiate(BlackRook, tiles[0, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(0, 7), Instantiate(BlackRook, tiles[0, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[7, 7] = Instantiate(BlackRook, tiles[7, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(7, 7), Instantiate(BlackRook, tiles[7, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[0, 0] = Instantiate(WhiteRook, tiles[0, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(0, 0), Instantiate(WhiteRook, tiles[0, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[7, 0] = Instantiate(WhiteRook, tiles[7, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(7, 0), Instantiate(WhiteRook, tiles[7, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);

        // Bishops
       //[2, 7] = Instantiate(BlackBishop, tiles[2, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(2, 7), Instantiate(BlackBishop, tiles[2, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[5, 7] = Instantiate(BlackBishop, tiles[5, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(5, 7), Instantiate(BlackBishop, tiles[5, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[2, 0] = Instantiate(WhiteBishop, tiles[2, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(2, 0), Instantiate(WhiteBishop, tiles[2, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[5, 0] = Instantiate(WhiteBishop, tiles[5, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(5, 0), Instantiate(WhiteBishop, tiles[5, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);

        // Knights
       //[1, 7] = Instantiate(BlackKnight, tiles[1, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(1, 7), Instantiate(BlackKnight, tiles[1, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[6, 7] = Instantiate(BlackKnight, tiles[6, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(6, 7), Instantiate(BlackKnight, tiles[6, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[1, 0] = Instantiate(WhiteKnight, tiles[1, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(1, 0), Instantiate(WhiteKnight, tiles[1, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);
       //[6, 0] = Instantiate(WhiteKnight, tiles[6, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieceDict.Add(new Vector2(6, 0), Instantiate(WhiteKnight, tiles[6, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData);

        for (int i = 0; i < pieceDict.Count; i++)
        {
            PieceData piece = pieceDict.ElementAt(i).Value;
            piece.position = pieceDict.ElementAt(i).Key;

            if (piece.isWhite == 1)
            {
                whitePieces.Add(piece.position, piece);
            }
            else if (piece.isWhite == -1)
            {
                blackPieces.Add(piece.position, piece);
            }
        }
    }

    public bool CheckChecks(IDictionary<Vector2, PieceData> oppPieces, int caller)
    {
        Vector2 kingPos = caller == 1 ? white_kingpos : black_kingpos;

        for (int i = 0; i < oppPieces.Count; i++)
        {
            PieceData piece = oppPieces.ElementAt(i).Value;
            if (piece.LegalMoves(pieceDict).Contains(kingPos)) return true;
        }

        return false;
    }

    public enum MateType
    {
        None,
        Checkmate,
        Stalemate
    }

    public MateType GenerateEndState(int turn)
    {
        List<(Vector2, List<Vector2>)> piecesAndMoves = MoveGenerator.GenerateAllMoves(pieces, turn);

        foreach ((Vector2, List<Vector2>) piece in piecesAndMoves)
        {
            foreach (Vector2 move in piece.Item2)
            {
                PieceData[,] testPieces = DeepCopyAllPieces(pieces);
                // If any move is legal, it's not checkmate
                if (TestMove(piece.Item1, move, turn, testPieces)) return MateType.None;
            }
        }

        // If no moves are legal and is in check, return checkmate
        if (CheckChecks(pieces, turn)) return MateType.Checkmate;
        // If not in check, return stalemate
        else return MateType.Stalemate;
    }

    public PieceData[,] DeepCopyAllPieces(PieceData[,] source)
    {
        PieceData[,] target = new PieceData[8, 8];
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (source[i, j] != null)
                {
                    target[i, j] = source[i, j].DeepCopy();
                }
            }
        }

        return target;
    }

    public IEnumerator TakeCoroutine(GameObject piece)
    {
        float elapsedTime = 0;
        float jumpHeight = 2f;
        float down = 20f;

        Vector3 start = piece.transform.position + new Vector3(0, 0, -5);
        Vector3 highestPos = new Vector3(start.x, start.y + jumpHeight, start.z);
        Vector3 end = new Vector3(start.x, start.y - down, start.z);

        while (elapsedTime < 0.5f)
        {
            piece.transform.position = Vector3.Lerp(start, highestPos, elapsedTime / 0.5f);
            piece.transform.Rotate(0, 0, 360 * Time.deltaTime * 10);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = highestPos;

        while (elapsedTime < 2f)
        {
            piece.transform.position = Vector3.Lerp(highestPos, end, (elapsedTime - 0.5f) / 0.2f);
            piece.transform.Rotate(0, 0, 360 * Time.deltaTime * 10);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        piece.transform.position = end;
        Destroy(piece);
    }
    
    public IEnumerator MoveCoroutine(GameObject piece, Vector3 start, Vector3 end, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            piece.transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = end;
    }
}
