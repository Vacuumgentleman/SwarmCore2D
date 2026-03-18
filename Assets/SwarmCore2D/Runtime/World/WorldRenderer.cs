using UnityEngine;
using SwarmCore2D.World;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        public Mesh tileMesh;

        [Header("Ground (Biomes)")]
        public Material[] groundMaterials; 
        
        [Header("Props (Multi Layer)")]
        public Material[] propMaterials;

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
                Material groundMat = GetGroundMaterial(chunk.biomeIndex);

                if (groundMat != null && chunk.groundCount > 0)
                {
                    DrawBatch(
                        tileMesh,
                        groundMat,
                        chunk.groundMatrices,
                        chunk.groundCount
                    );
                }

                for (int i = 0; i < chunk.propLayers; i++)
                {
                    if (propMaterials == null)
                        continue;

                    if (i >= propMaterials.Length)
                        continue;

                    var mat = propMaterials[i];

                    if (mat == null)
                        continue;

                    if (chunk.propCounts[i] == 0)
                        continue;

                    DrawBatch(
                        tileMesh,
                        mat,
                        chunk.propMatrices[i],
                        chunk.propCounts[i]
                    );
                }
            }
        }

        Material GetGroundMaterial(int biomeIndex)
        {
            if (groundMaterials == null || groundMaterials.Length == 0)
                return null;

            if (biomeIndex < 0 || biomeIndex >= groundMaterials.Length)
                return groundMaterials[0];

            return groundMaterials[biomeIndex];
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