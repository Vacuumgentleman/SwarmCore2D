using UnityEngine;

namespace SwarmCore2D.ScriptableObjects
{
    /// <summary>
    /// Configuración de rendering para SwarmCore2D.
    /// Permite ajustar el rendimiento según la plataforma destino (PC, móvil, online).
    /// </summary>
    [CreateAssetMenu(
        fileName = "RenderProfile",
        menuName = "SwarmCore2D/Configuration/Render Profile"
    )]
    public class SwarmRenderProfile : ScriptableObject
    {
        [Header("Performance")]

        [Tooltip("Máximo de entidades renderizadas por frame. Reducir para móvil o versiones online.")]
        public int maxVisibleEntities = 2000;

        [Tooltip("Tamaño de batch para GPU instancing. Máximo 1023 (límite de Unity). Reducir en dispositivos con poca VRAM.")]
        [Range(64, 1023)]
        public int batchSize = 1023;

        [Header("Optimization")]

        [Tooltip("Omite el rendering de entidades fuera del área visible de la cámara.")]
        public bool useFrustumCulling = true;
    }
}
