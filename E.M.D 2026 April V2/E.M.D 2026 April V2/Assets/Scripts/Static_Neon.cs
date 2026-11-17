using UnityEngine;

public class StaticNeon : MonoBehaviour
{
    [Header("Emission Settings")]
    [ColorUsage(true, true)]
    public Color emissionColor = Color.cyan;
    public float emissionIntensity = 3f;

    [Header("Point Light")]
    public bool addPointLight = true;
    public float lightRange = 3f;
    public float lightIntensity = 2f;

    [Header("Subtle Pulse (Optional)")]
    public bool addSubtlePulse = false;
    public float pulseSpeed = 0.5f;
    public float pulseAmount = 0.2f; // How much it varies (0-1)

    private Material material;
    private Light pointLight;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            enabled = false; 
            return;
        }
        material = rend.material;
        material.EnableKeyword("_EMISSION");

        // Create point light
        if (addPointLight)
        {
            GameObject lightObj = new GameObject("NeonLight");
            lightObj.transform.parent = transform;
            lightObj.transform.localPosition = Vector3.forward * 0.1f;

            pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = lightRange;
            pointLight.color = emissionColor;
            pointLight.intensity = lightIntensity;
        }

        UpdateEmission();
    }

    void Update()
    {
        if (addSubtlePulse)
        {
            float pulse = 1f + (Mathf.Sin(Time.time * pulseSpeed) * pulseAmount);
            Color finalColor = emissionColor * (emissionIntensity * pulse);
            material.SetColor("_EmissionColor", finalColor);

            if (pointLight != null)
                pointLight.intensity = lightIntensity * pulse;
        }
    }

    public void UpdateEmission()
    {
        Color finalColor = emissionColor * emissionIntensity;
        material.SetColor("_EmissionColor", finalColor);

        if (pointLight != null)
        {
            pointLight.intensity = lightIntensity;
            pointLight.color = emissionColor;
        }
    }

    void OnValidate()
    {
        if (Application.isPlaying && material != null)
            UpdateEmission();
    }

    void OnDestroy()
    {
        if (material != null)
            Destroy(material);
    }
}