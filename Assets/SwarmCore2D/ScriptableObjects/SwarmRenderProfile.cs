using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    /// <summary>
    /// Rendering configuration for swarm entities.
    /// Controls instancing and visual settings.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SwarmRenderProfile",
        menuName = "SwarmCore2D/Configuration/Render Profile"
    )]
    public class SwarmRenderProfile : ScriptableObject
    {
        [Header("Instancing")]

        [Tooltip("Enable GPU instancing.")]
        public bool useInstancing = true;

        [Tooltip("Maximum instances per batch.")]
        public int batchSize = 1023;

        [Header("Rendering")]

        public Mesh entityMesh;

        public Material entityMaterial;

        [Header("Performance")]

        [Tooltip("Enable frustum culling.")]
        public bool useFrustumCulling = true;

        [Tooltip("Maximum visible entities.")]
        public int maxVisibleEntities = 2000;
    }
}