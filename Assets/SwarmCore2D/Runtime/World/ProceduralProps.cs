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

            const float sortingFactor = 10f;
            const int sortingStep = 5;

            for (int i = 0; i < samples; i++)
            {
                int h = Hash(i, chunk.coord.x, chunk.coord.y);

                float rx = Frac(Hash(h, 1, 0) * 0.0001f);
                float ry = Frac(Hash(h, 2, 0) * 0.0001f);
                float rs = Frac(Hash(h, 4, 0) * 0.0001f);

                float px = rx * chunkSize;
                float py = ry * chunkSize;

                Vector2 worldPos = basePos + new Vector2(px, py);

                foreach (var prop in biome.props)
                {
                    float propRand = Frac(Hash(h, prop.GetInstanceID(), 0) * 0.0001f);

                    float noise = Mathf.PerlinNoise(
                        (worldPos.x + prop.noiseOffset.x) * prop.noiseScale,
                        (worldPos.y + prop.noiseOffset.y) * prop.noiseScale
                    );

                    if (noise < prop.minNoise)
                        continue;

                    if (propRand > prop.spawnChance)
                        continue;

                    if (IsTooClose(worldPos, prop.minDistance))
                        continue;

                    float scale = Mathf.Lerp(
                        prop.scaleRange.x,
                        prop.scaleRange.y,
                        rs
                    );

                    Vector3 pos = new Vector3(worldPos.x, worldPos.y, 0);

                    var matrix = Matrix4x4.TRS(
                        pos,
                        Quaternion.identity,
                        Vector3.one * scale
                    );

                    int ySort = -Mathf.RoundToInt(worldPos.y * sortingFactor);
                    ySort = (ySort / sortingStep) * sortingStep;

                    int finalSorting = prop.baseSorting + ySort;

                    PropBatchKey key = new PropBatchKey(
                        prop.mesh,
                        prop.material,
                        finalSorting
                    );

                    if (!chunk.propBatches.ContainsKey(key))
                        chunk.propBatches[key] = new List<Matrix4x4>();

                    chunk.propBatches[key].Add(matrix);

                    allPlaced.Add(pos);
                }
        }
    }
        int Hash(int a, int b, int c)
        {
            int h = a;
            h ^= b * 374761393;
            h ^= c * 668265263;
            h = (h ^ (h >> 13)) * 1274126177;
            return h;
        }

        float Frac(float v)
        {
            return v - Mathf.Floor(v);
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