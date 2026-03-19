using UnityEngine;
using SwarmCore2D.World;
using System.Collections.Generic;

namespace SwarmCore2D.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        public Mesh tileMesh;
        public Material[] groundMaterials;

        const int BatchSize = 1023;

        Matrix4x4[] batch = new Matrix4x4[BatchSize];

        MaterialPropertyBlock mpb;

        WorldChunkSystem chunkSystem;

        public void Initialize(WorldChunkSystem system)
        {
            chunkSystem = system;
            mpb = new MaterialPropertyBlock();
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
            Material mat = groundMaterials[chunk.biomeIndex];

            if (mat == null || chunk.groundCount == 0)
                return;

            Graphics.DrawMeshInstanced(
                tileMesh,
                0,
                mat,
                chunk.groundMatrices,
                chunk.groundCount
            );
        }

        void DrawProps(WorldChunk chunk)
        {
            foreach (var pair in chunk.propBatches)
            {
                var prop = pair.Key;
                var list = pair.Value;

                if (prop.mesh == null || prop.material == null)
                    continue;

                DrawBatch(prop, list);
            }
        }

        void DrawBatch(PropData prop, List<PropInstance> list)
        {
            int count = list.Count;
            int index = 0;

            while (index < count)
            {
                int size = Mathf.Min(BatchSize, count - index);

                float[] frames = new float[size];
                float[] flips = new float[size];
                Vector4[] tints = new Vector4[size];

                for (int i = 0; i < size; i++)
                {
                    var inst = list[index + i];

                    batch[i] = inst.matrix;
                    frames[i] = inst.frame;
                    flips[i] = inst.flip;
                    tints[i] = inst.tint;
                }

                mpb.Clear();
                mpb.SetFloatArray("_Frame", frames);
                mpb.SetFloatArray("_Flip", flips);
                mpb.SetVectorArray("_Tint", tints);

                Graphics.DrawMeshInstanced(
                    prop.mesh,
                    0,
                    prop.material,
                    batch,
                    size,
                    mpb
                );

                index += size;
            }
        }
    }
}