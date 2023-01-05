using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] TMP_Text evalScoreText;

    public void updateEngineText(int evalScore)
    {
        evalScoreText.text = evalScore.ToString();
    }
}
