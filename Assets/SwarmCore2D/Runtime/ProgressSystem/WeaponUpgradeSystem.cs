using UnityEngine;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public PlayerWeaponController weaponController;

    [Tooltip("Indice del arma a la que aplican los metodos de esta instancia")]
    public int weaponIndex = 0;

    void Start()
    {
        if (weaponController == null)
            weaponController = FindFirstObjectByType<PlayerWeaponController>();
    }

    public void IncreaseDamage(float amount)
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime != null)
            runtime.damage += amount;
    }

    public void ReduceCooldown(float percent)
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime != null)
            runtime.cooldown *= (1f - percent);
    }

    public void AddProjectile()
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime != null)
            runtime.amount++;
    }

    public void EnableDirectionUp()
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime == null)
            return;

        runtime.attackUp = true;
        weaponController.RebuildDirections(weaponIndex);
    }

    public void EnableAllDirections()
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime == null)
            return;

        runtime.attackUp = true;
        runtime.attackDown = true;
        runtime.attackLeft = true;
        runtime.attackRight = true;
        weaponController.RebuildDirections(weaponIndex);
    }

    public void IncreaseSize(float amount)
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime == null)
            return;

        runtime.hitRadius += amount;
        runtime.projectileSize += amount;
    }

    public void IncreasePierce(int amount)
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime != null)
            runtime.pierceCount += amount;
    }

    public void IncreaseKnockback(float amount)
    {
        var runtime = weaponController.GetRuntime(weaponIndex);
        if (runtime != null)
            runtime.knockback += amount;
    }
}