using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    /// <summary>
    /// Bridge between simulation and GPU rendering.
    /// </summary>
    public class SwarmRenderer : MonoBehaviour
    {
        [Header("Rendering")]
        public Mesh mesh;
        public Material material;

        RenderData renderData;
        InstancedBatcher batcher;

        SwarmState state;

        public void Initialize(SwarmState state)
        {
            this.state = state;

            renderData = new RenderData();
            batcher = new InstancedBatcher(mesh, material);
        }

        void LateUpdate()
        {
            if (state == null)
                return;

            renderData.Build(state);

            batcher.Draw(renderData.matrices, renderData.count);
        }
    }
}