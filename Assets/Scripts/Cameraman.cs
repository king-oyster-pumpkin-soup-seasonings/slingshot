using UnityEngine;

public class Cameraman : MonoBehaviour
{
    [SerializeField] private Transform target, targetArea;
    [SerializeField] float minX;
    [SerializeField] float maxX;
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
            targetArea.position = new Vector3(4f, 0f, 0f);
        }
        else if (currentLevel == 2)
        {
            targetArea.position = new Vector3(12f, 0.25f, 0f);
        }
    }


    void SetCameraFollow(bool state = true)
    {
        enableCameraFollow = state;
    }

    void Start()
    {
        enableCameraFollow = false;
        if (cameraSmoothSpeed == 0) cameraSmoothSpeed = 5f;
        if (minX == 0) minX = 0f;
        if (maxX == 0) maxX = 21f;

        originX = 0;
        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (GameManager.Instance.playerCanNowMove == false) return;

        // ZOOM
        if (!(mousePositionScreen.x >= 1671))
        {
            float targetPosition = Mathf.Max(5f,
                5f + (target.position.x * 0.05f) + (target.position.y * 0.4f));
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetPosition, Time.deltaTime * 5f);
        }


        // HOVER RIGHT SIDE
        mousePositionScreen = Input.mousePosition;
        if (!enableCameraFollow)
        {
            if (mousePositionScreen.x >= 1671)
            {
                if (cameraHoverGuide.activeSelf) cameraHoverGuide.SetActive(false);
                MoveCameraWithLerp(targetArea);
                float targetPosition = Mathf.Max(5f,
                    5f + (targetArea.position.x * 0.05f) + (targetArea.position.y * 0.4f));
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
            new Vector3(Mathf.Clamp(targetPoint.position.x, minX, maxX), Mathf.Clamp(targetPoint.position.y, 0, 5f),
                fixedZ);
        transform.position =
            Vector3.Lerp(transform.position, clampedTargetPosition, Time.deltaTime * cameraSmoothSpeed);
    }
}
