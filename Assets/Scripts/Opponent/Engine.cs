using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    readonly int posInfinity = 999999;
    readonly int negInfinity = -999999;
    public int nodesVisited;
    public int maxDepth;

    [SerializeField] Board board;
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
        (Vector2, Vector2) noMove = (new Vector2(-1, -1), new Vector2(-1, -1));

        if (depth == 0)
        {
            return (evaluation.Evaluate(oldBoard, oldBoard.sideToMove), noMove);
        }

        int movesDone = 0;
        int bigEval = negInfinity;
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        List<(Vector2, List<Vector2>)> allMoves = moveGenerator.GenerateAllMoves(oldBoard);
        foreach (var (position, moves) in allMoves)
        {
            foreach (var move in moves)
            {
                // Make the move and call Negamax recursively
                BoardData newBoard = oldBoard.DeepCopy();
                board.TestMove(position, move, newBoard);
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
            if (board.CheckChecks(oldBoard))
            {
                return (negInfinity + ply, noMove);
            }
            // Stalemate
            else
            {
                return (0, noMove);
            }
        }

        return (bigEval, bestMove);
    }
}
