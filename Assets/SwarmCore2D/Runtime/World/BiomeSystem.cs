using UnityEngine;

namespace SwarmCore2D.World
{
    public class BiomeSystem
    {
        BiomeData[] biomes;
        float globalScale = 0.005f;

        float offsetX;
        float offsetY;

        public BiomeSystem(BiomeData[] biomes)
        {
            this.biomes = biomes;

            // 🔴 Offset aleatorio para romper patrones de grilla
            offsetX = Random.Range(-10000f, 10000f);
            offsetY = Random.Range(-10000f, 10000f);
        }

        public BiomeData GetBiome(Vector2 worldPos, out int index)
        {
            float noise = Mathf.PerlinNoise(
                (worldPos.x + offsetX) * globalScale,
                (worldPos.y + offsetY) * globalScale
            );

            // suavizado leve
            noise = Mathf.Pow(noise, 1.5f);

            index = Mathf.FloorToInt(noise * biomes.Length);

            if (index >= biomes.Length)
                index = biomes.Length - 1;

            return biomes[index];
        }
    }
}