using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{
    public GameObject playerWeapon;
    public GameObject player;
    public Transform weaponThrowPoint;
    public float throwDamage;
    public float throwSpeed;
    public AudioSource throwSound;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerWeapon = GetComponent<WeaponPickup>().currentWeapon;
        if (playerWeapon != null)
        {
            throwDamage = playerWeapon.GetComponent<WeaponCleanUp>().contactDamage;
        }
        if (Input.GetButtonDown("Fire2") && player.GetComponent<Movement>().isDead == false && playerWeapon != null)
        {
            ThrowWeapon();
        }
    }

    public void ThrowWeapon()
    {
        if(playerWeapon != null)
        {
            throwSound.Play();
            GetComponent<Movement>().weapons[GetComponent<WeaponPickup>().weaponNum].SetActive(false);
            playerWeapon.transform.SetPositionAndRotation(weaponThrowPoint.transform.position, player.transform.rotation);
            playerWeapon.SetActive(true);
            playerWeapon.GetComponent<Rigidbody>().AddForce(weaponThrowPoint.forward * throwSpeed, ForceMode.Impulse);
            if (GetComponent<WeaponPickup>().currentWeapon.GetComponent<WeaponCleanUp>().isMelee == false)
            {
                GetComponent<WeaponPickup>().currentWeapon.GetComponent<WeaponCleanUp>().relatedMagSize = GetComponent<Movement>().weapons[GetComponent<WeaponPickup>().weaponNum].GetComponent<PlayerShoot>().magSize;
            }
            GetComponent<WeaponPickup>().lastWeaponPickup = null;
            GetComponent<WeaponPickup>().currentWeapon = null;
        }
    }

    public void ThrowWeaponForPickup()
    {
        if (playerWeapon != null)
        {
            throwSound.Play();
            GetComponent<Movement>().weapons[GetComponent<WeaponPickup>().weaponNum].SetActive(false);
            playerWeapon.transform.SetPositionAndRotation(weaponThrowPoint.transform.position, player.transform.rotation);
            playerWeapon.SetActive(true);
            playerWeapon.GetComponent<Rigidbody>().AddForce(weaponThrowPoint.forward * throwSpeed, ForceMode.Impulse);
            if(GetComponent<WeaponPickup>().currentWeapon.GetComponent<WeaponCleanUp>().isMelee == false)
            {
                GetComponent<WeaponPickup>().currentWeapon.GetComponent<WeaponCleanUp>().relatedMagSize = GetComponent<Movement>().weapons[GetComponent<WeaponPickup>().weaponNum].GetComponent<PlayerShoot>().magSize;
            }
        }
    }
}
