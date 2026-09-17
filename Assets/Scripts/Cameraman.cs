using UnityEngine;

public class Cameraman : MonoBehaviour
{
    [SerializeField] private Transform target, targetArea;
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] private float cameraOrthographicSizeModVal;
    [SerializeField] private float cameraSmoothSpeed;
    [SerializeField] private Vector3 mousePositionScreen;
    private float originX, fixedY, fixedZ;
    private bool enableCameraFollow;


    [SerializeField] private GameObject cameraHoverGuide;

    private Camera cam;

    private void OnEnable()
    {
        Slingshot.ObjectLaunched += SetCameraFollow;
        GameManager.OnLevelChange += RepositionTargetAreaOnLevelChange;
    }

    private void OnDisable()
    {
        Slingshot.ObjectLaunched -= SetCameraFollow;
        GameManager.OnLevelChange -= RepositionTargetAreaOnLevelChange;
    }

    void RepositionTargetAreaOnLevelChange()
    {
        int currentLevel = GameManager.Instance.levelNo;
        if (currentLevel == 1)
        {
            minX = 0f;
            maxX = 3f;
            cameraOrthographicSizeModVal = 0f;
            targetArea.position = new Vector3(4f, 0f, 0f);
        }
        else if (currentLevel == 2)
        {
            minX = 2.5f;
            maxX = 15f;
            cameraOrthographicSizeModVal = 2f;
            cam.orthographicSize += cameraOrthographicSizeModVal;
            targetArea.position = new Vector3(12f, 0.25f, 0f);
        }

        Zoom();
        MoveCameraWithLerp(target);
    }


    void SetCameraFollow(bool state = true)
    {
        enableCameraFollow = state;
    }

    void Start()
    {
        enableCameraFollow = false;
        if (cameraSmoothSpeed == 0) cameraSmoothSpeed = 5f;

        originX = 0;
        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        cam = GetComponent<Camera>();

        // Just to ensure camera is set to level 1 position at the start of the game
        RepositionTargetAreaOnLevelChange();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (GameManager.Instance.playerCanNowMove == false) return;

        // ZOOM
        Zoom();

        // HOVER RIGHT SIDE
        mousePositionScreen = Input.mousePosition;
        if (!enableCameraFollow)
        {
            if (mousePositionScreen.x >= 1671)
            {
                if (cameraHoverGuide.activeSelf) cameraHoverGuide.SetActive(false);
                MoveCameraWithLerp(targetArea);
                float targetPosition =
                    Mathf.Max(5f, 5f + (targetArea.position.x * 0.05f) + (targetArea.position.y * 0.4f)) +
                    cameraOrthographicSizeModVal;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetPosition, Time.deltaTime * 5f);
            }
            // else MoveCameraWithLerp(GetOriginTransform());
            else
            {
                Vector3 clampedTargetPosition =
                    new Vector3(Mathf.Clamp(0, minX, maxX), fixedY, fixedZ);
                transform.position =
                    Vector3.Lerp(transform.position, clampedTargetPosition, Time.deltaTime * cameraSmoothSpeed);
                if (cameraHoverGuide.activeSelf == false)
                    cameraHoverGuide.SetActive(true);
            }
        }
        else
        {
            MoveCameraWithLerp(target); // FOLLOW LAUNCHED OBJECT
            cameraHoverGuide.SetActive(false);
        }
        // transform.position = new Vector3(target.position.x, fixedY, fixedZ);
    }

    Transform GetOriginTransform()
    {
        Transform modifiedTransform = transform;
        modifiedTransform.position = new Vector3(originX, fixedY, fixedZ);
        return modifiedTransform;
    }

    void MoveCameraWithLerp(Transform targetPoint)
    {
        Vector3 clampedTargetPosition =
            new Vector3(Mathf.Clamp(targetPoint.position.x, minX, maxX),
                Mathf.Clamp(targetPoint.position.y, 0, 5f),
                fixedZ);
        transform.position =
            Vector3.Lerp(transform.position, clampedTargetPosition, Time.deltaTime * cameraSmoothSpeed);
    }

    void Zoom()
    {
        float targetPosition = Mathf.Max(5f, 5f + ((target.position.x) * 0.05f) + (target.position.y * 0.4f)) +
                               cameraOrthographicSizeModVal;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetPosition, Time.deltaTime * 5f);
    }
}
