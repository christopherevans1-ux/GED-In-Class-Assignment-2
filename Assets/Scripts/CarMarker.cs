using UnityEngine;

public class CarMarker : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrow;
    public float bobHeight = 0.4f;
    public float bobSpeed = 1.5f;

    private Vector3 _arrowStartPos;

    void OnEnable()
    {
        arrow.SetActive(true);
        _arrowStartPos = arrow.transform.localPosition;
    }

    void Update()
    {
        float newY = _arrowStartPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        arrow.transform.localPosition = new Vector3(_arrowStartPos.x, newY, _arrowStartPos.z);
    }
}