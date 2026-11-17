using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private BetterDoor door;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player" || other.gameObject.tag == "Enemy")
        {
            if (!door.isOpen)
            {
                door.Open(other.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player" || other.gameObject.tag == "Enemy")
        {
            if (door.isOpen)
            {
                door.Close();
            }
        }
    }
}
