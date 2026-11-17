using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    public Camera cam;
    public float maximumLength = 100f;

    private Ray rayMouse;
    private Vector3 direction;

    private Quaternion rotation;

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            rayMouse = cam.ScreenPointToRay(mousePos);

            Vector3 destination;

            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLength))
            {
                destination = hit.point;
            }
            else
            {
                destination = rayMouse.GetPoint(maximumLength);
            }

            RotateToMouseDirection(gameObject, destination);
        }
        else
        {
            Debug.Log("NO CAM");
        }
    }

    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;

        obj.transform.rotation = Quaternion.LookRotation(direction);

        rotation = obj.transform.rotation;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}