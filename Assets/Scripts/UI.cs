using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] TMP_Text evalScoreText;
    [SerializeField] TMP_Text nodesScoreText;

    public void updateEngineText(int evalScore, int nodesScore)
    {
        if (evalScore == int.MaxValue)
        {
            evalScoreText.text = "White Checkmate";
        }
        else if (evalScore == int.MinValue)
        {
            evalScoreText.text = "Black Checkmate";
        }
        else
        {
            evalScoreText.text = evalScore.ToString();
        }

        nodesScoreText.text = nodesScore.ToString();
    }
}
