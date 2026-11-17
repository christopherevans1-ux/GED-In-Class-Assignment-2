using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloryKill : MonoBehaviour
{
    public GameObject player;
    public bool inRangeofEnemy;
    public bool isKilling;
    GameObject currentEnemy;
    public bool inAnimation;
    GameObject currentWeaponEquipped;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        currentWeaponEquipped = player.GetComponent<Movement>().weapons[player.GetComponent<WeaponPickup>().weaponNum];
        if(inRangeofEnemy && Input.GetKeyDown(KeyCode.LeftShift)){
            playerKillAnimation();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            inRangeofEnemy = true;
            currentEnemy = collision.gameObject;
        }

    }

    public void playerKillAnimation()
    {
        player.GetComponent<Movement>().victory = true;
        animator.SetBool("isKilling", isKilling);
        //add a blend tree for this maybe?
        if(currentWeaponEquipped.name == "Baseball Bat")
        {
            animator.SetFloat("GloryKill", 1);
        }
        else
        {
            GetComponent<WeaponThrow>().ThrowWeapon();
            animator.SetFloat("GloryKill", 0);
        }
        currentEnemy.GetComponent<Enemy>().health = 0;
    }
}
