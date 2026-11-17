using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float startHealth;
    public int scoreOnKill = 100;
    public float multiplier = 1f;
    public float playerScore = 0;
    public Vector3 enemyStartPos;
    public bool hasWeapon = false;
    public GameObject weaponPickupPrefab;
    public GameObject ragdollBody;
    private ComboUI comboUI;
    public bool isFatso;
    public float speed = 3;
    public GameObject player;

    public void Start()
    {
        player = GameObject.Find("Player");
        comboUI = FindObjectOfType<ComboUI>();
        startHealth = health;
        enemyStartPos = transform.position;
    }
    //Destroys the enemy gameObject if health reaches 0
    public void Update() 
    {
        //cow.AddForce(GetComponent<PlayerCam>().player.position * speed, ForceMode.Acceleration);
        if (health <= 0)
        {
            //ragdollBody.SetActive(true);
            GameObject deadMan = Instantiate(ragdollBody, gameObject.transform.position - new Vector3(0,0.5f,0), Quaternion.Euler(player.transform.rotation.eulerAngles - new Vector3(0, 180, 0)), GameObject.FindGameObjectWithTag("WorldObjectHolder").transform);
            deadMan.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * speed,ForceMode.Impulse);
            if (gameObject.GetComponent<BetterEnemy>() != null)
            {
                GetComponent<BetterEnemy>().enemyAnimator.Rebind();
                GetComponent<BetterEnemy>().weaponAnimator.Rebind();
            }
            gameObject.SetActive(false);
            if(hasWeapon == true)
            {
                GameObject pickup = Instantiate(weaponPickupPrefab, gameObject.transform.position, Quaternion.identity);
            }
            comboUI.RegisterKill();
            comboUI.baseScorePerKill += scoreOnKill;

        }

    }

   
}
