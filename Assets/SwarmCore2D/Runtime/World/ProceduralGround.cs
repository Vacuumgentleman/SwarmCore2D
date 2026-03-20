using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.World
{
    public class ProceduralGround
    {
        float tileSize = 1f;

        public void Generate(WorldChunk chunk, int chunkSize, BiomeSystem biomeSystem)
        {
            chunk.groundBatches.Clear();

            Vector2 basePos = new Vector2(
                chunk.coord.x * chunkSize,
                chunk.coord.y * chunkSize
            );

            float half = tileSize * 0.5f;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector2 worldPos = new Vector2(
                        basePos.x + x,
                        basePos.y + y
                    );

                    var biome = biomeSystem.GetBiome(worldPos, out _);
                    Material mat = biome.groundMaterial;

                    if (mat == null)
                        continue;

                    Vector3 pos = new Vector3(
                        worldPos.x + half,
                        worldPos.y + half,
                        0f
                    );

                    int hash =
                        (x * 73856093) ^
                        (y * 19349663) ^
                        (chunk.coord.x * 83492791) ^
                        (chunk.coord.y * 1234567);

                    float rot = (hash & 3) * 90f;

                    var matrix = Matrix4x4.TRS(
                        pos,
                        Quaternion.Euler(0, 0, rot),
                        Vector3.one * tileSize
                    );

                    if (!chunk.groundBatches.ContainsKey(mat))
                        chunk.groundBatches[mat] = new List<Matrix4x4>();

                    chunk.groundBatches[mat].Add(matrix);
                }
            }
        }
    }
}