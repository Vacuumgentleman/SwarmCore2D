using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.World
{
    public class ProceduralProps
    {
        Dictionary<PropData, List<Vector3>> placed =
            new Dictionary<PropData, List<Vector3>>();

        public void Generate(WorldChunk chunk, int chunkSize, BiomeSystem biomeSystem)
        {
            placed.Clear();

            for (int i = 0; i < chunk.propLayers; i++)
                chunk.propCounts[i] = 0;

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

                var biome = biomeSystem.GetBiome(worldPos, out _);

                foreach (var prop in biome.props)
                {
                    if (!placed.ContainsKey(prop))
                        placed[prop] = new List<Vector3>();

                    float noise = Mathf.PerlinNoise(
                        (worldPos.x + prop.noiseOffset.x) * prop.noiseScale,
                        (worldPos.y + prop.noiseOffset.y) * prop.noiseScale
                    );
                    
                    if (noise < prop.minNoise)
                        continue;

                    if (Random.value > prop.spawnChance)
                        continue;

                    if (IsTooClose(prop, worldPos))
                        continue;

                    float scale = Random.Range(
                        prop.scaleRange.x,
                        prop.scaleRange.y
                    );

                    Vector3 pos = new Vector3(worldPos.x, worldPos.y, 0);

                    int layer = prop.layerIndex;

                    if (layer < 0 || layer >= chunk.propLayers)
                        continue;

                    int index = chunk.propCounts[layer];

                    if (index >= chunk.propMatrices[layer].Length)
                        continue;

                    chunk.propMatrices[layer][index] =
                        Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * scale);

                    chunk.propCounts[layer]++;

                    placed[prop].Add(pos);
                }
            }
        }

        bool IsTooClose(PropData prop, Vector2 pos)
        {
            if (!placed.ContainsKey(prop))
                return false;

            foreach (var p in placed[prop])
            {
                if (Vector2.Distance(p, pos) < prop.minDistance)
                    return true;
            }

            return false;
        }
    }
}