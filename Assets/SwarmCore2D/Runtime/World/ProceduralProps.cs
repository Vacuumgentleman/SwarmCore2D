using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.World
{
    public class ProceduralProps
    {
        List<Vector3> allPlaced = new List<Vector3>();

        public void Generate(WorldChunk chunk, int chunkSize, BiomeData biome)
        {
            allPlaced.Clear();
            chunk.propBatches.Clear();

            Vector2 basePos = new Vector2(
                chunk.coord.x * chunkSize,
                chunk.coord.y * chunkSize
            );

            int samples = chunkSize * chunkSize;

            for (int i = 0; i < samples; i++)
            {
                float px = Random.value * chunkSize;
                float py = Random.value * chunkSize;

                Vector2 worldPos = basePos + new Vector2(px, py);

                foreach (var prop in biome.props)
                {
                    float noise = Mathf.PerlinNoise(
                        (worldPos.x + prop.noiseOffset.x) * prop.noiseScale,
                        (worldPos.y + prop.noiseOffset.y) * prop.noiseScale
                    );

                    if (noise < prop.minNoise)
                        continue;

                    if (Random.value > prop.spawnChance)
                        continue;

                    if (IsTooClose(worldPos, prop.minDistance))
                        continue;

                    float scale = Random.Range(
                        prop.scaleRange.x,
                        prop.scaleRange.y
                    );

                    Vector3 pos = new Vector3(worldPos.x, worldPos.y, 0);

                    var matrix = Matrix4x4.TRS(
                        pos,
                        Quaternion.identity,
                        Vector3.one * scale
                    );

                    if (!chunk.propBatches.ContainsKey(prop))
                        chunk.propBatches[prop] = new List<Matrix4x4>();

                    chunk.propBatches[prop].Add(matrix);

                    allPlaced.Add(pos);
                }
            }
        }

        bool IsTooClose(Vector2 pos, float minDistance)
        {
            for (int i = 0; i < allPlaced.Count; i++)
            {
                if (Vector2.Distance(allPlaced[i], pos) < minDistance)
                    return true;
            }

            return false;
        }
    }
}