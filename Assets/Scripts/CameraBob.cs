using UnityEngine;

public class CameraHeadbob : MonoBehaviour
{
    [Header("Bob Settings")]
    public float bobFrequency = 4f;
    public float bobHorizontalAmp = 0.03f;
    public float bobVerticalAmp = 0.05f;
    public float bobSmoothing = 12f;

    [Header("References")]
    public Movement movement;

    private Vector3 defaultLocalPos;
    private float bobTimer = 0f;

    private void Start()
    {
        defaultLocalPos = transform.localPosition;
    }

    private void Update()
    {
        HandleBob();
    }

    private void HandleBob()
    {
        bool shouldBob = movement.isWalking
                      && movement.isGrounded
                      && !movement.isDead
                      && !movement.victory;

        if (shouldBob)
        {
            float speed = movement.controller.velocity.magnitude;
            bobTimer += Time.deltaTime * bobFrequency * (speed / movement.speed);

            Vector3 bobOffset = new Vector3(
                Mathf.Sin(bobTimer) * bobHorizontalAmp,
                Mathf.Abs(Mathf.Sin(bobTimer)) * bobVerticalAmp,
                0f
            );

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                defaultLocalPos + bobOffset,
                Time.deltaTime * bobSmoothing
            );
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                defaultLocalPos,
                Time.deltaTime * bobSmoothing
            );
        }
    }
}