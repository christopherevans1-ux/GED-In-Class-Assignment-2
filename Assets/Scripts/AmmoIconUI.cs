using System.Collections;
using UnityEngine;

public class AmmoIconUI : MonoBehaviour
{
    [System.Serializable]
    public class WeaponIcon
    {
        public string weaponName;
        public Animator iconAnimator;
    }

    [Header("Weapon Icons")]
    public WeaponIcon[] weaponIcons;

    [Header("Switch Settings")]
    public float switchOutDelay = 0.15f;

    private WeaponPickup weaponPickup;
    private Movement movement;

    private int lastWeaponNum = -1;
    private string lastWeaponName = "";
    private bool lastHadWeapon = false;
    private bool wasDead = false;

    void Start()
    {
        weaponPickup = GetComponent<WeaponPickup>();
        movement = GetComponent<Movement>();

        foreach (var wi in weaponIcons)
        {
            foreach (GameObject w in movement.weapons)
            {
                if (w.activeSelf && w.name == wi.weaponName)
                {
                    lastWeaponName = w.name;
                    lastHadWeapon = true;
                    lastWeaponNum = weaponPickup.weaponNum;
                    break;
                }
            }
            if (lastHadWeapon) break;
        }
    }

    void Update()
    {
        // Handle death — play Out and hide icon
        if (movement.isDead)
        {
            if (!wasDead)
            {
                if (!string.IsNullOrEmpty(lastWeaponName))
                {
                    TriggerIcon(lastWeaponName, "Out");
                    lastWeaponName = "";
                    lastHadWeapon = false;
                    lastWeaponNum = -1;
                }
                wasDead = true;
            }
            return;
        }
        wasDead = false;

        // Find which tracked weapon is currently active
        string currentWeaponName = "";
        foreach (var wi in weaponIcons)
        {
            foreach (GameObject w in movement.weapons)
            {
                if (w.activeSelf && w.name == wi.weaponName)
                {
                    currentWeaponName = w.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(currentWeaponName)) break;
        }

        bool hasWeapon = !string.IsNullOrEmpty(currentWeaponName);

        // Weapon thrown or unequipped
        if (lastHadWeapon && !hasWeapon)
        {
            TriggerIcon(lastWeaponName, "Out");
            lastWeaponName = "";
            lastHadWeapon = false;
            lastWeaponNum = -1;
            return;
        }

        // Weapon changed
        if (hasWeapon && currentWeaponName != lastWeaponName)
        {
            string outName = lastWeaponName;
            string inName = currentWeaponName;

            if (!string.IsNullOrEmpty(outName))
                StartCoroutine(SwitchIcon(outName, inName));
            else
                TriggerIcon(inName, "In");

            lastWeaponName = currentWeaponName;
            lastWeaponNum = weaponPickup.weaponNum;
            lastHadWeapon = true;
        }

        lastHadWeapon = hasWeapon;
    }

    private IEnumerator SwitchIcon(string outWeaponName, string inWeaponName)
    {
        // Freeze ammo text during switch delay
        foreach (GameObject w in movement.weapons)
        {
            PlayerShoot shoot = w.GetComponent<PlayerShoot>();
            if (shoot != null) shoot.freezeAmmoText = true;
        }

        TriggerIcon(outWeaponName, "Out");
        yield return new WaitForSeconds(switchOutDelay);

        foreach (GameObject w in movement.weapons)
        {
            PlayerShoot shoot = w.GetComponent<PlayerShoot>();
            if (shoot != null) shoot.freezeAmmoText = false;
        }

        TriggerIcon(inWeaponName, "In");
    }

    private void TriggerIcon(string weaponName, string trigger)
    {
        foreach (var wi in weaponIcons)
        {
            if (wi.weaponName == weaponName && wi.iconAnimator != null)
            {
                wi.iconAnimator.ResetTrigger("In");
                wi.iconAnimator.ResetTrigger("Out");
                wi.iconAnimator.SetTrigger(trigger);
                return;
            }
        }
    }
}