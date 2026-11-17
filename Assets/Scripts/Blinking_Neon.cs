using UnityEngine;
using System.Collections.Generic;

public class BlinkingNeon : MonoBehaviour
{
    [Header("Emission Settings")]
    [ColorUsage(true, true)]
    public Color emissionColor = Color.cyan;
    public float emissionIntensity = 3f;

    [Header("Blink Settings")]
    public float minOnTime = 0.5f;
    public float maxOnTime = 3f;
    public float minOffTime = 0.1f;
    public float maxOffTime = 0.5f;

    [Header("Flicker Settings")]
    public bool addFlicker = true;
    public float flickerChance = 0.3f;
    public int minFlickers = 1;
    public int maxFlickers = 3;
    public float flickerSpeed = 0.05f;

    [Header("Point Light")]
    public bool addPointLight = true;
    public float lightRange = 3f;
    public float lightIntensity = 2f;

    private List<Material> materials = new List<Material>();
    private List<Light> pointLights = new List<Light>();
    private bool isOn = true;
    private float timer;
    private bool isFlickering = false;
    private int flickerCount = 0;
    private int flickersRemaining = 0;

    void Start()
    {
        // Get all renderers in children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            // Create unique material instance for each child
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            materials.Add(mat);

            // Create point light for each child if enabled
            if (addPointLight)
            {
                GameObject lightObj = new GameObject("NeonLight_" + rend.name);
                lightObj.transform.parent = rend.transform;
                lightObj.transform.localPosition = Vector3.forward * 0.1f;

                Light pointLight = lightObj.AddComponent<Light>();
                pointLight.type = LightType.Point;
                pointLight.range = lightRange;
                pointLight.color = emissionColor;
                pointLight.intensity = lightIntensity;

                pointLights.Add(pointLight);
            }
        }

        timer = Random.Range(minOnTime, maxOnTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isFlickering)
        {
            HandleFlicker();
        }
        else if (timer <= 0)
        {
            if (isOn)
            {
                if (addFlicker && Random.value < flickerChance)
                {
                    StartFlicker();
                }
                else
                {
                    TurnOff();
                }
            }
            else
            {
                TurnOn();
            }
        }
    }

    void StartFlicker()
    {
        isFlickering = true;
        flickersRemaining = Random.Range(minFlickers, maxFlickers + 1);
        flickerCount = 0;
        timer = flickerSpeed;
    }

    void HandleFlicker()
    {
        if (timer <= 0)
        {
            flickerCount++;

            if (flickerCount % 2 == 0)
            {
                SetEmission(true);
            }
            else
            {
                SetEmission(false);
            }

            if (flickerCount >= flickersRemaining * 2)
            {
                isFlickering = false;
                TurnOff();
            }
            else
            {
                timer = flickerSpeed;
            }
        }
    }

    void TurnOn()
    {
        isOn = true;
        SetEmission(true);
        timer = Random.Range(minOnTime, maxOnTime);
    }

    void TurnOff()
    {
        isOn = false;
        SetEmission(false);
        timer = Random.Range(minOffTime, maxOffTime);
    }

    void SetEmission(bool enabled)
    {
        Color finalColor = enabled ? emissionColor * emissionIntensity : Color.black;
        float lightValue = enabled ? lightIntensity : 0f;

        // Update all child materials
        foreach (Material mat in materials)
        {
            mat.SetColor("_EmissionColor", finalColor);
        }

        // Update all point lights
        foreach (Light light in pointLights)
        {
            light.intensity = lightValue;
        }
    }

    void OnDestroy()
    {
        // Clean up all material instances
        foreach (Material mat in materials)
        {
            if (mat != null)
                Destroy(mat);
        }
    }
}