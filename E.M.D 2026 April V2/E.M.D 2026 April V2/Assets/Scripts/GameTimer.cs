using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public Text timerText;

    [Header("Player")]
    public string playerTag = "Player";

    private float timeElapsed = 0f;
    private bool timerRunning = true;
    private bool hasFinished = false;

    void Update()
    {
        if (timerRunning)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);
        int milliseconds = Mathf.FloorToInt((timeElapsed * 100f) % 100f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasFinished)
        {
            hasFinished = true;
            timerRunning = false;

            PlayerPrefs.SetFloat("FinalTime", timeElapsed);

            UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
        }
    }
}