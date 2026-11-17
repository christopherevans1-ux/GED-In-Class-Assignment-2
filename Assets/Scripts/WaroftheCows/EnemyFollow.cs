using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{

    bool lookAt = false;
    public GameObject player;
    Rigidbody rb;
    public float multiplier = 10;
    public float speed_limit = 2;
    public float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerDetection.found)
        {
            lookAt = true;
        }
        if (lookAt)
        {
            transform.LookAt(player.transform);
            Vector3 vel = rb.velocity;
            if(!PlayerDetection.found && vel.x > -2 && vel.x < 2 && vel.z > -2 && vel.z < 2)
            {
                rb.AddForce(speed * multiplier * Time.deltaTime * transform.forward);
            }
        }
    }
}
