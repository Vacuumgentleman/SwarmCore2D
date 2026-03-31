using UnityEngine;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public PlayerWeaponController player;

    public void IncreaseDamage(float amount)
    {
        player.runtime.damage += amount;
    }

    public void ReduceCooldown(float percent)
    {
        player.runtime.cooldown *= (1f - percent);
    }

    public void AddProjectile()
    {
        player.runtime.projectileCount++;
    }

    public void EnableDirectionUp()
    {
        player.runtime.attackUp = true;
        player.RebuildDirections();
    }

    public void EnableAllDirections()
    {
        player.runtime.attackUp = true;
        player.runtime.attackDown = true;
        player.runtime.attackLeft = true;
        player.runtime.attackRight = true;

        player.RebuildDirections();
    }
}