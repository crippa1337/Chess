using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class MouseManager : MonoBehaviour
{
    bool holding;
    bool canMove = true;

    [SerializeField] Board board;
    [SerializeField] Evaluation evaluation;
    [SerializeField] UI ui;
    AudioSource audioSource;
    PieceData heldPiece;
    GameObject hoveredTile;

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
    bool isCountingDown = false;
    float blackTimer;
    float whiteTimer;

    [Header("Engine")]
    [SerializeField] Engine engine;

    bool isEngineThinking = false;

    void Start()
    {
        highlightTile = Instantiate(highlightPrefabTile, new Vector2(-1000, 5000), Quaternion.identity);
        newGameButton.SetActive(false);

        blackTimer = GameManager.startSeconds;
        whiteTimer = GameManager.startSeconds;
        DisplayTime(blackTimer - 1, blackTimerText);
        DisplayTime(whiteTimer - 1, whiteTimerText);
        
        audioSource = board.GetComponent<AudioSource>();
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

        if (Board.board.sideToMove == -1 && !isEngineThinking)
        {
            ComputerMove();
            Debug.Log("Thinking....");
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
                bool[] copyCastling = new bool[4];
                for (int i = 0; i < 4; i++)
                {
                    copyCastling[i] = Board.board.castling[i];
                }

                if (board.MovePiece(new Move(heldPiece.position, tile.position), Board.board))
                {
                    if (!isCountingDown) isCountingDown = true;
                    heldPiece.gameObject.transform.position = tile.transform.position + new Vector3(0, 0, -1);

                    // Castling
                    if (heldPiece.type == PieceData.Type.King)
                    {
                        (int, int) index = heldPiece.isWhite == 1 ? (0, 1) : (2, 3);

                        if (tile.position.x == 6 && copyCastling[index.Item1])
                        {
                            GameObject rookPiece = Board.board.pieces[5, (int)tile.position.y].gameObject;
                            Vector3 rookTo = board.tiles[5, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
                            StartCoroutine(board.MoveCoroutine(rookPiece, rookPiece.transform.position, rookTo, 0.25f));
                        }
                        else if (tile.position.x == 2 && copyCastling[index.Item2])
                        {
                            GameObject rookPiece = Board.board.pieces[3, (int)tile.position.y].gameObject;
                            Vector3 rookTo = board.tiles[3, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
                            StartCoroutine(board.MoveCoroutine(rookPiece, rookPiece.transform.position, rookTo, 0.25f));
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

            Board.MateType endState = board.GenerateEndState(Board.board);
            if (endState == Board.MateType.None) return;
            else if (endState == Board.MateType.Checkmate && Board.board.sideToMove == 1)
            {
                newGameButton.SetActive(true);
                winText.text = "Black Wins!";
                canMove = false;
                isCountingDown = false;
            }
            else if (endState == Board.MateType.Checkmate && Board.board.sideToMove == -1)
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
        isEngineThinking = true;
        Move bestMove = Helper.noneMove;
        int evalScore = 0;
        engine.nodesVisited = 0;

        await Task.Run(() =>
        {
            (evalScore, bestMove) = engine.Negamax(Board.board, engine.maxDepth, Helper.negInfinity, Helper.infinity);
        });
        
        // If time is up and the computer wants to move, return
        if (!canMove) return;

        bool[] copyCastling = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            copyCastling[i] = Board.board.castling[i];
        }
        
        string move = board.VectorToMove(bestMove.from, bestMove.to);
        ui.updateEngineText(evalScore, engine.nodesVisited, move);
        board.MovePiece(bestMove, Board.board);
        isEngineThinking = false;
        
        PieceData movedPiece = Board.board.pieces[(int)bestMove.to.x, (int)bestMove.to.y];
        Vector3 from = movedPiece.gameObject.transform.position;
        Tile toTile = board.tiles[(int)bestMove.to.x, (int)bestMove.to.y].GetComponent<Tile>();
        StartCoroutine(board.MoveCoroutine(movedPiece.gameObject, from, toTile.transform.position + new Vector3(0, 0, -1), 0.15f));

        // Deletes previous move tiles
        if (previousMoveTiles.Count > 0) foreach (GameObject t in previousMoveTiles) Destroy(t);
        previousMoveTiles.Clear();
        // Create previous move prefab at computer from and to positions
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, board.tiles[(int)bestMove.from.x, (int)bestMove.from.y].transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity));
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, toTile.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity));

        // Castling
        if (movedPiece.type == PieceData.Type.King)
        {
            (int, int) index = movedPiece.isWhite == 1 ? (0, 1) : (2, 3);
            
            if (toTile.position.x == 6 && copyCastling[index.Item1])
            {
                GameObject rookPiece = Board.board.pieces[5, (int)toTile.position.y].gameObject;
                Vector3 rookTo = board.tiles[5, (int)toTile.position.y].transform.position + new Vector3(0, 0, -1);
                StartCoroutine(board.MoveCoroutine(rookPiece, rookPiece.transform.position, rookTo, 0.25f));
            }
            else if (toTile.position.x == 2 && copyCastling[index.Item2])
            {
                GameObject rookPiece = Board.board.pieces[3, (int)toTile.position.y].gameObject;
                Vector3 rookTo = board.tiles[3, (int)toTile.position.y].transform.position + new Vector3(0, 0, -1);
                StartCoroutine(board.MoveCoroutine(rookPiece, rookPiece.transform.position, rookTo, 0.25f));
            }
        }
        
        if (board.CheckChecks(Board.board)) audioSource.PlayOneShot(board.checkSound);

        Board.MateType endState = board.GenerateEndState(Board.board);
        if (endState == Board.MateType.None) return;
        else if (endState == Board.MateType.Checkmate && Board.board.sideToMove == 1)
        {
            newGameButton.SetActive(true);
            winText.text = "Black Wins!";
            canMove = false;
            isCountingDown = false;
        }
        else if (endState == Board.MateType.Checkmate && Board.board.sideToMove == -1)
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
        
        foreach (Vector2 target in legalMoves)
        {
            Move move = new Move(heldPiece.position, target);
            BoardData testBoard = Board.board.DeepCopy();
            if (!board.TestMove(move, testBoard)) continue;
            
            if (Board.board.pieces[(int)target.x, (int)target.y] == null)
            {
                GameObject moveHighlight = Instantiate(MoveHighlightPrefabPiece, board.tiles[(int)target.x, (int)target.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                MoveHighlightPieces.Add(moveHighlight);
            }
            else
            {
                GameObject attackHighlight = Instantiate(AttackHighlightPrefabPiece, board.tiles[(int)target.x, (int)target.y].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
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
        blackTimer = GameManager.startSeconds;
        whiteTimer = GameManager.startSeconds;
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
        if (Board.board.sideToMove == 1)
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


