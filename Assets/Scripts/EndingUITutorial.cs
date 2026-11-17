using UnityEngine;
using TMPro;

public class EndingUITutorial : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public TMP_Text finalTimeText;

    void Start()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        float time = PlayerPrefs.GetFloat("FinalTime", 0f);
        finalScoreText.text = "Final Score: " + score + " pts";
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        finalTimeText.text = string.Format(
            "Final Time: {0:00}:{1:00}:{2:00}",
            minutes, seconds, milliseconds
        );
    }
}