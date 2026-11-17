using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public RawImage dialogueBox;
    public TMP_Text dialogueText;

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.04f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    private Coroutine _activeCoroutine;
    private Coroutine _hideCoroutine;
    private bool _isPersistent = false;
    private bool _typingComplete = false;
    private bool _exitPending = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueBox.enabled = false;
        dialogueText.enabled = false;
    }

    public void ResetAllTriggers(bool floor1Cleared)
    {
        DialogueTrigger[] allTriggers = FindObjectsOfType<DialogueTrigger>(true);

        foreach (var trigger in allTriggers)
        {
            trigger.ResetTrigger(floor1Cleared);
        }

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);

        _isPersistent = false;
        _typingComplete = false;
        _exitPending = false;
        dialogueBox.enabled = false;
        dialogueText.enabled = false;
    }

    public void ShowDialogue(string text, float endDelay = 2f)
    {
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);

        _isPersistent = false;
        _typingComplete = false;
        _exitPending = false;
        _activeCoroutine = StartCoroutine(PlayDialogue(text, endDelay));
    }

    public void ShowPersistentDialogue(string text)
    {
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);

        _isPersistent = true;
        _typingComplete = false;
        _exitPending = false;
        _activeCoroutine = StartCoroutine(PlayPersistentDialogue(text));
    }

    public void EndPersistentDialogue()
    {
        if (!_isPersistent) return;

        if (_typingComplete)
        {
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay(1f));
        }
        else
        {
            _exitPending = true;
        }
    }

    public void CancelEndPersistentDialogue()
    {
        if (!_isPersistent) return;

        _exitPending = false;

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }
    }

    private IEnumerator PlayDialogue(string text, float endDelay)
    {
        dialogueText.text = "";
        dialogueBox.enabled = true;
        dialogueText.enabled = true;

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            if (typingSound != null && letter != ' ')
                audioSource.PlayOneShot(typingSound);
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(endDelay);

        dialogueBox.enabled = false;
        dialogueText.enabled = false;
        _activeCoroutine = null;
    }

    private IEnumerator PlayPersistentDialogue(string text)
    {
        dialogueText.text = "";
        dialogueBox.enabled = true;
        dialogueText.enabled = true;

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            if (typingSound != null && letter != ' ')
                audioSource.PlayOneShot(typingSound);
            yield return new WaitForSeconds(typingSpeed);
        }

        _typingComplete = true;
        _activeCoroutine = null;

        if (_exitPending)
        {
            _exitPending = false;
            _hideCoroutine = StartCoroutine(HideAfterDelay(1f));
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        _isPersistent = false;
        yield return new WaitForSeconds(delay);

        dialogueBox.enabled = false;
        dialogueText.enabled = false;
        _hideCoroutine = null;
    }
}