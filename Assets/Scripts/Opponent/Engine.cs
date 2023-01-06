using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    readonly int posInfinity = int.MaxValue;
    readonly int negInfinity = int.MinValue;
    int nodesScore;

    [SerializeField] Board board;
    Evaluation evaluation;

    private void Start()
    {
        evaluation = GetComponent<Evaluation>();
    }

    public int Negamax(BoardData oldBoard, int depth, int alpha, int beta, int caller)
    {
        nodesScore++;
        if (depth == 0)
        {
            return evaluation.Evaluate(oldBoard, caller);
        }
        
        int maxEval = negInfinity;
        List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(oldBoard, caller);
        foreach (var (position, moves) in allMoves)
        {
            foreach (var move in moves)
            {
                // Make the move and call Negamax recursively
                BoardData newBoard = oldBoard.DeepCopy();
                if (board.SilentMove(position, move, caller, newBoard))
                {
                    int eval = -Negamax(newBoard, depth - 1, -beta, -alpha, -caller);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
            }
        }

        return maxEval;
    }


    public ((Vector2, Vector2), int, int) MinMove(BoardData oldBoard, int depth)
    {
        nodesScore = 0;
        int bestEval = posInfinity;
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(oldBoard, -1);
        
        // find the best move for the minimizer
        foreach (var (position, moves) in allMoves)
        {
            foreach (var move in moves)
            {
                // Make the move and call Negamax recursively
                BoardData newBoard = oldBoard.DeepCopy();
                if (board.SilentMove(position, move, -1, newBoard))
                {
                    int eval = Negamax(newBoard, depth, negInfinity, posInfinity, 1);
                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestMove = (position, move);
                    }
                }

            }

        }

        return (bestMove, bestEval, nodesScore);
    }
}


    //public (Vector2, Vector2) MaxMove(PieceData[,] pieces, int depth)
    //{
    //    int maxEval = negInfinity;
    //    (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
    //    List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(pieces, 1);
    //    foreach ((Vector2, List<Vector2>) pieceMoves in allMoves)
    //    {
    //        foreach (Vector2 move in pieceMoves.Item2)
    //        {
    //            Vector2 from = pieceMoves.Item1;
    //            PieceData[,] testPieces = board.DeepCopy(pieces);
    //            if (board.TestMove(from, move, 1, testPieces))
    //            {
    //                int eval = MiniMax(testPieces, depth, negInfinity, infinity, false);
    //                if (eval > maxEval)
    //                {
    //                    maxEval = eval;
    //                    bestMove = (from, move);
    //                }
    //            }
    //        }
    //    }

    //    return bestMove;
    //}
