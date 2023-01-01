using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class MouseManager : MonoBehaviour
{
    bool holding;
    bool canMove = true;
    [SerializeField] int whiteTurn;
    int WhiteTurn 
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
                //ComputerMove();
            }
        }
    }
    [SerializeField] Board board;
    [SerializeField] Engine engine;
    PieceData heldPiece;
    GameObject hoveredTile;
    [SerializeField] GameObject highlightPrefabTile;
    GameObject highlightTile;
    [SerializeField] GameObject MoveHighlightPrefabPiece;
    List<GameObject> MoveHighlightPieces = new();
    [SerializeField] GameObject AttackHighlightPrefabPiece;
    List<GameObject> AttackHighlightPieces = new();
    [SerializeField] GameObject previousMovePrefabTile;
    List<GameObject> previousMoveTiles = new();

    [SerializeField] GameObject newGameButton;
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text blackTimerText;
    [SerializeField] TMP_Text whiteTimerText;
    [SerializeField] GameObject timerDivider;
    [SerializeField] float startingTimeSeconds;
    bool isCountingDown = false;
    float blackTimer;
    float whiteTimer;

    public static bool canCastle = true;
    
    // Start is called before the first frame update
    void Start()
    {
        highlightTile = Instantiate(highlightPrefabTile, new Vector2(-1000, 5000), Quaternion.identity);
        newGameButton.SetActive(false);

        blackTimer = startingTimeSeconds;
        whiteTimer = startingTimeSeconds;
        DisplayTime(blackTimer - 1, blackTimerText);
        DisplayTime(whiteTimer - 1, whiteTimerText);
    }

    // Update is called once per frame
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
                if (Board.pieces[(int)tilePos.x, (int)tilePos.y] != null)
                {
                    if (Board.pieces[(int)tilePos.x, (int)tilePos.y].isWhite == WhiteTurn)
                    {
                        heldPiece = Board.pieces[(int)tilePos.x, (int)tilePos.y];
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

                if (board.MovePiece(heldPiece.position, tile.position, WhiteTurn, Board.pieces))
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
                            Board.pieces[5, (int)tile.position.y].gameObject.transform.position = board.tiles[5, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
                        }
                        else if (tile.position.x == 2)
                        {
                            Board.pieces[3, (int)tile.position.y].gameObject.transform.position = board.tiles[3, (int)tile.position.y].transform.position + new Vector3(0, 0, -1);
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
            canCastle = !board.CheckChecks(Board.pieces, WhiteTurn);
            int mate = board.CheckMates(WhiteTurn);
            if (mate == 1)
            {
                newGameButton.SetActive(true);
                timerDivider.SetActive(false);
                winText.text = "White Wins!";
                canMove = false;
                isCountingDown = false;
            }
            else if (mate == -1)
            {
                newGameButton.SetActive(true);
                timerDivider.SetActive(false);
                winText.text = "Black Wins!";
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
                highlightTile.transform.position = hoveredTile.transform.position + new Vector3(0, 0, -2);
            }
        }
    }

    async void ComputerMove()
    {
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        await Task.Run(() =>
        {
            bestMove = engine.minMove(Board.pieces, 3);
        });
        board.MovePiece(bestMove.Item1, bestMove.Item2, WhiteTurn, Board.pieces);

        // Previous move tiles
        if (previousMoveTiles.Count > 0) foreach (GameObject t in previousMoveTiles) Destroy(t);
        // Create previous move prefab at computer from and to positions
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, board.tiles[(int)bestMove.Item1.x, (int)bestMove.Item1.y].transform.position, Quaternion.identity));
        previousMoveTiles.Add(Instantiate(previousMovePrefabTile, board.tiles[(int)bestMove.Item2.x, (int)bestMove.Item2.y].transform.position, Quaternion.identity));

        Board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[(int)bestMove.Item2.x, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);

        // Castling
        if (Board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].type == PieceData.Type.King && Board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].hasCastled)
        {
            Board.pieces[(int)bestMove.Item2.x, (int)bestMove.Item2.y].hasCastled = false;
            if (bestMove.Item2.x == 6)
            {
                Board.pieces[5, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[5, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
            }
            else if (bestMove.Item2.x == 2)
            {
                Board.pieces[3, (int)bestMove.Item2.y].gameObject.transform.position = board.tiles[3, (int)bestMove.Item2.y].transform.position + new Vector3(0, 0, -1);
            }
        }
        WhiteTurn = -WhiteTurn;
        
        canCastle = !board.CheckChecks(Board.pieces, WhiteTurn);
        int mate = board.CheckMates(WhiteTurn);
        if (mate == 1)
        {
            newGameButton.SetActive(true);
            timerDivider.SetActive(false);
            winText.text = "White Wins!";
            canMove = false;
            isCountingDown = false;
        }
        else if (mate == -1)
        {
            newGameButton.SetActive(true);
            timerDivider.SetActive(false);
            winText.text = "Black Wins!";
            canMove = false;
            isCountingDown = false;
        }
    }
    
    void DrawMoveHighlights()
    {
        if (heldPiece == null) return;

        List<Vector2> legalMoves = heldPiece.LegalMoves(Board.pieces);
        
        foreach (Vector2 move in legalMoves)
        {
            PieceData[,] testPieces = board.DeepCopyAllPieces(Board.pieces);
            if (!board.TestMove(heldPiece.position, move, WhiteTurn, testPieces))
            {
                continue;
            }
            
            if (Board.pieces[(int)move.x, (int)move.y] == null)
            {
                GameObject moveHighlight = Instantiate(MoveHighlightPrefabPiece, board.tiles[(int)move.x, (int)move.y].transform.position + new Vector3(0, 0, -2), Quaternion.identity);
                MoveHighlightPieces.Add(moveHighlight);
            }
            else
            {
                GameObject attackHighlight = Instantiate(AttackHighlightPrefabPiece, board.tiles[(int)move.x, (int)move.y].transform.position + new Vector3(0, 0, -2), Quaternion.identity);
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
        newGameButton.SetActive(false);
        WhiteTurn = 1;
        timerDivider.SetActive(true);
        blackTimer = startingTimeSeconds;
        whiteTimer = startingTimeSeconds;
        DisplayTime(blackTimer - 1, blackTimerText);
        DisplayTime(whiteTimer - 1, whiteTimerText);
        isCountingDown = false;
        canMove = true;
        winText.text = "";
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


