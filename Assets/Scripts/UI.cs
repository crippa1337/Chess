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
            int mateIn = (-999999 + evalScore) / 2;
            mateIn *= -1;
            mateIn = mateIn == 0 ? 1 : mateIn;
            evalScoreText.text = "WM" + mateIn;
        }
        else if (evalScore >= 900000)
        {
            int mateIn = (999999 - evalScore) / 2;
            mateIn = mateIn == 0 ? 1 : mateIn;
            evalScoreText.text = "BM" + mateIn;
        }
        else
        {
            evalScoreText.text = evalScore.ToString();
        }

        nodesScoreText.text = nodesScore.ToString();

        moveText.text = move;
    }
}
