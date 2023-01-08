using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public int nodesVisited;
    public int maxDepth;
    (Vector2, Vector2) noMove = (new Vector2(-1, -1), new Vector2(-1, -1));

    [SerializeField] Board gameBoard;
    [SerializeField] MoveGenerator moveGenerator;
    Evaluation evaluation;

    private void Start()
    {
        evaluation = GetComponent<Evaluation>();
    }

    public (int, (Vector2, Vector2)) Negamax(BoardData oldBoard, int depth, int alpha, int beta)
    {
        nodesVisited++;
        int ply = maxDepth - depth;
        
        if (depth == 0)
        {
            // return (QSearch(oldBoard, alpha, beta), noMove);
            return (evaluation.Evaluate(oldBoard, oldBoard.sideToMove), noMove);
        }

        int movesDone = 0;
        int bigEval = Helper.negInfinity;
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        List<(Vector2, List<Vector2>)> allMoves = moveGenerator.GenerateAllMoves(oldBoard);
        foreach (var (position, moves) in allMoves)
        {
            foreach (var move in moves)
            {
                // Make the move and call Negamax recursively
                BoardData newBoard = oldBoard.DeepCopy();
                gameBoard.TestMove(position, move, newBoard);
                movesDone++;
                int eval = -Negamax(newBoard, depth - 1, -beta, -alpha).Item1;
                if (eval > bigEval)
                {
                    bigEval = eval;
                    bestMove = (position, move);
                }

                alpha = Mathf.Max(alpha, eval);
                if (alpha >= beta)
                {
                    goto outer;
                }
            }
        }
        outer:
        
        if (movesDone == 0)
        {
            // Checkmate
            if (gameBoard.CheckChecks(oldBoard))
            {
                return (Helper.matedIn(ply), noMove);
            }
            // Stalemate
            else
            {
                return (0, noMove);
            }
        }

        return (bigEval, bestMove);
    }

    int QSearch(BoardData board, int alpha, int beta)
    {
        nodesVisited++;
        
        int bigEval = evaluation.Evaluate(board, board.sideToMove);
        if (bigEval >= beta)
        {
            return beta;
        }
        if (alpha < bigEval)
        {
            alpha = bigEval;
        }

        List<(Vector2, List<Vector2>)> allMoves = moveGenerator.GenerateAllCaptures(board);
        foreach (var (position, moves) in allMoves)
        {
            foreach (var move in moves)
            {
                // Make the move and call Negamax recursively
                BoardData newBoard = board.DeepCopy();
                gameBoard.TestMove(position, move, newBoard);
                int score = -QSearch(newBoard, -beta, -alpha);
                
                if (score > bigEval)
                {
                    bigEval = score;

                    if (score > alpha)
                    {
                        alpha = score;

                        if (score >= beta)
                        {
                            goto outer;
                        }
                    }
                }
            }
        }
        outer:

        return bigEval;
    }
}
