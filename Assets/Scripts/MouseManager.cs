using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class MouseManager : MonoBehaviour
{
    bool holding;
    bool canMove = true;
    public int whiteTurn;
    public int WhiteTurn 
    {
        get
        {
            return whiteTurn;
        }
        
        set
        {
            whiteTurn = value;
            if (value == -1)
            {
                ComputerMove();
            }
        }
    }
    [SerializeField] Board board;
    [SerializeField] Evaluation evaluation;
    [SerializeField] UI ui;
    PieceData heldPiece;
    GameObject hoveredTile;
    public static bool canCastle = true;

    [Header("Highlights")]
    [SerializeField] GameObject highlightPrefabTile;
    GameObject highlightTile;
    [SerializeField] GameObject MoveHighlightPrefabPiece;
    List<GameObject> MoveHighlightPieces = new();
    [SerializeField] GameObject AttackHighlightPrefabPiece;
    List<GameObject> AttackHighlightPieces = new();
    [SerializeField] GameObject previousMovePrefabTile;
    List<GameObject> previousMoveTiles = new();

    [Header("New Game")]
    [SerializeField] TMP_Text winText;
    [SerializeField] GameObject newGameButton;

    [Header("Timers")]
    [SerializeField] GameObject timerDivider;
    [SerializeField] TMP_Text blackTimerText;
    [SerializeField] TMP_Text whiteTimerText;
    [SerializeField] float startingTimeSeconds;
    bool isCountingDown = false;
    float blackTimer;
    float whiteTimer;

    [Header("Engine")]
    [SerializeField] Engine engine;
    [SerializeField] int depth;

    void Start()
    {
        highlightTile = Instantiate(highlightPrefabTile, new Vector2(-1000, 5000), Quaternion.identity);
        newGameButton.SetActive(false);

        blackTimer = startingTimeSeconds;
        whiteTimer = startingTimeSeconds;
        DisplayTime(blackTimer - 1, blackTimerText);
        DisplayTime(whiteTimer - 1, whiteTimerText);
    }

    void Update()
    {

        if (!canMove)
        {
            return;
        }

        if (isCountingDown)
        {
            HandleTimers();
        }
            
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Vector2 tilePos = hit.collider.gameObject.GetComponent<Tile>().position;
                if (Board.board.pieces[(int)tilePos.x, (int)tilePos.y] != null)
                {
                    if (Board.board.pieces[(int)tilePos.x, (int)tilePos.y].isWhite == 1)
                    {
                        heldPiece = Board.board.pieces[(int)tilePos.x, (int)tilePos.y];
                        DrawMoveHighlights();
                        holding = true;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;
            highlightTile.transform.position = new Vector2(-1000, 5000);

            if (hoveredTile != null && heldPiece != null)
            {
                Tile tile = hoveredTile.GetComponent<Tile>();

                if (board.MovePiece(heldPiece.position, tile.position, WhiteTurn, Board.board))
                {
                    if (!isCountingDown) isCountingDown = true;
                    WhiteTurn = -WhiteTurn;
                    heldPiece.gameObject.transform.position = tile.transform.position + new Vector3(0, 0, -1);

                    // Castling
                    if (heldPiece.type == PieceData.Type.King && heldPiece.hasCastled)
                    {
                        heldPiece.hasCastled = false;
                        if (tile.position.x == 6)
                        {
                            Board.board.pieces[5, (int)tile.position.y].gameObject.transform.position = board.tiles[5, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
                        }
                        else if (tile.position.x == 2)
                        {
                            Board.board.pieces[3, (int)tile.position.y].gameObject.transform.position = board.tiles[3, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
                        }
                    }
                }
                else
                {
                    heldPiece.gameObject.transform.position = board.tiles[(int)heldPiece.position.x, (int)heldPiece.position.y].transform.position + new Vector3(0, 0, -1);
                }
            }

            ClearMoveHighlights();
            heldPiece = null;
            canCastle = !board.CheckChecks(Board.board, WhiteTurn);
            
            Board.MateType endState = board.GenerateEndState(Board.board, WhiteTurn);
            if (endState == Board.MateType.None) return;
            else if (endState == Board.MateType.Checkmate && WhiteTurn == 1)
            {
                newGameButton.SetActive(true);
                winText.text = "Black Wins!";
                canMove = false;
                isCountingDown = false;
            }
            else if (endState == Board.MateType.Checkmate && WhiteTurn == -1)
            {
                newGameButton.SetActive(true);
                winText.text = "White Wins!";
                canMove = false;
                isCountingDown = false;
            }
            else if (endState == Board.MateType.Stalemate || Board.board.fiftyMoveCounter >= 50)
            {
                newGameButton.SetActive(true);
                winText.text = "Stalemate!";
                canMove = false;
                isCountingDown = false;
            }
        }

        if (holding)
        {
            heldPiece.gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                hoveredTile = hit.collider.gameObject;
                highlightTile.transform.position = hoveredTile.transform.position + new Vector3(0, 0, -1);
            }
        }
    }

    async void ComputerMove()
    {
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        int evalScore = 0;
        int nodesScore = 0;

        await Task.Run(() =>
        {
            (bestMove, evalScore, nodesScore) = engine.MinMove(Board.board, depth);
        });
        
        // If time is up and the computer wants to move, return
        if (!canMove) return;

        string move = board.VectorToMove(bestMove.Item1, bestMove.Item2);
        ui.updateEngineText(evalScore, nodesScore, move);
        board.MovePiece(bestMove.Item1, bestMove.Item2, WhiteTurn, Board.board);
        
        //Board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[(int)bestMove.Item2.x, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
        // Moves the piece representative
        GameObject gPiece = Board.board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].gameObject;
        Vector3 from = gPiece.transform.position;
        Vector3 to = board.tiles[(int)bestMove.Item2.x, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
        StartCoroutine(board.MoveCoroutine(gPiece, from, to, 0.15f));

        // Deletes previous move tiles
        if (previousMoveTiles.Count > 0) foreach (GameObject t in previousMoveTiles) Destroy(t);
        previousMoveTiles.Clear();
        // Create previous move prefab at computer from and to positions
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, board.tiles[(int)bestMove.Item1.x, (int)bestMove.Item1.y].transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity));
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, board.tiles[(int)bestMove.Item2.x, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity));

        // Castling
        if (Board.board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].type == PieceData.Type.King && Board.board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].hasCastled)
        {
            Board.board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].hasCastled = false;
            if (bestMove.Item2.x == 6)
            {
                Board.board.pieces[5, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[5, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
            }
            else if (bestMove.Item2.x == 2)
            {
                Board.board.pieces[3, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[3, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
            }
        }
        WhiteTurn = -WhiteTurn;
        
        canCastle = !board.CheckChecks(Board.board, WhiteTurn);
        
        Board.MateType endState = board.GenerateEndState(Board.board, whiteTurn);
        if (endState == Board.MateType.None) return;
        else if (endState == Board.MateType.Checkmate && WhiteTurn == 1)
        {
            newGameButton.SetActive(true);
            winText.text = "Black Wins!";
            canMove = false;
            isCountingDown = false;
        }
        else if (endState == Board.MateType.Checkmate && WhiteTurn == -1)
        {
            newGameButton.SetActive(true);
            winText.text = "White Wins!";
            canMove = false;
            isCountingDown = false;
        }
        else if (endState == Board.MateType.Stalemate || Board.board.fiftyMoveCounter >= 50)
        {
            newGameButton.SetActive(true);
            winText.text = "Stalemate!";
            canMove = false;
            isCountingDown = false;
        }
    }
    
    void DrawMoveHighlights()
    {
        if (heldPiece == null) return;

        List<Vector2> legalMoves = heldPiece.LegalMoves(Board.board);
        
        foreach (Vector2 move in legalMoves)
        {
            BoardData testBoard = Board.board.DeepCopy();
            if (!board.TestMove2(heldPiece.position, move, WhiteTurn, testBoard)) continue;
            
            if (Board.board.pieces[(int)move.x, (int)move.y] == null)
            {
                GameObject moveHighlight = Instantiate(MoveHighlightPrefabPiece, board.tiles[(int)move.x, (int)move.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                MoveHighlightPieces.Add(moveHighlight);
            }
            else
            {
                GameObject attackHighlight = Instantiate(AttackHighlightPrefabPiece, board.tiles[(int)move.x, (int)move.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                AttackHighlightPieces.Add(attackHighlight);
            }
        }
    }

    void ClearMoveHighlights()
    {
        foreach (GameObject moveHighlight in MoveHighlightPieces)
        {
            Destroy(moveHighlight);
        }
        MoveHighlightPieces.Clear();

        foreach (GameObject attackHighlight in AttackHighlightPieces)
        {
            Destroy(attackHighlight);
        }
        AttackHighlightPieces.Clear();
    }

    public void ResetTurn()
    {
        if (previousMoveTiles.Count > 0) foreach (GameObject t in previousMoveTiles) Destroy(t);
        previousMoveTiles.Clear();
        newGameButton.SetActive(false);
        timerDivider.SetActive(true);
        WhiteTurn = 1;
        blackTimer = startingTimeSeconds;
        whiteTimer = startingTimeSeconds;
        DisplayTime(blackTimer - 1, blackTimerText);
        DisplayTime(whiteTimer - 1, whiteTimerText);
        isCountingDown = false;
        canMove = true;
        winText.text = "";
        ui.updateEngineText(0, 0, "");
        board.InitStartingPositions();
    }

    void HandleTimers()
    {
        if (whiteTurn == 1)
        {
            if (whiteTimer > 0)
            {
                whiteTimer -= Time.deltaTime;
                DisplayTime(whiteTimer, whiteTimerText);
            }
            else
            {
                whiteTimer = 0;
                newGameButton.SetActive(true);
                timerDivider.SetActive(false);
                winText.text = "Black Wins!";
                canMove = false;
                isCountingDown = false;
            }
        }
        else
        {
            if (blackTimer > 0)
            {
                blackTimer -= Time.deltaTime;
                DisplayTime(blackTimer, blackTimerText);
            }
            else
            {
                blackTimer = 0;
                newGameButton.SetActive(true);
                timerDivider.SetActive(false);
                winText.text = "White Wins!";
                canMove = false;
                isCountingDown = false;
            }
        }   
    }

    void DisplayTime(float timeToDisplay, TMP_Text timeText)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}


