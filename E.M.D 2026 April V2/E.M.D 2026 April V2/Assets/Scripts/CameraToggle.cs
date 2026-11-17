using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [Header("Camera References")]
    public Camera normalCamera;
    public Camera birdsEyeCamera;
    public Transform player;
    public PlayerCam playerCamScript;
    public GameObject crosshair;

    [Header("Player Camera Rotation Script")]
    public MonoBehaviour playerCameraRotationScript;

    [Header("Transition Settings")]
    public float transitionSpeed = 3f;

    [Header("Birds Eye Heights")]
    public float birdsEyeStartHeight = 30f;
    public float birdsEyeRestHeight = 15.4f;

    [Header("Camera Descent")]
    public float descentDuration = 10f;

    [Header("UI")]
    public Canvas mainCanvas;

    public bool isBirdsEye = false;
    private MoveCamera normalCamScript;
    private MoveCamera birdsEyeCamScript;
    private Vector3 normalCamStartPos;
    private Quaternion normalCamStartRot;
    private Vector3 birdsEyeStartPos;
    private Quaternion birdsEyeStartRot;
    private float transitionProgress = 0f;
    private bool isTransitioning = false;
    private Quaternion fixedBirdsEyeRotation;

    private float descentTimer = 0f;
    private bool isDescending = false;
    private float descentStartHeight;
    private float descentTargetHeight;

    private BirdsEyeOverlay overlay;
    private Movement movement;
    private bool wasDead = false;

    void Start()
    {
        overlay = FindFirstObjectByType<BirdsEyeOverlay>();
        movement = player.GetComponent<Movement>();

        if (normalCamera != null)
        {
            normalCamScript = normalCamera.GetComponent<MoveCamera>();
            normalCamera.enabled = true;
        }

        if (birdsEyeCamera != null)
        {
            birdsEyeCamScript = birdsEyeCamera.GetComponent<MoveCamera>();
            birdsEyeCamera.enabled = false;
            fixedBirdsEyeRotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    void Update()
    {
        if (movement != null)
        {
            bool isDead = movement.isDead;
            if (isDead && !wasDead && isBirdsEye)
                ForceReturnToFirstPerson();
            if (wasDead && !isDead && isBirdsEye)
                ForceReturnToFirstPerson();
            wasDead = isDead;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (movement != null && movement.isDead) return;
            if (!isBirdsEye && overlay != null && overlay.CooldownActive)
            {
                Debug.Log("Cooldown: " + Mathf.CeilToInt(overlay.CooldownTimer) + "s remaining");
                return;
            }
            ToggleCamera();
        }

        if (isTransitioning)
            HandleTransition();

        if (isDescending)
        {
            descentTimer += Time.deltaTime;
            float t = Mathf.Clamp01(descentTimer / descentDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentHeight = Mathf.Lerp(descentStartHeight, descentTargetHeight, smoothT);

            birdsEyeCamera.transform.position = new Vector3(
                player.position.x,
                player.position.y + currentHeight,
                player.position.z
            );
            birdsEyeCamera.transform.rotation = fixedBirdsEyeRotation;

            RotatePlayerTowardsMouse();

            if (t >= 1f)
                isDescending = false;
        }
        else if (isBirdsEye && !isTransitioning && player != null)
        {
            Vector3 current = birdsEyeCamera.transform.position;
            birdsEyeCamera.transform.position = new Vector3(
                player.position.x,
                current.y,
                player.position.z
            );
            birdsEyeCamera.transform.rotation = fixedBirdsEyeRotation;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            RotatePlayerTowardsMouse();
        }
    }

    void RotatePlayerTowardsMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Ray ray = birdsEyeCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = hit.point - player.position;
            direction.y = 0;
            if (direction != Vector3.zero)
                player.rotation = Quaternion.LookRotation(direction);
        }
    }

    void ToggleCamera()
    {
        isBirdsEye = !isBirdsEye;
        isTransitioning = true;
        transitionProgress = 0f;

        if (movement != null)
            movement.SetBirdsEyeMode(isBirdsEye);

        if (isBirdsEye)
        {
            if (overlay != null) overlay.StartBirdsEye();
            if (normalCamScript != null) normalCamScript.enabled = false;
            if (playerCameraRotationScript != null) playerCameraRotationScript.enabled = false;
            if (playerCamScript != null) playerCamScript.SetBirdsEyeMode(true);
            if (crosshair != null) crosshair.SetActive(false);
            if (mainCanvas != null) mainCanvas.worldCamera = birdsEyeCamera;

            normalCamStartPos = normalCamera.transform.position;
            normalCamStartRot = normalCamera.transform.rotation;

            birdsEyeStartPos = new Vector3(player.position.x, player.position.y + birdsEyeStartHeight, player.position.z);
            birdsEyeStartRot = fixedBirdsEyeRotation;

            birdsEyeCamera.enabled = true;
            if (birdsEyeCamScript != null) birdsEyeCamScript.enabled = false;

            birdsEyeCamera.transform.position = normalCamStartPos;
            birdsEyeCamera.transform.rotation = normalCamStartRot;

            for (int i = 0; i < player.GetComponent<Movement>().weapons.Count; i++)
            {
                //player.GetComponent<Movement>().weapons[i].gameObject.transform.position -= new Vector3(0, 0, 0.5f);
            }
        }
        else
        {
            if (overlay != null) overlay.EndBirdsEye();
            isDescending = false;
            if (birdsEyeCamScript != null) birdsEyeCamScript.enabled = false;
            if (mainCanvas != null) mainCanvas.worldCamera = normalCamera;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            birdsEyeStartPos = birdsEyeCamera.transform.position;
            birdsEyeStartRot = birdsEyeCamera.transform.rotation;
            normalCamStartPos = normalCamera.transform.position;
            normalCamStartRot = normalCamera.transform.rotation;
            for (int i = 0; i < player.GetComponent<Movement>().weapons.Count; i++)
            {
                //player.GetComponent<Movement>().weapons[i].gameObject.transform.position += new Vector3(0, 0, 0.5f);
            }
        }
    }

    void HandleTransition()
    {
        transitionProgress += Time.deltaTime * transitionSpeed * 2f;

        if (isBirdsEye)
        {
            birdsEyeCamera.transform.position = Vector3.Lerp(normalCamStartPos, birdsEyeStartPos, transitionProgress);
            birdsEyeCamera.transform.rotation = Quaternion.Slerp(normalCamStartRot, birdsEyeStartRot, transitionProgress);

            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
                normalCamera.enabled = false;
                if (birdsEyeCamScript != null) birdsEyeCamScript.enabled = true;

                isDescending = true;
                descentTimer = 0f;
                descentStartHeight = birdsEyeStartHeight;
                descentTargetHeight = birdsEyeRestHeight;
            }
        }
        else
        {
            birdsEyeCamera.transform.position = Vector3.Lerp(birdsEyeStartPos, normalCamStartPos, transitionProgress);
            birdsEyeCamera.transform.rotation = Quaternion.Slerp(birdsEyeStartRot, normalCamStartRot, transitionProgress);

            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
                birdsEyeCamera.enabled = false;
                normalCamera.enabled = true;
                if (normalCamScript != null) normalCamScript.enabled = true;
                if (playerCameraRotationScript != null) playerCameraRotationScript.enabled = true;
                if (playerCamScript != null) playerCamScript.SetBirdsEyeMode(false);
                if (crosshair != null) crosshair.SetActive(true);
            }
        }
    }

    public void ForceReturnToFirstPerson()
    {
        if (!isBirdsEye) return;
        isBirdsEye = false;
        isDescending = false;
        isTransitioning = true;
        transitionProgress = 0f;

        if (overlay != null) overlay.EndBirdsEye();
        if (mainCanvas != null) mainCanvas.worldCamera = normalCamera;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        birdsEyeStartPos = birdsEyeCamera.transform.position;
        birdsEyeStartRot = birdsEyeCamera.transform.rotation;
        normalCamStartPos = normalCamera.transform.position;
        normalCamStartRot = normalCamera.transform.rotation;

        if (movement != null) movement.SetBirdsEyeMode(false);
        if (playerCameraRotationScript != null) playerCameraRotationScript.enabled = false;
        if (playerCamScript != null) playerCamScript.SetBirdsEyeMode(false);

        for (int i = 0; i < player.GetComponent<Movement>().weapons.Count; i++)
        {
            //player.GetComponent<Movement>().weapons[i].gameObject.transform.position += new Vector3(0, 0, 0.5f);
        }
    }
}