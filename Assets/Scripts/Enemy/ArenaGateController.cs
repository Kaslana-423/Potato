using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class ArenaGateController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform arenaSpace;
    [SerializeField] private PlayerController player;
    [SerializeField] private BoxCollider2D topDoorCollider;
    [SerializeField] private BoxCollider2D bottomDoorCollider;

    [Header("Door Presentation")]
    [SerializeField] private Transform topDoorVisual;
    [SerializeField] private Transform bottomDoorVisual;
    [SerializeField, Range(0f, 0.25f)] private float openHeightScale = 0.04f;
    [SerializeField, Min(0.05f)] private float doorAnimationSeconds = 0.55f;
    [SerializeField, Min(0f)] private float entranceHoldSeconds = 0.12f;

    [Header("Exit Camera Reveal")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform cameraFocusTarget;
    [SerializeField] private Vector2 topCameraFocusLocal = new Vector2(0.09f, 3.65f);
    [SerializeField, Min(0.05f)] private float cameraMoveSeconds = 1.1f;
    [SerializeField, Min(0f)] private float cameraSettleSeconds = 0.2f;
    [SerializeField, Min(0f)] private float exitRevealHoldSeconds = 0.3f;
    [SerializeField, Min(0.05f)] private float cameraReturnSeconds = 0.75f;

    [Header("Bottom Entrance")]
    [SerializeField] private Vector2 entranceStartLocal = new Vector2(0.09f, -5.35f);
    [SerializeField] private Vector2 entranceEndLocal = new Vector2(0.09f, -3.6f);
    [SerializeField, Min(0.05f)] private float entranceDurationSeconds = 0.65f;

    private bool exitArmed;
    private Vector3 topDoorClosedScale;
    private Vector3 bottomDoorClosedScale;
    private Transform playerCameraFollow;
    private Transform playerCameraLookAt;

    public bool HasPlayerExited { get; private set; }
    public bool IsConfigured => arenaSpace != null
        && player != null
        && topDoorCollider != null
        && bottomDoorCollider != null;

    private void Awake()
    {
        CachePresentationState();
        BoxCollider2D exitTrigger = GetComponent<BoxCollider2D>();
        exitTrigger.isTrigger = true;
        CloseDoors();
    }

    private void OnValidate()
    {
        entranceDurationSeconds = Mathf.Max(0.05f, entranceDurationSeconds);
        doorAnimationSeconds = Mathf.Max(0.05f, doorAnimationSeconds);
        entranceHoldSeconds = Mathf.Max(0f, entranceHoldSeconds);
        cameraMoveSeconds = Mathf.Max(0.05f, cameraMoveSeconds);
        cameraSettleSeconds = Mathf.Max(0f, cameraSettleSeconds);
        exitRevealHoldSeconds = Mathf.Max(0f, exitRevealHoldSeconds);
        cameraReturnSeconds = Mathf.Max(0.05f, cameraReturnSeconds);
        BoxCollider2D exitTrigger = GetComponent<BoxCollider2D>();
        if (exitTrigger != null)
        {
            exitTrigger.isTrigger = true;
        }
    }

    public IEnumerator PlayEntrance()
    {
        if (!IsConfigured)
        {
            yield break;
        }

        HasPlayerExited = false;
        exitArmed = false;
        RestorePlayerCamera();
        topDoorCollider.enabled = false;
        bottomDoorCollider.enabled = false;
        player.SetControlEnabled(false);
        player.SetBoundaryEnabled(false);

        SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, 1f);
        SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, 0f);
        yield return AnimateDoor(bottomDoorVisual, bottomDoorClosedScale, true);
        if (entranceHoldSeconds > 0f)
        {
            yield return new WaitForSeconds(entranceHoldSeconds);
        }

        Vector3 start = arenaSpace.TransformPoint(entranceStartLocal);
        Vector3 end = arenaSpace.TransformPoint(entranceEndLocal);
        start.z = player.transform.position.z;
        end.z = player.transform.position.z;
        player.SetTransitionPosition(start);

        float elapsed = 0f;
        while (elapsed < entranceDurationSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / entranceDurationSeconds);
            player.SetTransitionPosition(Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, progress)));
            yield return null;
        }

        yield return AnimateBothDoorsClosed();
        topDoorCollider.enabled = true;
        bottomDoorCollider.enabled = true;
        player.SetBoundaryEnabled(true);
        player.RestorePosition(end);
        player.SetControlEnabled(true);
    }

    public IEnumerator PlayExitOpening()
    {
        if (!IsConfigured)
        {
            HasPlayerExited = true;
            yield break;
        }

        HasPlayerExited = false;
        exitArmed = false;
        player.SetControlEnabled(false);
        topDoorCollider.enabled = true;
        bottomDoorCollider.enabled = true;

        yield return MoveCameraToTopDoor();
        yield return AnimateDoor(topDoorVisual, topDoorClosedScale, true);
        if (exitRevealHoldSeconds > 0f)
        {
            yield return new WaitForSeconds(exitRevealHoldSeconds);
        }
        yield return ReturnCameraToPlayer();

        topDoorCollider.enabled = false;
        player.SetBoundaryEnabled(false);
        player.SetControlEnabled(true);
        exitArmed = true;
    }

    public void OpenExit()
    {
        if (!IsConfigured)
        {
            HasPlayerExited = true;
            return;
        }

        HasPlayerExited = false;
        exitArmed = true;
        bottomDoorCollider.enabled = true;
        topDoorCollider.enabled = false;
        SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, 1f);
        SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, 0f);
        player.SetBoundaryEnabled(false);
        player.SetControlEnabled(true);
    }

    public void PrepareCombatWithoutEntrance()
    {
        HasPlayerExited = false;
        exitArmed = false;
        RestorePlayerCamera();
        CloseDoors();
        if (player != null)
        {
            player.SetBoundaryEnabled(true);
            player.SetControlEnabled(true);
        }
    }

    public void LockPlayerForShop()
    {
        exitArmed = false;
        if (player != null)
        {
            player.SetControlEnabled(false);
        }
    }

    public void ShowShopState()
    {
        HasPlayerExited = true;
        exitArmed = false;
        if (bottomDoorCollider != null)
        {
            bottomDoorCollider.enabled = true;
        }

        if (topDoorCollider != null)
        {
            topDoorCollider.enabled = false;
        }

        SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, 1f);
        SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, 0f);

        if (player != null)
        {
            player.SetBoundaryEnabled(false);
            player.SetControlEnabled(false);
        }
    }

    private void CloseDoors()
    {
        if (topDoorCollider != null)
        {
            topDoorCollider.enabled = true;
        }

        if (bottomDoorCollider != null)
        {
            bottomDoorCollider.enabled = true;
        }

        SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, 0f);
        SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, 0f);
    }

    private void CachePresentationState()
    {
        if (topDoorVisual != null)
        {
            topDoorClosedScale = topDoorVisual.localScale;
        }

        if (bottomDoorVisual != null)
        {
            bottomDoorClosedScale = bottomDoorVisual.localScale;
        }

        if (virtualCamera != null)
        {
            playerCameraFollow = virtualCamera.Follow;
            playerCameraLookAt = virtualCamera.LookAt;
        }
    }

    private IEnumerator AnimateBothDoorsClosed()
    {
        float topStart = GetDoorOpenAmount(topDoorVisual, topDoorClosedScale);
        float bottomStart = GetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale);
        float elapsed = 0f;
        while (elapsed < doorAnimationSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / doorAnimationSeconds));
            SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, Mathf.Lerp(topStart, 0f, progress));
            SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, Mathf.Lerp(bottomStart, 0f, progress));
            yield return null;
        }

        SetDoorOpenAmount(topDoorVisual, topDoorClosedScale, 0f);
        SetDoorOpenAmount(bottomDoorVisual, bottomDoorClosedScale, 0f);
    }

    private IEnumerator AnimateDoor(Transform door, Vector3 closedScale, bool open)
    {
        if (door == null)
        {
            yield break;
        }

        float start = GetDoorOpenAmount(door, closedScale);
        float target = open ? 1f : 0f;
        float elapsed = 0f;
        while (elapsed < doorAnimationSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / doorAnimationSeconds));
            SetDoorOpenAmount(door, closedScale, Mathf.Lerp(start, target, progress));
            yield return null;
        }

        SetDoorOpenAmount(door, closedScale, target);
    }

    private void SetDoorOpenAmount(Transform door, Vector3 closedScale, float openAmount)
    {
        if (door == null)
        {
            return;
        }

        float height = Mathf.Lerp(1f, openHeightScale, Mathf.Clamp01(openAmount));
        door.localScale = new Vector3(closedScale.x, closedScale.y * height, closedScale.z);
    }

    private float GetDoorOpenAmount(Transform door, Vector3 closedScale)
    {
        if (door == null || Mathf.Approximately(closedScale.y, 0f) || Mathf.Approximately(1f, openHeightScale))
        {
            return 0f;
        }

        float heightRatio = door.localScale.y / closedScale.y;
        return Mathf.InverseLerp(1f, openHeightScale, heightRatio);
    }

    private IEnumerator MoveCameraToTopDoor()
    {
        if (virtualCamera == null || cameraFocusTarget == null || arenaSpace == null)
        {
            yield break;
        }

        Vector3 start = player != null ? player.transform.position : cameraFocusTarget.position;
        Vector3 target = arenaSpace.TransformPoint(topCameraFocusLocal);
        target.z = start.z;
        cameraFocusTarget.position = start;
        virtualCamera.Follow = cameraFocusTarget;
        virtualCamera.LookAt = cameraFocusTarget;

        float elapsed = 0f;
        while (elapsed < cameraMoveSeconds)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / cameraMoveSeconds));
            cameraFocusTarget.position = Vector3.Lerp(start, target, progress);
            yield return null;
        }

        cameraFocusTarget.position = target;
        if (cameraSettleSeconds > 0f)
        {
            yield return new WaitForSeconds(cameraSettleSeconds);
        }
    }

    private void RestorePlayerCamera()
    {
        if (virtualCamera == null)
        {
            return;
        }

        virtualCamera.Follow = playerCameraFollow != null ? playerCameraFollow : player?.transform;
        virtualCamera.LookAt = playerCameraLookAt != null ? playerCameraLookAt : player?.transform;
    }

    private IEnumerator ReturnCameraToPlayer()
    {
        if (virtualCamera == null || cameraFocusTarget == null || player == null)
        {
            RestorePlayerCamera();
            yield break;
        }

        Vector3 start = cameraFocusTarget.position;
        Vector3 target = player.transform.position;
        float elapsed = 0f;
        while (elapsed < cameraReturnSeconds)
        {
            elapsed += Time.deltaTime;
            target = player.transform.position;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / cameraReturnSeconds));
            cameraFocusTarget.position = Vector3.Lerp(start, target, progress);
            yield return null;
        }

        RestorePlayerCamera();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!exitArmed || player == null)
        {
            return;
        }

        PlayerController enteringPlayer = other.GetComponentInParent<PlayerController>();
        if (enteringPlayer != player)
        {
            return;
        }

        HasPlayerExited = true;
        LockPlayerForShop();
    }
}
