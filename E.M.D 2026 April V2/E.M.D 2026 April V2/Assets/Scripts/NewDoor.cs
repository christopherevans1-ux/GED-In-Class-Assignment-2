using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDoor : MonoBehaviour
{
    public float rotationForce = 10f; // Adjust this value in the Inspector

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb != null)
        {
            // Calculate a direction for the torque based on the collision normal
            // For example, to make it spin around the Y-axis relative to the impact point:
            Vector3 torqueDirection = Vector3.Cross(collision.contacts[0].normal, Vector3.up).normalized;
            Debug.Log("Working");
            // Apply torque
            rb.AddTorque((torqueDirection - new Vector3(0,0,0)) * rotationForce, ForceMode.Impulse);
            // ForceMode.Impulse applies an instant force
            // ForceMode.Force applies a continuous force over time (use in FixedUpdate for this)
        }
    }
}