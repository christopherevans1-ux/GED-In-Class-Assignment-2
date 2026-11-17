using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform player;
    public GameObject weaponHolder;
    private Vector3 startTransform;
    float cameraVerticalRotation = 0f;
    private float savedVerticalRotation = 0f;
    public bool isBirdsEye = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startTransform = weaponHolder.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (PauseMenu.GameIsPaused) return;

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * PauseMenu.mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * PauseMenu.mouseSens;

        if (!isBirdsEye)
        {
            cameraVerticalRotation -= mouseY;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
            transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

            player.Rotate(Vector3.up * mouseX); // Only rotate player in normal mode
        }
    }

    public void SetBirdsEyeMode(bool birdsEye)
    {
        if (birdsEye && !isBirdsEye)
        {
            weaponHolder.transform.localPosition -= new Vector3(0, 0, 0.45f);
            savedVerticalRotation = cameraVerticalRotation;
            cameraVerticalRotation = 0f;
            transform.localEulerAngles = Vector3.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!birdsEye && isBirdsEye)
        {
            weaponHolder.transform.localPosition = startTransform;
            cameraVerticalRotation = savedVerticalRotation;
            transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        isBirdsEye = birdsEye;
    }
}