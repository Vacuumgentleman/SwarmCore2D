using UnityEngine;
using SwarmCore2D.World;
using System.Collections.Generic;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        [Header("Ground")]
        public Mesh tileMesh;

        const int BatchSize = 1023;

        Matrix4x4[] batch = new Matrix4x4[BatchSize];

        WorldChunkSystem chunkSystem;

        Dictionary<(Material, int), Material> materialCache =
            new Dictionary<(Material, int), Material>();

        public void Initialize(WorldChunkSystem system)
        {
            chunkSystem = system;
        }

        void LateUpdate()
        {
            if (chunkSystem == null)
                return;

            Dictionary<Material, List<Matrix4x4>> globalBatches =
                new Dictionary<Material, List<Matrix4x4>>();

            foreach (var chunk in chunkSystem.GetChunks())
            {
                foreach (var pair in chunk.groundBatches)
                {
                    var mat = pair.Key;
                    var matrices = pair.Value;

                    if (!globalBatches.ContainsKey(mat))
                        globalBatches[mat] = new List<Matrix4x4>();

                    globalBatches[mat].AddRange(matrices);
                }

                DrawProps(chunk);
            }

            foreach (var pair in globalBatches)
            {
                DrawBatch(
                    tileMesh,
                    pair.Key,
                    pair.Value
                );
            }
        }

        void DrawProps(WorldChunk chunk)
        {
            foreach (var pair in chunk.propBatches)
            {
                var key = pair.Key;
                var matrices = pair.Value;

                if (key.mesh == null || key.material == null)
                    continue;

                Material mat = GetMaterialWithSorting(
                    key.material,
                    key.sortingOrder
                );

                DrawBatch(
                    key.mesh,
                    mat,
                    matrices
                );
            }
        }

        Material GetMaterialWithSorting(Material baseMat, int sortingOrder)
        {
            var cacheKey = (baseMat, sortingOrder);

            if (materialCache.TryGetValue(cacheKey, out var mat))
                return mat;

            mat = new Material(baseMat);
            mat.renderQueue = 3000 + sortingOrder;

            materialCache[cacheKey] = mat;

            return mat;
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