using UnityEngine;

public class StreetLamp : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public float spotAngle = 60f;
    public float lightRange = 10f;
    public float lightIntensity = 3f;
    public Color lightColor = new Color(1f, 0.88f, 0.6f);

    [Header("Pulse Settings")]
    public float pulseSpeed = 1.2f;    
    public float pulseDepth = 0.18f;      
    public bool randomizeOffset = true;    

    [Header("Bulb Emission (Optional)")]
    public bool controlBulbEmission = true;
    [ColorUsage(true, true)]
    public Color emissionColor = new Color(1f, 0.88f, 0.6f);
    public float emissionIntensity = 3f;

    private Light spotlight;
    private Material bulbMaterial;
    private float timeOffset;

    void Start()
    {
        timeOffset = randomizeOffset ? Random.Range(0f, Mathf.PI * 2f) : 0f;

        GameObject lightObj = new GameObject("StreetLamp_Spotlight");
        lightObj.transform.parent = transform;
        lightObj.transform.localPosition = Vector3.zero;
        lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); 

        spotlight = lightObj.AddComponent<Light>();
        spotlight.type = LightType.Spot;
        spotlight.spotAngle = spotAngle;
        spotlight.range = lightRange;
        spotlight.color = lightColor;
        spotlight.intensity = lightIntensity;
        spotlight.renderMode = LightRenderMode.ForcePixel;

        if (controlBulbEmission)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                bulbMaterial = rend.material;
                bulbMaterial.EnableKeyword("_EMISSION");
            }
        }
    }

    void Update()
    {
        float pulse = 1f - (pulseDepth * (0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed + timeOffset)));

        spotlight.intensity = lightIntensity * pulse;

        if (bulbMaterial != null)
            bulbMaterial.SetColor("_EmissionColor", emissionColor * (emissionIntensity * pulse));
    }

    void OnDestroy()
    {
        if (bulbMaterial != null)
            Destroy(bulbMaterial);
    }
}