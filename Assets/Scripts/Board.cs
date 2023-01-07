using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject[,] tiles = new GameObject[8, 8];
    public static BoardData board;
    [SerializeField] MoveGenerator moveGenerator;

    [Header("Board")]
    float TileScale;
    [SerializeField] GameObject Tile;
    [SerializeField] Color FirstColor;
    [SerializeField] Color SecondColor;
    [SerializeField] bool defaultStartingPos = false;
    [SerializeField] string fenStartingPos;

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
    [SerializeField] public AudioClip checkSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitBoard();
        if (defaultStartingPos)
        {
            fenStartingPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            InitFENPosition();
        }
        else
        {
            InitFENPosition();
        }
    }
    
    public bool MovePiece(Vector2 from, Vector2 to, BoardData board)
    {
        BoardData testBoard = board.DeepCopy();
        if (!TestMove(from, to, testBoard)) return false;
        
        PieceData fromPiece = board.pieces[(int)from.x, (int)from.y];
        board.fiftyMoveCounter++;

        if (fromPiece.type == PieceData.Type.King || fromPiece.type == PieceData.Type.Rook || fromPiece.type == PieceData.Type.Pawn)
        {
            if (fromPiece.type == PieceData.Type.Pawn)
            {
                if (to == board.enPassant)
                {
                    // En Passant Capture
                    StartCoroutine(TakeCoroutine(board.pieces[(int)to.x, (int)from.y].gameObject));
                    audioSource.PlayOneShot(captureSound);
                    board.pieces[(int)to.x, (int)from.y] = null;
                }
            }

            if (fromPiece.type == PieceData.Type.Rook)
            {
                (int, int) index = fromPiece.isWhite == 1 ? (0, 1) : (2, 3);
                if (from.x == 7 && board.castling[index.Item1]) board.castling[index.Item1] = false; // King side
                if (from.x == 0 && board.castling[index.Item2]) board.castling[index.Item2] = false; // Queen side
            }
            
            // Update king positions
            if (fromPiece.type == PieceData.Type.King)
            {
                if (board.sideToMove == 1)
                {
                    board.white_kingpos = to;
                }
                else
                {
                    board.black_kingpos = to;
                }

                // Update castling
                (int, int) index = fromPiece.isWhite == 1 ? (0, 1) : (2, 3);
                board.castling[index.Item1] = false;
                board.castling[index.Item2] = false;

                if (Math.Abs(from.x - to.x) == 2)
                {
                    // Castling
                    if (to.x == 6)
                    {
                        // Kingside
                        // Move Rook
                        board.pieces[5, (int)from.y] = board.pieces[7, (int)from.y];
                        board.pieces[5, (int)from.y].position = new Vector2(5, from.y);
                        board.pieces[7, (int)from.y] = null;
                    }
                    else if (to.x == 2)
                    {
                        // Queenside
                        // Move Rook
                        board.pieces[3, (int)from.y] = board.pieces[0, (int)from.y];
                        board.pieces[3, (int)from.y].position = new Vector2(3, from.y);
                        board.pieces[0, (int)from.y] = null;
                    }
                }
            }
        }

        // Extra pawn logic
        board.enPassant = new Vector2(-1, -1);
        if (fromPiece.type == PieceData.Type.Pawn)
        {
            board.fiftyMoveCounter = 0;

            if (Math.Abs(from.y - to.y) == 2)
            {
                board.enPassant = new Vector2(from.x, (from.y + to.y) / 2);
            }

            if (to.y == 0)
            {
                fromPiece.type = PieceData.Type.Queen;
                Destroy(fromPiece.gameObject);
                fromPiece.gameObject = Instantiate(BlackQueen, tiles[(int)to.x, (int)to.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
            }
            else if (to.y == 7)
            {
                fromPiece.type = PieceData.Type.Queen;
                Destroy(fromPiece.gameObject);
                fromPiece.gameObject = Instantiate(WhiteQueen, tiles[(int)to.x, (int)to.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
            }
        }

        if (board.pieces[(int)to.x, (int)to.y] != null)
        {
            StartCoroutine(TakeCoroutine(board.pieces[(int)to.x, (int)to.y].gameObject));
            audioSource.PlayOneShot(captureSound);
            audioSource.PlayOneShot(moveSound);
            board.fiftyMoveCounter = 0;
        } else
        {
            audioSource.PlayOneShot(moveSound);
        }

        // move piece
        board.pieces[(int)to.x, (int)to.y] = fromPiece;
        // update piece pos
        board.pieces[(int)to.x, (int)to.y].position = to;
        // delete old pos piece
        board.pieces[(int)from.x, (int)from.y] = null;

        board.sideToMove = -board.sideToMove;

        return true;
    }

    public bool TestMove(Vector2 from, Vector2 to, BoardData testBoard)
    {
        PieceData fromPiece = testBoard.pieces[(int)from.x, (int)from.y];

        if (fromPiece == null) return false;
        if (fromPiece.isWhite != testBoard.sideToMove) return false;
        if (!fromPiece.LegalMoves(testBoard).Contains(to)) return false;

        if (fromPiece.type == PieceData.Type.King || fromPiece.type == PieceData.Type.Rook || fromPiece.type == PieceData.Type.Pawn)
        {
            if (fromPiece.type == PieceData.Type.Pawn)
            {
                if (to == testBoard.enPassant)
                {
                    // En Passant Capture
                    testBoard.pieces[(int)to.x, (int)from.y] = null;
                }
            }

            if (fromPiece.type == PieceData.Type.Rook)
            {
                (int, int) index = fromPiece.isWhite == 1 ? (0, 1) : (2, 3);
                if (from.x == 7 && testBoard.castling[index.Item1]) testBoard.castling[index.Item1] = false; // King side
                if (from.x == 0 && testBoard.castling[index.Item2]) testBoard.castling[index.Item2] = false; // Queen side
            }

            // Update king positions
            if (fromPiece.type == PieceData.Type.King)
            {
                if (testBoard.sideToMove == 1)
                {
                    testBoard.white_kingpos = to;
                }
                else
                {
                    testBoard.black_kingpos = to;
                }

                // Update castling
                (int, int) index = fromPiece.isWhite == 1 ? (0, 1) : (2, 3);
                testBoard.castling[index.Item1] = false;
                testBoard.castling[index.Item2] = false;

                if (Math.Abs(from.x - to.x) == 2)
                {
                    // Castling
                    if (to.x == 6)
                    {
                        // Kingside
                        // Move Rook
                        testBoard.pieces[5, (int)from.y] = testBoard.pieces[7, (int)from.y];
                        testBoard.pieces[5, (int)from.y].position = new Vector2(5, from.y);
                        testBoard.pieces[7, (int)from.y] = null;
                    }
                    else if (to.x == 2)
                    {
                        // Queenside
                        // Move Rook
                        testBoard.pieces[3, (int)from.y] = testBoard.pieces[0, (int)from.y];
                        testBoard.pieces[3, (int)from.y].position = new Vector2(3, from.y);
                        testBoard.pieces[0, (int)from.y] = null;
                    }
                }
            }
        }

        // Extra pawn logic
        testBoard.enPassant = new Vector2(-1, -1);
        if (fromPiece.type == PieceData.Type.Pawn)
        {
            if (Math.Abs(from.y - to.y) == 2)
            {
                testBoard.enPassant = new Vector2(from.x, (from.y + to.y) / 2);
            }

            if (to.y == 0 || to.y == 7)
            {
                fromPiece.type = PieceData.Type.Queen;
            }
        }

        // Move Piece
        testBoard.pieces[(int)to.x, (int)to.y] = fromPiece;
        testBoard.pieces[(int)to.x, (int)to.y].position = to;
        testBoard.pieces[(int)from.x, (int)from.y] = null;
        
        if (CheckChecks(testBoard))
        {
            testBoard.sideToMove = -testBoard.sideToMove;
            return false;
        }

        testBoard.sideToMove = -testBoard.sideToMove;
        return true;
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
        if (board != null)
        {
            foreach (PieceData piece in board.pieces)
            {
                if (piece == null) continue;
                Destroy(piece.gameObject);
            }
        }

        bool[] castling = new bool[4] { true, true, true, true };
        board = new BoardData(new PieceData[8, 8], new Vector2(4, 0), new Vector2(4, 7), new Vector2(-1, -1), castling, 0, 1);
        
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            board.pieces[i, 1] = Instantiate(WhitePawn, tiles[i, 1].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
            board.pieces[i, 6] = Instantiate(BlackPawn, tiles[i, 6].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        }
        
        // Kings
        board.pieces[4, 0] = Instantiate(WhiteKing, tiles[4, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[4, 7] = Instantiate(BlackKing, tiles[4, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Queens
        board.pieces[3, 0] = Instantiate(WhiteQueen, tiles[3, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[3, 7] = Instantiate(BlackQueen, tiles[3, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Rooks
        board.pieces[0, 7] = Instantiate(BlackRook, tiles[0, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[7, 7] = Instantiate(BlackRook, tiles[7, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[0, 0] = Instantiate(WhiteRook, tiles[0, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[7, 0] = Instantiate(WhiteRook, tiles[7, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Bishops
        board.pieces[2, 7] = Instantiate(BlackBishop, tiles[2, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[5, 7] = Instantiate(BlackBishop, tiles[5, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[2, 0] = Instantiate(WhiteBishop, tiles[2, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[5, 0] = Instantiate(WhiteBishop, tiles[5, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        // Knights
        board.pieces[1, 7] = Instantiate(BlackKnight, tiles[1, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[6, 7] = Instantiate(BlackKnight, tiles[6, 7].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[1, 0] = Instantiate(WhiteKnight, tiles[1, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;
        board.pieces[6, 0] = Instantiate(WhiteKnight, tiles[6, 0].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceData piece = board.pieces[i, j];
                if (piece == null) continue;

                piece.position = new Vector2(i, j);
            }
        }
    }
    
    public void InitFENPosition()
    {
        string fen = fenStartingPos;
        board = new BoardData(new PieceData[8, 8], Vector2.zero, Vector2.zero, new Vector2(-1, -1), new bool[4] { false, false, false, false }, 0, 0);

        // Split FEN string
        string[] fenSplit = fen.Split(' ');

        // Split board
        string[] boardSplit = fenSplit[0].Split('/');

        // Pieces
        for (int i = 0; i < 8; i++)
        {
            int adder = 0;
            for (int j = 0; j < boardSplit[i].Length; j++)
            {
                char c = boardSplit[i][j];

                if (Char.IsNumber(c))
                {
                    adder += int.Parse(c.ToString());
                    adder -= 1;
                } 
                else
                {
                    GameObject piece = null;
                    switch (c)
                    {
                        case 'p':
                            piece = BlackPawn;
                            break;
                        case 'P':
                            piece = WhitePawn;
                            break;
                        case 'n':
                            piece = BlackKnight;
                            break;
                        case 'N':
                            piece = WhiteKnight;
                            break;
                        case 'b':
                            piece = BlackBishop;
                            break;
                        case 'B':
                            piece = WhiteBishop;
                            break;
                        case 'r':
                            piece = BlackRook;
                            break;
                        case 'R':
                            piece = WhiteRook;
                            break;
                        case 'q':
                            piece = BlackQueen;
                            break;
                        case 'Q':
                            piece = WhiteQueen;
                            break;
                        case 'k':
                            piece = BlackKing;
                            break;
                        case 'K':
                            piece = WhiteKing;
                            break;
                    }

                    // Adder is for empty spaces in FEN notation, [7 - i] is due to FEN boards being flipped compared to mine
                    board.pieces[j + adder, 7 - i] = Instantiate(piece, tiles[j + adder, 7 - i].transform.position + new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Piece>().pieceData;

                    // Set King pos
                    if (piece == WhiteKing)
                    {
                        board.white_kingpos = new Vector2(j + adder, 7 - i);
                    }
                    else if (piece == BlackKing)
                    {
                        board.black_kingpos = new Vector2(j + adder, 7 - i);
                    }
                }
            }
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceData piece = board.pieces[i, j];
                if (piece == null) continue;

                piece.position = new Vector2(i, j);
            }
        }

        // Turn
        board.sideToMove = fenSplit[1] == "w" ? 1 : -1;

        // Castling
        string castling = fenSplit[2];
        if (castling.Contains("K")) board.castling[0] = true;
        if (castling.Contains("Q")) board.castling[1] = true;
        if (castling.Contains("k")) board.castling[2] = true;
        if (castling.Contains("q")) board.castling[3] = true;

        // En Passant
        if (fenSplit[3] != "-")
        {
            int x = fenSplit[3][0] - 'a';
            int y = (fenSplit[3][1] - '1');

            board.enPassant = new Vector2(x, y);
        }
        
        // Halfmove clock - Fifty Move Rule
        board.fiftyMoveCounter = int.Parse(fenSplit[4]);

        // Fullmove
        // TODO
    }

    public bool CheckChecks(BoardData board)
    {
        Vector2 kingPos = board.sideToMove == 1 ? board.white_kingpos : board.black_kingpos;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PieceData piece = board.pieces[i, j];
                if (piece == null) continue;
                if (piece.isWhite == board.sideToMove) continue;
                
                // Only checks moves for the opposite color of caller
                if (piece.LegalMoves(board).Contains(kingPos)) return true;
            }
        }

        return false;
    }

    public enum MateType
    {
        None,
        Checkmate,
        Stalemate
    }

    public MateType GenerateEndState(BoardData board)
    {
        List<(Vector2, List<Vector2>)> piecesAndMoves = moveGenerator.GenerateAllMoves(board);

        foreach ((Vector2, List<Vector2>) piece in piecesAndMoves)
        {
            foreach (Vector2 move in piece.Item2)
            {
                BoardData testBoard = board.DeepCopy();
                // If any move is legal, it's not checkmate
                if (TestMove(piece.Item1, move, testBoard)) return MateType.None;
            }
        }

        // If no moves are legal and is in check, return checkmate
        if (CheckChecks(board)) return MateType.Checkmate;
        // If not in check, return stalemate
        else return MateType.Stalemate;
    }

    public PieceData[,] DeepCopy(PieceData[,] source)
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

    public string VectorToMove(Vector2 from, Vector2 to)
    {
        return $"{(char)('a' + from.x)}{(char)('1' + from.y)}-{(char)('a' + to.x)}{(char)('1' + to.y)}";
    }
}
