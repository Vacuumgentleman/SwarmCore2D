using UnityEngine;
using SwarmCore2D.World;
using System.Collections.Generic;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        [Header("Ground")]
        public Mesh tileMesh;
        public Material[] groundMaterials;

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
                DrawGround(chunk);
                DrawProps(chunk);
            }
        }

        void DrawGround(WorldChunk chunk)
        {
            Material groundMat = GetGroundMaterial(chunk.biomeIndex);

            if (groundMat == null || chunk.groundCount == 0)
                return;

            DrawBatch(
                tileMesh,
                groundMat,
                chunk.groundMatrices,
                chunk.groundCount
            );
        }

        void DrawProps(WorldChunk chunk)
        {
            foreach (var pair in chunk.propBatches)
            {
                var prop = pair.Key;
                var matrices = pair.Value;

                if (prop.mesh == null || prop.material == null)
                    continue;

                DrawBatch(
                    prop.mesh,
                    prop.material,
                    matrices
                );
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

        void DrawBatch(
            Mesh mesh,
            Material material,
            List<Matrix4x4> matrices)
        {
            int count = matrices.Count;
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