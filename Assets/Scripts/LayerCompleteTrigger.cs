using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCompleteTrigger : MonoBehaviour
{
    public bool layerCompleted = false;
    public bool playerSpawnSecondLayer = false;
    public int savedPlayerScore;
    public GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (layerCompleted)
        {
            layerCompleted = true;
            playerSpawnSecondLayer = true;
            savedPlayerScore = player.GetComponent<ComboUI>().totalScore;

        }
    }
}
