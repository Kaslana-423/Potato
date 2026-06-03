using System.Globalization;

public enum WeaponAttackStyle
{
    Unspecified,
    Slash,
    Thrust,
    Ranged
}

public abstract class ShopWeaponDefinition : ShopContentDefinition
{
    public sealed override ShopContentKind Kind => ShopContentKind.Weapon;

    public virtual WeaponAttackStyle AttackStyle => WeaponAttackStyle.Unspecified;
    public virtual float Damage => 0f;
    public virtual float AttackCooldown => 1f;
    public virtual float AttackRange => 0f;
    public virtual string ClassTags => string.Empty;
    public virtual string SpecialEffects => string.Empty;
    public virtual string RuntimePrefabResourcePath => string.Empty;

    public string LocalizedClassTags => ShopLocalization.GetWeaponClasses(ClassTags);

    public override string BuildStatLine()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<color=#55E875>{0:0.##}</color> 伤害\n<color=#55E875>{1:0.##}</color> 秒攻击间隔\n<color=#55E875>{2:0.##}</color> 攻击范围",
            Damage,
            AttackCooldown,
            AttackRange);
    }
}
