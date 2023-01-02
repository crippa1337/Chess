using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static PieceData[,] pieces = new PieceData[8, 8];
    public GameObject[,] tiles = new GameObject[8, 8];
    public static Vector2 white_kingpos = new Vector2(4, 0);
    public static Vector2 black_kingpos = new Vector2(4, 7);

    [SerializeField] GameObject Tile;
    float TileScale;
    [SerializeField] Color FirstColor;
    [SerializeField] Color SecondColor;

    [Space(20)]
    [Header("Pieces")]
    [SerializeField] GameObject BlackKing;
    [SerializeField] GameObject WhiteKing;
    [SerializeField] GameObject BlackQueen;
    [SerializeField] GameObject WhiteQueen;
    [SerializeField] GameObject BlackRook;
    [SerializeField] GameObject WhiteRook;
    [SerializeField] GameObject BlackBishop;
    [SerializeField] GameObject WhiteBishop;
    [SerializeField] GameObject BlackKnight;
    [SerializeField] GameObject WhiteKnight;
    [SerializeField] GameObject BlackPawn;
    [SerializeField] GameObject WhitePawn;

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
            StartCoroutine(LerpCoroutine(pieces[(int)to.x, (int)to.y].gameObject));
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
        foreach (PieceData piece in pieces)
        {
            if (piece != null)
            {
                Destroy(piece.gameObject);
            }
        }

        Array.Clear(pieces, 0, pieces.Length);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            pieces[i, 1] = Instantiate(WhitePawn, tiles[i, 1].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
            pieces[i, 6] = Instantiate(BlackPawn, tiles[i, 6].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        }

        // Kings
        pieces[4, 0] = Instantiate(WhiteKing, tiles[4, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[4, 7] = Instantiate(BlackKing, tiles[4, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Queens
        pieces[3, 0] = Instantiate(WhiteQueen, tiles[3, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[3, 7] = Instantiate(BlackQueen, tiles[3, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Rooks
        pieces[0, 7] = Instantiate(BlackRook, tiles[0, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[7, 7] = Instantiate(BlackRook, tiles[7, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[0, 0] = Instantiate(WhiteRook, tiles[0, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[7, 0] = Instantiate(WhiteRook, tiles[7, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Bishops
        pieces[2, 7] = Instantiate(BlackBishop, tiles[2, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[5, 7] = Instantiate(BlackBishop, tiles[5, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[2, 0] = Instantiate(WhiteBishop, tiles[2, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[5, 0] = Instantiate(WhiteBishop, tiles[5, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Knights
        pieces[1, 7] = Instantiate(BlackKnight, tiles[1, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[6, 7] = Instantiate(BlackKnight, tiles[6, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[1, 0] = Instantiate(WhiteKnight, tiles[1, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        pieces[6, 0] = Instantiate(WhiteKnight, tiles[6, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tiles[i, j].GetComponent<Tile>().position = new Vector2(i, j);
                if (pieces[i, j] != null)
                {
                    pieces[i, j].position = new Vector2(i, j);
                }
            }
        }
    }

    public bool CheckChecks(PieceData[,] checkPieces, int isWhite)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (checkPieces[i, j] == null) continue;
                PieceData piece = checkPieces[i, j];

                if (isWhite == 1 )
                {
                    // Check if any of the opponents moves are on the kingpos
                    if (piece.isWhite == -1)
                    {
                        foreach (Vector2 move in piece.LegalMoves(checkPieces))
                        {
                            if (move == white_kingpos)
                            {
                                return true;
                            }
                        }
                    } 
                }
                
                else if (isWhite == -1)
                {
                    // Check if any of the opponents moves are on the kingpos
                    if (piece.isWhite == 1)
                    {
                        foreach (Vector2 move in piece.LegalMoves(checkPieces))
                        {
                            if (move == black_kingpos)
                            {
                                return true;
                            }
                        }
                    }
                }
                   
            }
        }

        return false;
    }

    public int CheckMates(int turn)
    {
        if (CheckChecks(pieces, turn))
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] == null) continue;

                    if (pieces[i, j].isWhite == turn)
                    {
                        foreach (Vector2 move in pieces[i, j].LegalMoves(pieces))
                        {
                            PieceData[,] testPieces = DeepCopyAllPieces(pieces);
                            if (TestMove(pieces[i, j].position, move, turn, testPieces))
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            return -turn;
        }

        return 0;
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

    public IEnumerator LerpCoroutine(GameObject piece)
    {
        float elapsedTime = 0;
        float jumpHeight = 2f;
        float down = 20f;

        Vector3 start = piece.transform.position + new Vector3(0, 0, -5);
        Vector3 highestPos = new Vector3(start.x, start.y + jumpHeight, start.z);
        Vector3 end = new Vector3(start.x, start.y - down, start.z);

        while (elapsedTime < 0.5f)
        {
            piece.transform.position = Vector3.Lerp(start, highestPos, elapsedTime / 0.2f);
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
}
