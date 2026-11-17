using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;         
using System.Collections.Generic;

public class ComboUI : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private float comboDuration = 6f;
    [SerializeField] public int baseScorePerKill = 100;

    private bool hasSavedScore = false;
    private Movement movement;
    private EnemyRespawn respawn;
    private float comboTimer;
    private int comboMultiplier = 1;
    private bool comboActive = false;
    private float popIntensity = 1f;
    private GameObject player;


    [Header("UI References")]
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image comboTimerBar;
    [SerializeField] private GameObject comboRoot;

    [Header("Animations")]
    [SerializeField] private Animator comboAnimator;
    [SerializeField] private Animator scoreAnimator;

    public int totalScore = 0;
    public int savedPlayerScore;

    void Start()

    {
        comboRoot.SetActive(false);
        player = GameObject.Find("Player");
        ResetComboUI();
        movement = player.GetComponent<Movement>();
        respawn = player.GetComponent<EnemyRespawn>();
    }

    void Update()
    {
        if (comboActive)
        {
            comboTimer -= Time.deltaTime;
            comboTimerBar.fillAmount = comboTimer / comboDuration;

            if (comboTimer <= 0f)
                EndCombo();

            if (movement.isDead == true && Input.GetKeyDown(KeyCode.R))
                EndCombo();
        }
        if (respawn.floor1Cleared && !hasSavedScore)
        {
            hasSavedScore = true;

            savedPlayerScore = totalScore;

            PlayerPrefs.SetInt("FinalScore", savedPlayerScore);
        }
        if (movement.isDead == true && savedPlayerScore != 0)
        {
            totalScore = savedPlayerScore;
            scoreText.text = totalScore.ToString() + "pts";
        }
        else if (movement.isDead == true)
        {
            totalScore = 0;
            scoreText.text = totalScore.ToString() + "pts";
            EndCombo();
        }
    }
    private IEnumerator ShakeUI(RectTransform target, float amount, float duration)
    {
        Vector3 originalPos = target.anchoredPosition;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float shakeX = Random.Range(-amount, amount);
            float shakeY = Random.Range(-amount, amount);

            target.anchoredPosition = originalPos + new Vector3(shakeX, shakeY, 0);

            yield return null;
        }

        target.anchoredPosition = originalPos;
    }

    public void RegisterKill()
    {
        if (!comboActive)
        {
            comboActive = true;
            comboMultiplier = 1;

            comboRoot.SetActive(true);
        }
        else
        {
            comboMultiplier += 1; 
        }

        
        comboTimer = comboDuration;

        UpdateComboUI();

        int pointsGained = baseScorePerKill * comboMultiplier;
        totalScore += pointsGained;
        scoreText.text = totalScore.ToString() + "pts";

        popIntensity = 1f + (comboMultiplier * 0.1f);
        popIntensity = Mathf.Clamp(popIntensity, 1f, 3f);
        comboText.transform.localScale = Vector3.one * popIntensity;

        
        comboAnimator.SetTrigger("Pop");
        //scoreAnimator.SetTrigger("Pop");
        float shakeAmount = 3f + comboMultiplier * 0.5f;
        StartCoroutine(ShakeUI(comboText.rectTransform, shakeAmount, 0.15f));
        shakeAmount = Mathf.Clamp(shakeAmount, 3f, 20f);
    }

    private void UpdateComboUI()
    {
        comboText.text = "x" + comboMultiplier.ToString();
        comboTimerBar.fillAmount = 1f;
    }

    public void EndCombo()
    {
        comboRoot.SetActive(false);
        comboActive = false;
        comboMultiplier = 1;
        ResetComboUI();
    }

    private void ResetComboUI()
    {
        comboText.text = "x1";
        comboTimerBar.fillAmount = 0f;
    }
}
