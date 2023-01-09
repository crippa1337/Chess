using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public int nodesVisited;
    public int maxDepth;

    [SerializeField] Board gameBoard;
    [SerializeField] MoveGenerator moveGenerator;
    Evaluation evaluation;

    private void Start()
    {
        evaluation = GetComponent<Evaluation>();
    }

    public (int, Move) Negamax(BoardData oldBoard, int depth, int alpha, int beta)
    {

        if (depth == 0)
        {
            //return (QSearch(oldBoard, alpha, beta), Helper.noneMove);
            return (evaluation.Evaluate(oldBoard, oldBoard.sideToMove), Helper.noneMove);
        }

        int ply = maxDepth - depth;
        int movesDone = 0;
        int bigEval = Helper.negInfinity;
        Move bestMove = Helper.noneMove;
        List<Move> allMoves = moveGenerator.GenerateLegalMoves(oldBoard);

        for (int i = 0; i < allMoves.Count; i++)
        {
            nodesVisited++;
            Move move = allMoves[i];

            // Make the move and call Negamax recursively
            BoardData newBoard = oldBoard.DeepCopy();
            gameBoard.TestMove(move, newBoard);
            movesDone++;
            int eval = -Negamax(newBoard, depth - 1, -beta, -alpha).Item1;
            if (eval > bigEval)
            {
                bigEval = eval;
                bestMove = move;
            }

            alpha = Mathf.Max(alpha, eval);
            if (alpha >= beta)
            {
                break;
            }
        }
        
        if (movesDone == 0)
        {
            // Checkmate
            if (gameBoard.CheckChecks(oldBoard))
            {
                return (Helper.MatedIn(ply), Helper.noneMove);
            }
            // Stalemate
            else
            {
                return (0, Helper.noneMove);
            }
        }

        return (bigEval, bestMove);
    }

    int QSearch(BoardData board, int alpha, int beta)
    {
        int bigEval = evaluation.Evaluate(board, board.sideToMove);
        if (bigEval >= beta)
        {
            return beta;
        }
        if (bigEval > alpha)
        {
            alpha = bigEval;
        }

        List<Move> allMoves = moveGenerator.GenerateLegalCaptures(board);

        for (int i = 0; i < allMoves.Count; i++)
        {
            nodesVisited++;
            Move move = allMoves[i];

            // Delta pruning
            bool isPromotion = false;
            PieceData fromPiece = board.pieces[(int)move.from.x, (int)move.from.y];
            PieceData captured = board.pieces[(int)move.to.x, (int)move.to.y];
            if (fromPiece.type == PieceData.Type.Pawn)
            {
                if (board.sideToMove == 1 && move.to.y == 7)
                {
                    isPromotion = true;
                } else if (board.sideToMove == -1 && move.to.y == 0)
                {
                    isPromotion = true;
                }
            }

            if (captured.value + 400 + bigEval < alpha && !isPromotion)
            {
                continue;
            }

            // Make the move and call Negamax recursively
            BoardData newBoard = board.DeepCopy();
            gameBoard.TestMove(move, newBoard);
            int score = -QSearch(newBoard, -beta, -alpha);

            if (score > bigEval)
            {
                bigEval = score;

                if (score > alpha)
                {
                    alpha = score;

                    if (score >= beta)
                    {
                        break;
                    }
                }
            }
        }

        return bigEval;
    }
}
