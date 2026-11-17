using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door;
    public float openRot, closeRot, speed;
    public bool opening;
    private float timer;

    void Update()
    {
        Vector3 currentRot = door.transform.localEulerAngles;
        if(opening)
        {
            if(currentRot.y < openRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, openRot, currentRot.z), speed * Time.deltaTime);
            }
        }
        else
        {
            if(currentRot.y > closeRot)
            {
                door.transform.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, closeRot, currentRot.z), speed * Time.deltaTime);
            }
        }

   

    }
    public void OnTriggerEnter(Collider other)
    {
        Vector3 currentRot = door.transform.localEulerAngles;
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") && currentRot.y < openRot)
        {
            Debug.Log("hit door");
            opening = true;
            currentRot = door.transform.localEulerAngles;
        }
        if (other.gameObject.tag == "Player" /*|| other.gameObject.tag == "Enemy"*/ && currentRot.y > closeRot)
        {
            Debug.Log("Close Door");
            opening = false;
            currentRot = door.transform.localEulerAngles;
        }
    }
}
