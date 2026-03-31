using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    public enum UpgradeType
    {
        Damage,
        AttackSpeed,
        ProjectileCount,
        UnlockDirectionUp,
        UnlockDirectionDown,
        UnlockDirectionLeft,
        UnlockDirectionRight,
        Heal
    }

    public UpgradeType type;

    public float value = 1f;
}