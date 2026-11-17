using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPickup : MonoBehaviour
{
    public int weaponNum = 0;
    public GameObject lastWeaponPickup;
    public GameObject currentWeapon;

    [Header("Pickup UI")]
    public GameObject pickupPromptPrefab;
    public float promptHeightOffset = 0.3f;

    [Header("Weapon Highlight")]
    public Color highlightColor = new Color(1f, 0.92f, 0.016f, 1f);
    public float emissionIntensity = 2f;

    [Header("Tutorial Arrow")]
    public GameObject gunArrow;

    private bool isNearWeapon = false;
    private GameObject currentPrompt;

    private Renderer[] allWeaponRenderers;
    private Material[] originalMaterials;

    void Update()
    {
        if (isNearWeapon && (lastWeaponPickup == null || !lastWeaponPickup.activeInHierarchy))
        {
            RemoveGlowAndPrompt();
            lastWeaponPickup = null;
            return;
        }

        if (isNearWeapon && Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<WeaponThrow>().ThrowWeaponForPickup();
            PickupWeapon();
            currentWeapon = lastWeaponPickup;
        }

        if (currentPrompt != null && Camera.main != null)
        {
            if (lastWeaponPickup != null)
                currentPrompt.transform.position = lastWeaponPickup.transform.position + Vector3.up * promptHeightOffset;

            currentPrompt.transform.LookAt(Camera.main.transform);
            currentPrompt.transform.Rotate(0, 180, 0);
        }
    }

    void OnDisable()
    {
        RemoveGlowAndPrompt();
        lastWeaponPickup = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (isNearWeapon)
                RemoveGlowAndPrompt();

            isNearWeapon = true;
            lastWeaponPickup = other.gameObject;

            if (pickupPromptPrefab != null)
            {
                currentPrompt = Instantiate(pickupPromptPrefab,
                    other.transform.position + Vector3.up * promptHeightOffset,
                    Quaternion.identity);

                if (Camera.main != null)
                {
                    currentPrompt.transform.LookAt(Camera.main.transform);
                    currentPrompt.transform.Rotate(0, 180, 0);
                }
            }

            ApplyGlow(lastWeaponPickup);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon") && other.gameObject == lastWeaponPickup)
        {
            RemoveGlowAndPrompt();
            lastWeaponPickup = null;
        }
    }

    private void ApplyGlow(GameObject weapon)
    {
        allWeaponRenderers = weapon.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[allWeaponRenderers.Length];

        for (int i = 0; i < allWeaponRenderers.Length; i++)
        {
            Renderer rend = allWeaponRenderers[i];
            if (rend is ParticleSystemRenderer) continue;

            originalMaterials[i] = rend.sharedMaterial;
            Material glowMat = new Material(rend.sharedMaterial);
            glowMat.EnableKeyword("_EMISSION");
            glowMat.SetColor("_EmissionColor", highlightColor * emissionIntensity);
            rend.material = glowMat;
        }
    }

    public void PickupWeapon()
    {
        if (lastWeaponPickup == null) return;

        RemoveGlowAndPrompt();
        if (gunArrow != null) gunArrow.SetActive(false);
        lastWeaponPickup.SetActive(false);

        string wName = lastWeaponPickup.name;
        Movement movement = GetComponent<Movement>();

        int newWeaponNum = weaponNum;

        if (wName == "AKR Pickup" || wName == "AKR Pickup(Clone)") newWeaponNum = 0;
        else if (wName == "AA-13 Pickup" || wName == "AA-13 Pickup(Clone)") newWeaponNum = 1;
        else if (wName == "BaseballBat Pickup" || wName == "BaseballBat Pickup(Clone)") newWeaponNum = 2;
        else if (wName == "G18 Pickup" || wName == "G18 Pickup(Clone)") newWeaponNum = 3;
        else if (wName == "Pipe Pickup" || wName == "Pipe Pickup(Clone)") newWeaponNum = 4;

        movement.weapons[weaponNum].SetActive(false);
        weaponNum = newWeaponNum;
        movement.weapons[weaponNum].SetActive(true);

        var shoot = movement.weapons[weaponNum].GetComponent<PlayerShoot>();
        var cleanUp = lastWeaponPickup.GetComponent<WeaponCleanUp>();
        if (shoot != null && cleanUp != null)
            shoot.magSize = cleanUp.relatedMagSize;
    }

    private void RemoveGlowAndPrompt()
    {
        isNearWeapon = false;

        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
            currentPrompt = null;
        }

        if (allWeaponRenderers != null)
        {
            for (int i = 0; i < allWeaponRenderers.Length; i++)
            {
                if (allWeaponRenderers[i] != null && originalMaterials[i] != null)
                    allWeaponRenderers[i].material = originalMaterials[i];
            }
            allWeaponRenderers = null;
            originalMaterials = null;
        }
    }
}