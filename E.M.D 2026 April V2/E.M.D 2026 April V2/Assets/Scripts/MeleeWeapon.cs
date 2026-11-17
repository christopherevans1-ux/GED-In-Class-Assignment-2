using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float attackSpeed; //animation time
    private float timer;
    public Animator animator;
    public GameObject player;
    public bool isSwing;
    public bool canSwing;
    public float damage;
    public AudioSource meleeSwing;
    public AudioSource meleeHit;
    public bool isFists;
    int checker = 0;
    Vector3 startTransform;
    public ParticleSystem meleeTrail;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        startTransform = gameObject.transform.localPosition;
        if (meleeTrail != null)
        {
            var emission = meleeTrail.emission;
            emission.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < player.GetComponent<Movement>().weapons.Count; i++)
        {
            if(player.GetComponent<Movement>().weapons[i].gameObject.active == true)
            {
                checker += 1;
            }
        }
        if (isFists == true && checker >= 1)
        {
            canSwing = false;
            animator.SetBool("canFist", canSwing);
            //this is to keep the fists in purgatory so they can't be used while using a weapon
            checker = 0;
        }
        else
        {
            if(gameObject.name == "mixamorig:RightHand" || gameObject.name == "mixamorig:LeftHand")
            {
                canSwing = true;
                animator.SetBool("canFist", canSwing);
            }
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime / attackSpeed;
        }
        if (Input.GetButton("Fire1") && timer <= 0 && isSwing == false && canSwing == true)
        {
            isSwing = true;
            animator.SetBool("isSwing", isSwing);
            meleeSwing.Play();
            Invoke("EndSwing", attackSpeed);
            timer = 1;
            if (meleeTrail != null)
            {
                var emission = meleeTrail.emission;
                emission.enabled = true;
                meleeTrail.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSwing == true && other.GetComponent<Enemy>() != null && other.GetComponent<Enemy>().isFatso == false)
        {
            other.GetComponent<Enemy>().health -= damage;
            meleeHit.PlayOneShot(meleeHit.clip);
        }
    }
    public void EndSwing()
    {
        isSwing = false;
        animator.SetBool("isSwing", isSwing);
        if (meleeTrail != null)
        {
            var emission = meleeTrail.emission;
            emission.enabled = false;
        }
    }
}
