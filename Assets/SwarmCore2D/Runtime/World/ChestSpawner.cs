using UnityEngine;
using SwarmCore2D.Core;

public class ChestSpawner : MonoBehaviour
{
    public static ChestSpawner Instance;

    public ChestSpawnProfile profile;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void TrySpawnChest(Vector2 position, uint seed)
    {
        if (profile == null || profile.tiers == null || profile.tiers.Length == 0) return;

        var rng = new DeterministicRNG(seed);

        if (rng.NextFloat() > profile.baseSpawnChance) return;

        ChestData selected = SelectTier(rng);
        if (selected == null || selected.prefab == null) return;

        GameObject go = Instantiate(selected.prefab, position, Quaternion.identity);
        var controller = go.AddComponent<ChestController>();
        controller.data = selected;
    }

    ChestData SelectTier(DeterministicRNG rng)
    {
        float total = 0f;
        foreach (var t in profile.tiers)
            total += Mathf.Max(0f, t.weight);

        if (total <= 0f) return profile.tiers[0].data;

        float roll = rng.NextFloat() * total;
        float cumulative = 0f;

        foreach (var t in profile.tiers)
        {
            cumulative += Mathf.Max(0f, t.weight);
            if (roll <= cumulative) return t.data;
        }

        return profile.tiers[profile.tiers.Length - 1].data;
    }
}
