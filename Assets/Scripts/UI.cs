using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] TMP_Text evalScoreText;
    [SerializeField] TMP_Text nodesScoreText;
    [SerializeField] TMP_Text depthText;
    [SerializeField] TMP_Text moveText;

    public void updateEngineText(int evalScore, int nodesScore, string move)
    {
        if (evalScore <= -900000)
        {
            evalScoreText.fontSize = 25;
            int mateIn = evalScore + Helper.mateValue;
            mateIn -= 1;
            mateIn = mateIn == 0 ? -1 : mateIn;
            if (mateIn == -1) {
                evalScoreText.fontSize = 50;
                evalScoreText.text = "1-0";
            } else {
                evalScoreText.text = "White mates in " + mateIn + " ply";
            }
        }
        else if (evalScore >= 900000)
        {
            evalScoreText.fontSize = 25;
            int mateIn = (evalScore - Helper.mateValue) * -1;
            // Remove 1 because the engine counts the move it's making as a ply
            mateIn -= 1;
            mateIn = mateIn == 0 ? -1 : mateIn;
            if (mateIn == -1) {
                evalScoreText.fontSize = 50;
                evalScoreText.text = "0-1";
            } else {
                evalScoreText.text = "Black mates in " + mateIn + " ply";
            }
        }
        else
        {
            evalScoreText.fontSize = 50;
            evalScoreText.text = evalScore.ToString();
        }

        nodesScoreText.text = nodesScore.ToString();

        moveText.text = move;
    }
}
