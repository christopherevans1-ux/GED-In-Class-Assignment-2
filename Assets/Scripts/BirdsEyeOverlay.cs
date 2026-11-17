using System.Collections;
using UnityEngine;
using TMPro;

public class BirdsEyeOverlay : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource staticAudioSource;

    [Header("Timing")]
    public float maxDuration = 10f;
    public float effectStartDelay = 5f;
    public float cooldownDuration = 15f;

    [Header("Cooldown UI")]
    public TMP_Text cooldownText;

    public bool IsActive = false;
    public bool CooldownActive = false;
    public float Timer = 0f;
    public float CooldownTimer = 0f;

    private bool _hasShownTutorial = false;
    private EnemyRespawn _enemyRespawn;
    private float currentEffectT = 0f;
    private float audioFadeSpeed = 2f;
    private Movement movement;
    private bool wasDead = false;

    void Start()
    {
        _enemyRespawn = FindFirstObjectByType<EnemyRespawn>();
        movement = FindFirstObjectByType<Movement>();
        StaticDistortionFeature.IsEnabled = false;
        StaticDistortionFeature.Intensity = 0f;

        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = "Birds Eye View: <color=#00B400>Ready</color>";
            cooldownText.color = Color.white;
        }

        if (staticAudioSource != null)
        {
            staticAudioSource.loop = true;
            staticAudioSource.volume = 0f;
            staticAudioSource.Stop();
        }
    }

    void Update()
    {
        if (movement != null)
        {
            bool isDead = movement.isDead;
            if (isDead && !wasDead)
            {
                CooldownActive = false;
                CooldownTimer = 0f;
                IsActive = false;
                currentEffectT = 0f;
                StaticDistortionFeature.IsEnabled = false;
                StaticDistortionFeature.Intensity = 0f;
                if (staticAudioSource != null)
                {
                    staticAudioSource.volume = 0f;
                    staticAudioSource.Stop();
                }
                if (cooldownText != null)
                {
                    cooldownText.text = "Birds Eye View: <color=#00B400>Ready</color>";
                    cooldownText.color = Color.white;
                }
            }

            if (wasDead && !isDead)
            {
                CooldownActive = false;
                CooldownTimer = 0f;
                if (cooldownText != null)
                {
                    cooldownText.text = "Birds Eye View: <color=#00B400>Ready</color>";
                    cooldownText.color = Color.white;
                }
            }

            wasDead = isDead;
        }

        if (CooldownActive)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer <= 0f)
            {
                CooldownTimer = 0f;
                CooldownActive = false;
                if (cooldownText != null)
                {
                    cooldownText.text = "Birds Eye View: <color=#00B400>Ready</color>";
                    cooldownText.color = Color.white;
                }
            }
            else
            {
                if (cooldownText != null)
                {
                    cooldownText.text = "Birds Eye View: " + Mathf.CeilToInt(CooldownTimer) + "s";
                    cooldownText.color = Color.white;
                }
            }
        }

        if (IsActive)
        {
            Timer += Time.deltaTime;

            float effectTime = Mathf.Max(0f, Timer - effectStartDelay);
            float effectDuration = maxDuration - effectStartDelay;
            float t = Mathf.Clamp01(effectTime / effectDuration);

            currentEffectT = t;
            StaticDistortionFeature.IsEnabled = true;
            StaticDistortionFeature.Intensity = t;

            if (staticAudioSource != null)
                staticAudioSource.volume = Mathf.Lerp(0f, 0.42f, t);

            if (Timer >= maxDuration)
                ForceEnd();
        }
        else
        {
            currentEffectT = Mathf.Lerp(currentEffectT, 0f, Time.deltaTime * audioFadeSpeed);
            StaticDistortionFeature.Intensity = currentEffectT;

            if (currentEffectT < 0.01f)
                StaticDistortionFeature.IsEnabled = false;

            if (staticAudioSource != null)
            {
                staticAudioSource.volume = Mathf.Lerp(staticAudioSource.volume, 0f, Time.deltaTime * audioFadeSpeed);
                if (staticAudioSource.volume < 0.01f && staticAudioSource.isPlaying)
                    staticAudioSource.Stop();
            }
        }
    }

    public void StartBirdsEye()
    {
        IsActive = true;
        Timer = 0f;
        if (staticAudioSource != null && !staticAudioSource.isPlaying)
            staticAudioSource.Play();

        if (!_hasShownTutorial && _enemyRespawn != null && _enemyRespawn.isTutorialLevel)
        {
            _hasShownTutorial = true;
            StartCoroutine(ShowTabTipWhenReady());
        }
    }

    private IEnumerator ShowTabTipWhenReady()
    {
        while (DialogueManager.Instance.IsPlaying)
            yield return null;

        DialogueManager.Instance.ShowDialogue("You can press TAB again to cancel the birds-eye effect.", 2.5f);
    }

    public void EndBirdsEye()
    {
        IsActive = false;
        StartCooldown();
    }

    private void ForceEnd()
    {
        IsActive = false;
        StartCooldown();
        CameraToggle cameraToggle = FindFirstObjectByType<CameraToggle>();
        if (cameraToggle != null)
            cameraToggle.ForceReturnToFirstPerson();
    }

    private void StartCooldown()
    {
        CooldownActive = true;

        float halfwayPoint = effectStartDelay + (maxDuration - effectStartDelay) * 0.5f;
        float usageFraction = Mathf.Clamp01(Timer / halfwayPoint);
        CooldownTimer = Mathf.Lerp(effectStartDelay, cooldownDuration, usageFraction);
    }
}