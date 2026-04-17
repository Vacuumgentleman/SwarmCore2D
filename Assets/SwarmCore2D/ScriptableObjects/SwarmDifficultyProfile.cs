using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    /// <summary>
    /// Controls enemy scaling and spawn difficulty over time.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SwarmDifficultyProfile",
        menuName = "SwarmCore2D/Configuration/Difficulty Profile"
    )]
    public class SwarmDifficultyProfile : ScriptableObject
    {
        [Header("Spawn")]

        [Tooltip("Base spawn rate per second.")]
        public float spawnRate = 2f;

        [Tooltip("Maximum enemies alive at once.")]
        public int maxEnemies = 500;

        [Header("Scaling")]

        [Tooltip("Spawn multiplier over time.")]
        public AnimationCurve spawnMultiplier =
            AnimationCurve.Linear(0, 1, 30, 3);

        [Tooltip("Enemy speed scaling.")]
        public AnimationCurve speedMultiplier =
            AnimationCurve.Linear(0, 1, 30, 1.5f);

        [Tooltip("Enemy health scaling.")]
        public AnimationCurve healthMultiplier =
            AnimationCurve.Linear(0, 1, 30, 2);
    }
}