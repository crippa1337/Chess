using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static float startSeconds = 600;
    [SerializeField] GameObject selector;
    private RectTransform selectorPos;

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ChangeTime(float time)
    {
        if (selectorPos == null)
        {
            selectorPos = selector.GetComponent<RectTransform>();
        }

        if (time != startSeconds)
        {
            startSeconds = time;
            
            // Blitz
            if (time == 180)
            {
                selectorPos.anchoredPosition = new Vector2(-375, -327);
            }

            // Rapid
            if (time == 600)
            {
                selectorPos.anchoredPosition = new Vector2(0, -327);
            }

            // Classic
            if (time == 1800)
            {
                selectorPos.anchoredPosition = new Vector2(375, -327);
            }
        }
    }
}
