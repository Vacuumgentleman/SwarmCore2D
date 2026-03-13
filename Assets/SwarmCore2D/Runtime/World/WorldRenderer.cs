using UnityEngine;
using SwarmCore2D.World;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        public Mesh tileMesh;
        public Material groundMaterial;
        public Material propMaterial;

        const int BatchSize = 1023;

        Matrix4x4[] batch = new Matrix4x4[BatchSize];

        WorldChunkSystem chunkSystem;

        public void Initialize(WorldChunkSystem system)
        {
            chunkSystem = system;
        }

        void LateUpdate()
        {
            if (chunkSystem == null)
                return;

            foreach (var chunk in chunkSystem.GetChunks())
            {
                DrawBatch(
                    tileMesh,
                    groundMaterial,
                    chunk.groundMatrices,
                    chunk.groundCount
                );

                DrawBatch(
                    tileMesh,
                    propMaterial,
                    chunk.propMatrices,
                    chunk.propCount
                );
            }
        }

        void DrawBatch(
            Mesh mesh,
            Material material,
            Matrix4x4[] matrices,
            int count)
        {
            int index = 0;

            while (index < count)
            {
                int size = Mathf.Min(BatchSize, count - index);

                for (int i = 0; i < size; i++)
                    batch[i] = matrices[index + i];

                Graphics.DrawMeshInstanced(
                    mesh,
                    0,
                    material,
                    batch,
                    size
                );

                index += size;
            }
        }
    }
}