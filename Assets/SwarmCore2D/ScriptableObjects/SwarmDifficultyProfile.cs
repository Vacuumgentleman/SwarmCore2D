using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    [System.Serializable]
    public class SpawnPhase
    {
        [Tooltip("Segundos desde el inicio de la partida en que esta fase se activa.")]
        public float startTime = 0f;

        [Tooltip("Enemigos que pueden spawnear en esta fase. Se elige uno al azar.")]
        public EnemyData[] enemyPool;

        [Min(0.1f)]
        [Tooltip("Segundos entre cada spawn. Mínimo 0.1s.")]
        public float spawnInterval = 2f;
    }

    /// <summary>
    /// Define las fases de dificultad de la partida.
    /// Cada fase controla qué enemigos spawnean y con qué frecuencia.
    /// Los multiplicadores de stats escalan globalmente con el tiempo mediante curvas.
    /// </summary>
    [CreateAssetMenu(
        fileName = "DifficultyProfile",
        menuName = "SwarmCore2D/Configuration/Difficulty Profile"
    )]
    public class SwarmDifficultyProfile : ScriptableObject
    {
        [Tooltip("Fases ordenadas por startTime. La fase activa es la última cuyo startTime <= tiempo transcurrido.")]
        public SpawnPhase[] phases;

        [Header("Stat Scaling Over Time")]
        [Tooltip("Multiplicador de vida de los enemigos. Eje X = segundos de partida, eje Y = multiplicador.")]
        public AnimationCurve healthMultiplier = AnimationCurve.Linear(0, 1, 300, 3);

        [Tooltip("Multiplicador de velocidad de los enemigos. Eje X = segundos de partida, eje Y = multiplicador.")]
        public AnimationCurve speedMultiplier = AnimationCurve.Linear(0, 1, 300, 2);

        /// <summary>
        /// Devuelve la fase activa según el tiempo transcurrido en segundos.
        /// </summary>
        public SpawnPhase GetActivePhase(float elapsedSeconds)
        {
            if (phases == null || phases.Length == 0)
                return null;

            SpawnPhase active = phases[0];
            foreach (var phase in phases)
            {
                if (elapsedSeconds >= phase.startTime)
                    active = phase;
            }
            return active;
        }

        public float GetHealthMultiplier(float elapsedSeconds)
        {
            return healthMultiplier != null ? Mathf.Max(0.01f, healthMultiplier.Evaluate(elapsedSeconds)) : 1f;
        }

        public float GetSpeedMultiplier(float elapsedSeconds)
        {
            return speedMultiplier != null ? Mathf.Max(0.01f, speedMultiplier.Evaluate(elapsedSeconds)) : 1f;
        }
    }
}
