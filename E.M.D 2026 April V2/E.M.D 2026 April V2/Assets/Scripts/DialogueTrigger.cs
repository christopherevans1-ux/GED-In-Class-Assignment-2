using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string dialogueText = "Enter your dialogue here...";

    [Header("Floor")]
    public int floorNumber = 1;

    private bool _hasTriggered = false;
    private Collider _col;

    void Start()
    {
        _col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;
        _col.enabled = false;
        DialogueManager.Instance.ShowDialogue(dialogueText);
    }

    public void ResetTrigger(bool floor1Cleared)
    {
        if (floorNumber == 1 && floor1Cleared) return;

        _hasTriggered = false;
        _col.enabled = true;
    }
}