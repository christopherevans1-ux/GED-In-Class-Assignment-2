using System.Collections;
using UnityEngine;

public class TutorialIntro : MonoBehaviour
{
    private Movement _movement;

    void Start()
    {
        _movement = GetComponent<Movement>();
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.2f);

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager instance is null");
            yield break;
        }

        _movement.victory = true;

        yield return StartCoroutine(DialogueManager.Instance.PlayTutorialDialogue(
            "Good, you made it. There is a building ahead of you. Get inside — You have some business to attend to."
        ));

        _movement.victory = false;
    }
}