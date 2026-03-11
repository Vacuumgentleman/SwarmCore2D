using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    public class SwarmRenderer : MonoBehaviour
    {
        [Header("Rendering")]
        public Mesh mesh;
        public Material material;

        [Header("Sprite")]
        [Range(0.1f, 5f)]
        public float spriteScale = 0.6f;

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

            ApplyScale(renderData.matrices, renderData.count);

            batcher.Draw(
                renderData.matrices,
                renderData.frames,
                renderData.flips,
                renderData.count
            );
        }

        void ApplyScale(Matrix4x4[] matrices, int count)
        {
            Vector3 scale = Vector3.one * spriteScale;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = matrices[i].GetColumn(3);
                matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, scale);
            }
        }
    }
}