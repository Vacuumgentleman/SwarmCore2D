using UnityEngine;

namespace SwarmCore2D.World
{
    public class BiomeSystem
    {
        BiomeData[] biomes;
        float globalScale = 0.02f;

        public BiomeSystem(BiomeData[] biomes)
        {
            this.biomes = biomes;
        }

        public BiomeData GetBiome(Vector2 worldPos, out int index)
        {
            float noise = Mathf.PerlinNoise(
                worldPos.x * globalScale,
                worldPos.y * globalScale
            );

            for (int i = 0; i < biomes.Length; i++)
            {
                var b = biomes[i];

                if (noise >= b.thresholdMin && noise <= b.thresholdMax)
                {
                    index = i;
                    return b;
                }
            }

            index = 0;
            return biomes[0];
        }
    }
}