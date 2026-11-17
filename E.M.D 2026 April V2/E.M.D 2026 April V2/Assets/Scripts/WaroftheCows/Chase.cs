using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;
    public float damage;

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            //Destroy(other.gameObject);
            other.GetComponent<Movement>().health -= damage;
            Debug.Log("i touched the player");
        }
    }
}
