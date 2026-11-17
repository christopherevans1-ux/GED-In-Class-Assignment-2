using UnityEngine;

public class CarIdleMotion : MonoBehaviour
{
    void Update()
    {
        float sway = Mathf.Sin(Time.time * 2f) * 0.05f;

        transform.position = new Vector3(
            sway,
            transform.position.y,
            transform.position.z
        );
    }
}