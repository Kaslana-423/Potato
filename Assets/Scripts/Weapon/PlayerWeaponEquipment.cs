using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerWeaponEquipment : MonoBehaviour
{
    private const float ShopRangeToSlashWorldUnits = 0.02f;
    private const string SlashTemplatePath = "Weapon/Templates/SlashWeaponTemplate";
    private const string ThrustTemplatePath = "Weapon/Templates/ThrustWeaponTemplate";
    private const string RangedTemplatePath = "Weapon/Templates/RangedWeaponTemplate";

    [Header("Runtime Root")]
    [SerializeField] private Transform weaponRoot;
    [SerializeField, Min(0f)] private float formationRadius = 0.55f;

    private readonly List<WeaponBase> runtimeWeapons = new List<WeaponBase>();
    private WeaponBag boundBag;
    private bool hasTakenControl;

    public IReadOnlyList<WeaponBase> RuntimeWeapons => runtimeWeapons;
    public int EquippedCount => runtimeWeapons.Count;
    public bool HasTakenControl => hasTakenControl;

    private void Awake()
    {
        AutoBindReferences();
    }

    private void OnValidate()
    {
        formationRadius = Mathf.Max(0f, formationRadius);
    }

    private void OnDestroy()
    {
        UnsubscribeFromBag();
    }

    public void Bind(WeaponBag bag, bool synchronizeImmediately)
    {
        if (boundBag == bag)
        {
            if (synchronizeImmediately && !hasTakenControl)
            {
                SynchronizeNow();
            }

            return;
        }

        UnsubscribeFromBag();
        boundBag = bag;
        if (boundBag != null)
        {
            boundBag.ContentsChanged += HandleBagContentsChanged;
        }

        if (synchronizeImmediately)
        {
            SynchronizeNow();
        }
    }

    [ContextMenu("Synchronize From Weapon Bag")]
    public void SynchronizeNow()
    {
        if (boundBag == null)
        {
            return;
        }

        AutoBindReferences();
        hasTakenControl = true;
        ClearRuntimeWeapons();

        int weaponCount = 0;
        foreach (ShopContentDefinition content in boundBag.Contents)
        {
            if (content is ShopWeaponDefinition)
            {
                weaponCount++;
            }
        }

        int weaponIndex = 0;
        foreach (ShopContentDefinition content in boundBag.Contents)
        {
            ShopWeaponDefinition definition = content as ShopWeaponDefinition;
            if (definition == null)
            {
                continue;
            }

            WeaponBase runtimeWeapon = CreateRuntimeWeapon(definition, weaponIndex, weaponCount);
            if (runtimeWeapon != null)
            {
                runtimeWeapons.Add(runtimeWeapon);
            }

            weaponIndex++;
        }
    }

    [ContextMenu("Auto Bind References")]
    public void AutoBindReferences()
    {
        if (weaponRoot == null)
        {
            Transform existingRoot = transform.Find("RuntimeWeapons");
            if (existingRoot != null)
            {
                weaponRoot = existingRoot;
            }
            else if (Application.isPlaying)
            {
                var rootObject = new GameObject("RuntimeWeapons");
                rootObject.layer = gameObject.layer;
                weaponRoot = rootObject.transform;
                weaponRoot.SetParent(transform, false);
            }
        }
    }

    private void HandleBagContentsChanged()
    {
        SynchronizeNow();
    }

    private void UnsubscribeFromBag()
    {
        if (boundBag != null)
        {
            boundBag.ContentsChanged -= HandleBagContentsChanged;
        }
    }

    private WeaponBase CreateRuntimeWeapon(
        ShopWeaponDefinition definition,
        int weaponIndex,
        int weaponCount)
    {
        if (definition == null || weaponRoot == null)
        {
            return null;
        }

        GameObject instance = TryInstantiateResourcePrefab(definition);
        if (instance == null)
        {
            instance = TryInstantiateAttackStyleTemplate(definition);
        }

        if (instance == null)
        {
            Debug.LogError($"No runtime weapon template is available for '{definition.Id}'.", this);
            return null;
        }

        instance.SetActive(false);
        instance.name = $"Equipped Weapon {weaponIndex + 1} - {definition.DisplayName}";
        instance.transform.SetParent(weaponRoot, false);
        instance.transform.localPosition = GetFormationPosition(weaponIndex, weaponCount);
        instance.transform.localRotation = Quaternion.identity;

        WeaponBase runtimeWeapon = instance.GetComponent<WeaponBase>();
        if (runtimeWeapon == null)
        {
            Debug.LogWarning(
                $"Weapon runtime prefab for '{definition.Id}' has no WeaponBase component.",
                this);
            DestroyRuntimeObject(instance);
            return null;
        }

        ApplyDefinition(runtimeWeapon, definition);
        instance.SetActive(true);
        return runtimeWeapon;
    }

    private GameObject TryInstantiateResourcePrefab(ShopWeaponDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.RuntimePrefabResourcePath))
        {
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>(definition.RuntimePrefabResourcePath);
        if (prefab == null)
        {
            Debug.LogWarning(
                $"Weapon runtime prefab was not found at Resources/{definition.RuntimePrefabResourcePath}.",
                this);
            return null;
        }

        return Instantiate(prefab, weaponRoot);
    }

    private GameObject TryInstantiateAttackStyleTemplate(ShopWeaponDefinition definition)
    {
        string resourcePath;
        switch (ResolveAttackStyle(definition))
        {
            case WeaponAttackStyle.Thrust:
                resourcePath = ThrustTemplatePath;
                break;
            case WeaponAttackStyle.Ranged:
                resourcePath = RangedTemplatePath;
                break;
            default:
                resourcePath = SlashTemplatePath;
                break;
        }

        GameObject template = Resources.Load<GameObject>(resourcePath);
        return template != null ? Instantiate(template, weaponRoot) : null;
    }

    private static WeaponAttackStyle ResolveAttackStyle(ShopWeaponDefinition definition)
    {
        if (definition.AttackStyle != WeaponAttackStyle.Unspecified)
        {
            return definition.AttackStyle;
        }

        foreach (ShopContentDefinition content in ShopContentCatalog.All)
        {
            ShopWeaponDefinition familyWeapon = content as ShopWeaponDefinition;
            if (familyWeapon != null
                && familyWeapon.AttackStyle != WeaponAttackStyle.Unspecified
                && string.Equals(familyWeapon.FamilyId, definition.FamilyId, StringComparison.OrdinalIgnoreCase))
            {
                return familyWeapon.AttackStyle;
            }
        }

        string tags = definition.ClassTags ?? string.Empty;
        if (ContainsTag(tags, "Gun") || ContainsTag(tags, "Ranged") || ContainsTag(tags, "Projectile"))
        {
            return WeaponAttackStyle.Ranged;
        }

        if (ContainsTag(tags, "Precise")
            && !ContainsTag(tags, "Blade")
            && !ContainsTag(tags, "Unarmed"))
        {
            return WeaponAttackStyle.Thrust;
        }

        return WeaponAttackStyle.Slash;
    }

    private static bool ContainsTag(string tags, string expectedTag)
    {
        string[] entries = tags.Split(',');
        foreach (string entry in entries)
        {
            if (string.Equals(entry.Trim(), expectedTag, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void ApplyDefinition(WeaponBase runtimeWeapon, ShopWeaponDefinition definition)
    {
        runtimeWeapon.weaponName = definition.LocalizedDisplayName;
        runtimeWeapon.description = definition.LocalizedDescription;
        runtimeWeapon.attackPower = Mathf.Max(0f, definition.Damage);
        runtimeWeapon.attackCooldown = Mathf.Max(0.01f, definition.AttackCooldown);
        runtimeWeapon.attackRange = runtimeWeapon is Melee_SlashWeapon
            ? Mathf.Max(0.01f, definition.AttackRange * ShopRangeToSlashWorldUnits)
            : Mathf.Max(0.01f, definition.AttackRange);
        runtimeWeapon.rarity = (WeaponRarity)Mathf.Clamp((int)definition.Rarity - 1, 0, 3);

        Sprite icon = definition.LoadIcon();
        if (icon == null)
        {
            return;
        }

        runtimeWeapon.weaponIcon = icon;
        SpriteRenderer renderer = runtimeWeapon.GetComponentInChildren<SpriteRenderer>(true);
        if (renderer != null)
        {
            renderer.sprite = icon;
        }
    }

    private Vector3 GetFormationPosition(int weaponIndex, int weaponCount)
    {
        if (weaponCount <= 1 || formationRadius <= 0f)
        {
            return new Vector3(formationRadius, 0f, 0f);
        }

        float angle = Mathf.PI * 2f * weaponIndex / weaponCount;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * formationRadius;
    }

    private void ClearRuntimeWeapons()
    {
        foreach (WeaponBase weapon in runtimeWeapons)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(false);
                DestroyRuntimeObject(weapon.gameObject);
            }
        }

        runtimeWeapons.Clear();
    }

    private static void DestroyRuntimeObject(GameObject instance)
    {
        if (instance == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(instance);
        }
        else
        {
            DestroyImmediate(instance);
        }
    }
}
