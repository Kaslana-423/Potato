using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class ArenaGateController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform arenaSpace;
    [SerializeField] private PlayerController player;
    [SerializeField] private BoxCollider2D topDoorCollider;
    [SerializeField] private BoxCollider2D bottomDoorCollider;

    [Header("Bottom Entrance")]
    [SerializeField] private Vector2 entranceStartLocal = new Vector2(0.09f, -5.35f);
    [SerializeField] private Vector2 entranceEndLocal = new Vector2(0.09f, -3.6f);
    [SerializeField, Min(0.05f)] private float entranceDurationSeconds = 0.65f;

    private bool exitArmed;

    public bool HasPlayerExited { get; private set; }
    public bool IsConfigured => arenaSpace != null
        && player != null
        && topDoorCollider != null
        && bottomDoorCollider != null;

    private void Awake()
    {
        BoxCollider2D exitTrigger = GetComponent<BoxCollider2D>();
        exitTrigger.isTrigger = true;
        CloseDoors();
    }

    private void OnValidate()
    {
        entranceDurationSeconds = Mathf.Max(0.05f, entranceDurationSeconds);
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
        topDoorCollider.enabled = true;
        bottomDoorCollider.enabled = false;
        player.SetControlEnabled(false);
        player.SetBoundaryEnabled(false);

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

        bottomDoorCollider.enabled = true;
        player.SetBoundaryEnabled(true);
        player.RestorePosition(end);
        player.SetControlEnabled(true);
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
        player.SetBoundaryEnabled(false);
        player.SetControlEnabled(true);
    }

    public void PrepareCombatWithoutEntrance()
    {
        HasPlayerExited = false;
        exitArmed = false;
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
