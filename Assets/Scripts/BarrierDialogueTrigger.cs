using UnityEngine;

public class BarrierDialogueTrigger : MonoBehaviour
{
    private EnemyRespawn _enemyRespawn;
    private bool _playerInside = false;

    void Start()
    {
        _enemyRespawn = FindObjectOfType<EnemyRespawn>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_enemyRespawn == null || _enemyRespawn.floor1Cleared) return;

        _playerInside = true;
        DialogueManager.Instance.CancelEndPersistentDialogue();
        DialogueManager.Instance.ShowPersistentDialogue("You still have unfinished business on this floor.");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInside = false;
        DialogueManager.Instance.EndPersistentDialogue();
    }

    void Update()
    {
        if (_enemyRespawn == null) return;

        if (_playerInside && _enemyRespawn.floor1Cleared)
            _playerInside = false;

        if (_enemyRespawn.floor1Cleared)
            gameObject.SetActive(false);
    }
}