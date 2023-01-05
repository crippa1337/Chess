using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    readonly int infinity = int.MaxValue;
    readonly int negInfinity = int.MinValue;

    [SerializeField] Board board;
    Evaluation evaluation;

    private void Start()
    {
        evaluation = GetComponent<Evaluation>();
    }

    public int MiniMax(PieceData[,] pieces, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (depth == 0)
        {
            return evaluation.Evaluate(pieces);
        }

        if (isMaximizing)
        {
            int maxEval = negInfinity;
            List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(pieces, 1);
            foreach ((Vector2, List<Vector2>) pieceMoves in allMoves)
            {
                foreach (Vector2 move in pieceMoves.Item2)
                {
                    Vector2 from = pieceMoves.Item1;
                    PieceData[,] testPieces = board.DeepCopy(pieces);
                    if (board.TestMove(from, move, 1, testPieces))
                    {
                        int eval = MiniMax(testPieces, depth - 1, alpha, beta, false);
                        maxEval = Mathf.Max(maxEval, eval);
                        alpha = Mathf.Max(alpha, eval);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = infinity;
            List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(pieces, -1);
            foreach ((Vector2, List<Vector2>) pieceMoves in allMoves)
            {
                foreach (Vector2 move in pieceMoves.Item2)
                {
                    Vector2 from = pieceMoves.Item1;
                    PieceData[,] testPieces = board.DeepCopy(pieces);
                    if (board.TestMove(from, move, -1, testPieces))
                    {
                        int eval = MiniMax(testPieces, depth - 1, alpha, beta, true);
                        minEval = Mathf.Min(minEval, eval);
                        beta = Mathf.Min(beta, eval);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return minEval;
        }
    }

    public (Vector2, Vector2) MaxMove(PieceData[,] pieces, int depth)
    {
        int maxEval = negInfinity;
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(pieces, 1);
        foreach ((Vector2, List<Vector2>) pieceMoves in allMoves)
        {
            foreach (Vector2 move in pieceMoves.Item2)
            {
                Vector2 from = pieceMoves.Item1;
                PieceData[,] testPieces = board.DeepCopy(pieces);
                if (board.TestMove(from, move, 1, testPieces))
                {
                    int eval = MiniMax(testPieces, depth, negInfinity, infinity, false);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = (from, move);
                    }
                }
            }
        }

        return bestMove;
    }

    public ((Vector2, Vector2), int) MinMove(PieceData[,] pieces, int depth)
    {
        int minEval = infinity;
        (Vector2, Vector2) bestMove = (Vector2.zero, Vector2.zero);
        List<(Vector2, List<Vector2>)> allMoves = MoveGenerator.GenerateAllMoves(pieces, -1);
        foreach ((Vector2, List<Vector2>) pieceMoves in allMoves)
        {
            foreach (Vector2 move in pieceMoves.Item2)
            {
                Vector2 from = pieceMoves.Item1;
                PieceData[,] testPieces = board.DeepCopy(pieces);
                if (board.TestMove(from, move, -1, testPieces))
                {
                    int eval = MiniMax(testPieces, depth, negInfinity, infinity, true);
                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = (pieceMoves.Item1, move);
                    }
                }
            }
        }

        return (bestMove, minEval);
    }
}
