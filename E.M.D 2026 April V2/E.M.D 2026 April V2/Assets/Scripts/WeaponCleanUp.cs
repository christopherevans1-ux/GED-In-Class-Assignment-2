using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCleanUp : MonoBehaviour
{
    public bool AIDropped = false;
    GameObject Player;
    //these are for when the weapon is thrown
    Vector3 lastPosition;
    public Vector3 startPos;
    Rigidbody rb;
    float speed;
    public float contactDamage;
    public float relatedMagSize;
    public float initialRMagSize;
    public bool isMelee;
    public AudioSource throwContact;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        startPos = this.gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.GetComponent<Movement>().isDead == true && AIDropped == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>() != null && other.GetComponent<Enemy>().isFatso == false)
        {
            if (rb.velocity.magnitude >= 1.5f)
            {
                other.GetComponent<Enemy>().health -= contactDamage;
                throwContact.PlayOneShot(throwContact.clip);
            }

        }
    }
}
