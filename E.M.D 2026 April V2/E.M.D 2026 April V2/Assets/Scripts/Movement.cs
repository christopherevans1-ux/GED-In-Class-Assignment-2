using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    //This is the main player script. It controls player movement, respawn, death
    // and ammo ui text

    public CharacterController controller;
    [Header("Player Values")]
    public float health = 10;
    public float speed = 12f;
    private float baseHealth = 10;
    public float gravity = -9.81f;
    public float jumpHeight = 3;
    [Header("Player Movement")]
    public GameObject Player;
    public Transform playerSpawnPoint;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Animator animator;
    public bool isWalking;
    [Header("Cameras/Body")]
    public GameObject deathCam;
    public GameObject playerCam;
    public GameObject playerBody;
    [Header("Game Text")]
    public Text loserText;
    public Text scoreText;
    public Text weaponSwitchText;
    //private floats for functions
    [Header("GUNZZZ")]
    public List<GameObject> weapons;
    public List<GameObject> FloorWeapons;
    private float magSizeForText;
    public bool holdingWeapon;
    [Header("Random Shit")]
    public bool victory = false;
    public bool isDead = false;
    public float score = 0;
    private float playerScore;
    private float playerInitialMagSize;
    private float playerMagSize;
    private int weaponNum;
    private bool isBirdsEye = false;
    public List<GameObject> theDead;
    [Header("UI")]
    public Canvas mainCanvas;
    public EnemyRadar enemyRadar;

    Vector3 velocity;
    [HideInInspector] public bool isGrounded;

    public void SetBirdsEyeMode(bool birdsEye)
    {
        isBirdsEye = birdsEye;
    }

    void Update()
    {
        weaponNum = GetComponent<WeaponPickup>().weaponNum;
        if (GameObject.FindGameObjectWithTag("Ragdoll") != null)
        {
            theDead.Add(GameObject.FindGameObjectWithTag("Ragdoll"));
        }
        Death();
        Respawn();
        if (PauseMenu.GameIsPaused) return;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isDead == false && victory == false)
        {
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move;
            if (isBirdsEye)
            {
                move = Vector3.right * x + Vector3.forward * z;
            }
            else
            {
                move = transform.right * x + transform.forward * z;
            }

            move = Vector3.ClampMagnitude(move, 1);
            controller.Move(move * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && isDead == false && victory == false)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
        animator.SetBool("isWalking", isWalking);
        if (weapons[0].active == true || weapons[1].active == true)
        {
            holdingWeapon = true;
            animator.SetBool("holdingWeapon", holdingWeapon);
            animator.SetFloat("Blend", 1);
        }
        else if (weapons[3].active == true)
        {
            holdingWeapon = true;
            animator.SetBool("holdingWeapon", holdingWeapon);
            animator.SetFloat("Blend", 2);
        }
        else if (weapons[2].active == true || weapons[4].active == true)
        {
            holdingWeapon = true;
            animator.SetBool("holdingWeapon", holdingWeapon);
            animator.SetFloat("Blend", 3);
        }
        else
        {
            holdingWeapon = false;
            animator.SetBool("holdingWeapon", holdingWeapon);
            animator.SetFloat("Blend", 0);
        }

        if (playerCam.GetComponent<PlayerCam>().isBirdsEye == false && holdingWeapon == true)
        {
            animator.SetFloat("Blend", 4);
        }
    }

    public void Death()
    {
        if (health <= 0)
        {
            for (int i = 0; i < theDead.Count; i++)
            {
                Destroy(theDead[i]);
            }
            animator.Rebind();
            isWalking = false;
            isDead = true;
            deathCam.transform.SetPositionAndRotation(Player.transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(90, 0, 0));
            playerCam.SetActive(false);
            playerBody.SetActive(false);
            deathCam.SetActive(true);
            if (enemyRadar != null) enemyRadar.gameObject.SetActive(false);
            if (mainCanvas != null) mainCanvas.worldCamera = deathCam.GetComponent<Camera>();
            loserText.text = "Press 'R' to restart";
        }
    }

    public void Respawn()
    {
        if (isDead == true && Input.GetKeyDown(KeyCode.R))
        {
            isDead = false;
            controller.enabled = false;

            controller.transform.position = playerSpawnPoint.position;
            controller.enabled = true;
            health = baseHealth;
            playerCam.SetActive(true);
            playerBody.SetActive(true);
            animator.Rebind();
            deathCam.SetActive(false);
            if (enemyRadar != null) enemyRadar.gameObject.SetActive(true);
            if (mainCanvas != null) mainCanvas.worldCamera = playerCam.GetComponent<Camera>();
            loserText.text = " ";
            bool floor1Cleared = GetComponent<EnemyRespawn>().floor1Cleared;
            DialogueManager.Instance.ResetAllTriggers(floor1Cleared);
            for (int i = 0; i < FloorWeapons.Count; i++)
            {
                if (FloorWeapons[i] != null)
                {
                    FloorWeapons[i].SetActive(true);
                    FloorWeapons[i].GetComponent<WeaponCleanUp>().relatedMagSize = FloorWeapons[i].GetComponent<WeaponCleanUp>().initialRMagSize;
                    FloorWeapons[i].GetComponent<WeaponCleanUp>().transform.position = FloorWeapons[i].GetComponent<WeaponCleanUp>().startPos;
                }
            }
            weapons[weaponNum].SetActive(false);
            if (weapons[weaponNum].GetComponent<MeleeWeapon>() == null)
            {
                weapons[weaponNum].GetComponent<PlayerShoot>().magSize = weapons[weaponNum].GetComponent<PlayerShoot>().initialMagSize;
            }
            GetComponent<WeaponPickup>().lastWeaponPickup = null;
            GetComponent<WeaponPickup>().currentWeapon = null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Floor2Checkpoint")
        {
            GetComponent<EnemyRespawn>().arrowToFloor2.SetActive(false);
            gameObject.GetComponent<EnemyRespawn>().floor1Cleared = true;
            playerSpawnPoint.position = other.transform.position;
        }
        if (other.name == "Death Barrier")
        {
            health = 0;
        }

        if (other.name == "LevelEndDoor" && GetComponent<EnemyRespawn>().levelCleared == true)
        {
            Debug.Log("Player has completed the level");
        }
    }
}