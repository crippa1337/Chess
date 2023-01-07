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
        if (evalScore == -999999)
        {
            evalScoreText.text = "W Mate";
        }
        else if (evalScore == 999999)
        {
            evalScoreText.text = "B Mate";
        }
        else
        {
            evalScoreText.text = evalScore.ToString();
        }

        nodesScoreText.text = nodesScore.ToString();

        moveText.text = move;
    }
}
