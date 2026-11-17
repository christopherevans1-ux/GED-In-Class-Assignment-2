using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerShoot : MonoBehaviour
{
    [Header("Bullet Variables")]
    public float bulletSpeed;
    public float fireRate, bulletDamage;
    public float initialMagSize;
    public float magSize = 0f;
    public bool isAuto;
    public float reloadTime;
    public float reloadStart = 0;
    public AudioSource gunShot;
    public AudioSource reloadSound;
    bool playerDead;
    public GameObject Player;
    public Animator animator;
    bool isReloading;
    bool canShoot = true;

    [Header("Bullet Spawn")]
    public List<Transform> bulletSpawnTransform;
    public GameObject bulletPrefab;

    [Header("VFX Variables")]
    public VisualEffect muzzleFlash;
    public Color secondaryColor;
    public Vector3 velocity;
    private VFXEventAttribute eventAttribute;
    private float timer;

    [Header("Hitscan & Impact VFX")]
    public float weaponRange = 100f;
    public GameObject hitPref;
    public GameObject bloodBurst;

    [Header("Ammo UI")]
    public Text rifleAmmoText;
    public Text pistolAmmoText;
    public Text shotgunAmmoText;

    [HideInInspector] public bool freezeAmmoText = false;

    private void Start()
    {
        isReloading = false;
        initialMagSize = magSize;
        eventAttribute = muzzleFlash.CreateVFXEventAttribute();
    }

    private void Update()
    {
        if (animator != null)
            animator.SetBool("isWalking", Player.GetComponent<Movement>().isWalking);

        ResetAmmoOnRespawn();
        Ammo();

        if (PauseMenu.GameIsPaused) return;

        playerDead = Player.GetComponent<Movement>().isDead;

        if (timer > 0)
            timer -= Time.deltaTime / fireRate;

        if (isAuto)
        {
            if (Input.GetButton("Fire1") && timer <= 0 && magSize > 0 && playerDead == false && canShoot)
            {
                ExecuteShoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && timer <= 0 && magSize > 0 && playerDead == false && canShoot)
            {
                ExecuteShoot();
            }
        }
    }

    private void ExecuteShoot()
    {
        gunShot.PlayOneShot(gunShot.clip);
        Shoot();
        muzzleFlash.Play();
        if (isAuto) animator.SetBool("isWalking", false);

        timer = 1f;
        magSize--;
    }

    public void Shoot()
    {
        for (int i = 0; i < bulletSpawnTransform.Count; i++)
        {
            if (bulletSpawnTransform[i].gameObject.activeSelf == false) continue;

            //First, fire the line of fire. Then, add `QueryTriggerInteraction.Ignore` at the end to force it to penetrate the bullet you set as a Trigger.
            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnTransform[i].position, bulletSpawnTransform[i].forward, out hit, weaponRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 spawnPos = hit.point + (hit.normal * 0.05f);
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (bloodBurst != null)
                    {
                        GameObject bloodVFX = Instantiate(bloodBurst, spawnPos, Quaternion.identity);
                        VisualEffect vfxComponent = bloodVFX.GetComponent<VisualEffect>();

                    }

                    Enemy targetEnemy = hit.collider.GetComponent<Enemy>();
                    if (targetEnemy != null)
                    {
                        if (hit.collider.name == "EnemyHead") targetEnemy.health -= bulletDamage * 2;
                        else targetEnemy.health -= bulletDamage;
                    }
                }
                else if (hitPref != null)
                {
                    Quaternion wallRot = Quaternion.LookRotation(hit.normal);
                    Instantiate(hitPref, spawnPos, wallRot);
                }
            }

            //After the ray instantly calculates the damage, the visual bullet is generated and sent flying, perfectly avoiding it.
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnTransform[i].position, bulletSpawnTransform[i].rotation, GameObject.FindGameObjectWithTag("WorldObjectHolder").transform);
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnTransform[i].forward * bulletSpeed, ForceMode.Impulse);
        }
        PlayVFX();
    }

    public void ResetAmmoOnRespawn()
    {
        if (playerDead == true && Input.GetKeyDown(KeyCode.R))
            magSize = initialMagSize;
    }

    private void PlayVFX()
    {
        eventAttribute.SetFloat("size", Random.Range(0f, 1f));
        eventAttribute.SetVector3("velocity", velocity);
        eventAttribute.SetVector3("secondaryColor", new Vector3(secondaryColor.r, secondaryColor.g, secondaryColor.b));
        muzzleFlash.SendEvent("OnPlay", eventAttribute);
        muzzleFlash.Play(eventAttribute);
    }

    public void Ammo()
    {
        if (freezeAmmoText) return;

        if (Player.GetComponent<EnemyRespawn>().levelCleared != true || Player.GetComponent<Movement>().health > 0)
        {
            string ammoStr = ((int)magSize).ToString();
            Color ammoColor = magSize <= 0 ? Color.red : Color.white;

            if (rifleAmmoText != null) { rifleAmmoText.text = ammoStr; rifleAmmoText.color = ammoColor; }
            if (pistolAmmoText != null) { pistolAmmoText.text = ammoStr; pistolAmmoText.color = ammoColor; }
            if (shotgunAmmoText != null) { shotgunAmmoText.text = ammoStr; shotgunAmmoText.color = ammoColor; }
        }
        else
        {
            if (rifleAmmoText != null) { rifleAmmoText.text = " "; rifleAmmoText.color = Color.white; }
            if (pistolAmmoText != null) { pistolAmmoText.text = " "; pistolAmmoText.color = Color.white; }
            if (shotgunAmmoText != null) { shotgunAmmoText.text = " "; shotgunAmmoText.color = Color.white; }
        }
    }
}