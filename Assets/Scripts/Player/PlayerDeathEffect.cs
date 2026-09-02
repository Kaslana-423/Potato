using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerDeathEffect : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float duration = 0.85f;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.18f, 0.18f, 1f);
    [SerializeField, Range(0f, 360f)] private float spinDegrees = 120f;
    [SerializeField, Range(0.01f, 1f)] private float finalScale = 0.12f;

    private Coroutine routine;

    public void Play(Action completed)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        FreezePlayerGameplay();
        routine = StartCoroutine(PlayRoutine(completed));
    }

    private void FreezePlayerGameplay()
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        PlayerVisuals visuals = GetComponentInChildren<PlayerVisuals>(true);
        if (visuals != null)
        {
            visuals.enabled = false;
        }

        PlayerWeaponEquipment equipment = GetComponent<PlayerWeaponEquipment>();
        if (equipment != null)
        {
            foreach (WeaponBase weapon in equipment.RuntimeWeapons)
            {
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(false);
                }
            }

            equipment.enabled = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D playerCollider in colliders)
        {
            playerCollider.enabled = false;
        }

        Animator[] animators = GetComponentsInChildren<Animator>(true);
        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.simulated = false;
        }
    }

    private IEnumerator PlayRoutine(Action completed)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        Color[] originalColors = new Color[renderers.Length];
        for (int index = 0; index < renderers.Length; index++)
        {
            originalColors[index] = renderers[index].color;
        }

        Vector3 originalScale = transform.localScale;
        Quaternion originalRotation = transform.localRotation;
        float elapsed = 0f;
        float safeDuration = Mathf.Max(0.1f, duration);

        while (elapsed < safeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / safeDuration);
            float flash = (1f - progress) * (0.45f + Mathf.Abs(Mathf.Sin(progress * Mathf.PI * 5f)) * 0.55f);
            float fade = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.35f, 1f, progress));

            for (int index = 0; index < renderers.Length; index++)
            {
                if (renderers[index] == null)
                {
                    continue;
                }

                Color color = Color.Lerp(originalColors[index], hitFlashColor, flash);
                color.a = originalColors[index].a * fade;
                renderers[index].color = color;
            }

            float scale = Mathf.Lerp(1f, finalScale, progress * progress);
            transform.localScale = originalScale * scale;
            transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, spinDegrees * progress);
            yield return null;
        }

        routine = null;
        completed?.Invoke();
    }
}
