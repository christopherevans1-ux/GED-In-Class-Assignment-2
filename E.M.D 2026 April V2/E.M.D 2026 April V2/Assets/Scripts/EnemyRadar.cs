using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyRadar : MonoBehaviour
{
    [Header("Radar Settings")]
    public float detectionRadius = 5f;
    public int barCount = 64;
    public float barWidth = 4f;
    public float barMinHeight = 4f;
    public float barMaxHeight = 28f;
    public float ringRadius = 120f;
    public float pulseSpeed = 8f;

    [Header("Appearance")]
    public Color barColor = new Color(1f, 0.25f, 0.25f, 0.85f);
    public Color barIdleColor = new Color(0f, 0f, 0f, 0f);

    private Transform _player;
    private RectTransform _container;
    private List<RectTransform> _bars = new List<RectTransform>();
    private float[] _barHeights;
    private float[] _barTargets;
    private CameraToggle _cameraToggle;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraToggle = FindFirstObjectByType<CameraToggle>();

        _container = new GameObject("RadarContainer").AddComponent<RectTransform>();
        _container.SetParent(transform, false);
        _container.anchorMin = new Vector2(0.5f, 0.5f);
        _container.anchorMax = new Vector2(0.5f, 0.5f);
        _container.anchoredPosition = Vector2.zero;
        _container.sizeDelta = Vector2.zero;

        _barHeights = new float[barCount];
        _barTargets = new float[barCount];

        for (int i = 0; i < barCount; i++)
        {
            float angle = i * 360f / barCount;
            float rad = angle * Mathf.Deg2Rad;

            GameObject barObj = new GameObject("Bar_" + i);
            RectTransform rt = barObj.AddComponent<RectTransform>();
            Image img = barObj.AddComponent<Image>();

            rt.SetParent(_container, false);
            rt.sizeDelta = new Vector2(barWidth, barMinHeight);
            rt.pivot = new Vector2(0.5f, 0f);

            float x = Mathf.Sin(rad) * ringRadius;
            float y = Mathf.Cos(rad) * ringRadius;
            rt.anchoredPosition = new Vector2(x, y);
            rt.localRotation = Quaternion.Euler(0f, 0f, -angle);

            img.color = barIdleColor;
            _bars.Add(rt);
            _barHeights[i] = barMinHeight;
            _barTargets[i] = barMinHeight;
        }
    }

    void Update()
    {
        if (_player == null) return;

        for (int i = 0; i < barCount; i++)
            _barTargets[i] = barMinHeight;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        bool isBirdsEye = _cameraToggle != null && _cameraToggle.isBirdsEye;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3 toEnemy = enemy.transform.position - _player.position;
            float distance = toEnemy.magnitude;

            if (distance > detectionRadius) continue;

            float proximityStrength = 1f - (distance / detectionRadius);
            float targetHeight = Mathf.Lerp(barMinHeight, barMaxHeight, proximityStrength * proximityStrength);

            float angle;

            if (isBirdsEye)
            {
                Vector3 flat = new Vector3(toEnemy.x, 0f, toEnemy.z).normalized;
                angle = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
                if (angle < 0f) angle += 360f;
            }
            else
            {
                Vector3 flat = new Vector3(toEnemy.x, 0f, toEnemy.z).normalized;
                Vector3 playerForward = new Vector3(_player.forward.x, 0f, _player.forward.z).normalized;
                angle = Vector3.SignedAngle(playerForward, flat, Vector3.up);
                if (angle < 0f) angle += 360f;
            }

            float barFloat = (angle / 360f) * barCount;
            int centerBar = Mathf.RoundToInt(barFloat) % barCount;
            int spread = Mathf.RoundToInt(proximityStrength * 6f) + 2;

            for (int s = -spread; s <= spread; s++)
            {
                int idx = ((centerBar + s) % barCount + barCount) % barCount;
                float falloff = 1f - (Mathf.Abs(s) / (float)(spread + 1));
                float h = Mathf.Lerp(barMinHeight, targetHeight, falloff * falloff);
                if (h > _barTargets[idx])
                    _barTargets[idx] = h;
            }
        }

        for (int i = 0; i < barCount; i++)
        {
            _barHeights[i] = Mathf.Lerp(_barHeights[i], _barTargets[i], Time.deltaTime * pulseSpeed);
            _bars[i].sizeDelta = new Vector2(barWidth, _barHeights[i]);

            float t = (_barHeights[i] - barMinHeight) / (barMaxHeight - barMinHeight);
            _bars[i].GetComponent<Image>().color = Color.Lerp(barIdleColor, barColor, t);
        }
    }
}