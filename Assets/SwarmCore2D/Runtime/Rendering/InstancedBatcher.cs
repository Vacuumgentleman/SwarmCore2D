using UnityEngine;

namespace SwarmCore2D.Rendering
{
    /// <summary>
    /// Handles GPU instanced rendering batches.
    /// </summary>
    public class InstancedBatcher
    {
        const int BatchSize = 1023;

        Mesh mesh;
        Material material;

        public InstancedBatcher(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;
        }

        public void Draw(Matrix4x4[] matrices, int count)
        {
            int index = 0;

            while (index < count)
            {
                int batch = Mathf.Min(BatchSize, count - index);

                Graphics.DrawMeshInstanced(
                    mesh,
                    0,
                    material,
                    matrices,
                    batch,
                    null,
                    UnityEngine.Rendering.ShadowCastingMode.Off,
                    false
                );

                index += batch;
            }
        }
    }
}