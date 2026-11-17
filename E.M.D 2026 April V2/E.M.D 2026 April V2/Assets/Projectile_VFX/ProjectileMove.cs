using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public GameObject hitPref;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        speed = 0;
        
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up,contact.normal);
        Vector3 pos = contact.point;
        if(hitPref != null)
        {
            var hitVFX = Instantiate(hitPref, pos, rot);
        }
        Destroy(gameObject);
    }
}
