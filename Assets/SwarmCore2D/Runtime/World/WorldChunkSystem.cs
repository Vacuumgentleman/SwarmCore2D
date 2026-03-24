using System.Collections.Generic;
using UnityEngine;

namespace SwarmCore2D.World
{
    public class WorldChunkSystem
    {
        Dictionary<Vector2Int, WorldChunk> chunks =
            new Dictionary<Vector2Int, WorldChunk>();

        int chunkSize = 32;
        int renderDistance = 2;

        ProceduralGround groundGen;
        ProceduralProps propGen;
        BiomeSystem biomeSystem;

        public WorldChunkSystem(BiomeData[] biomes)
        {
            groundGen = new ProceduralGround();
            propGen = new ProceduralProps();
            biomeSystem = new BiomeSystem(biomes);
        }

        Vector2Int GetChunk(Vector2 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / chunkSize),
                Mathf.FloorToInt(pos.y / chunkSize)
            );
        }

        public void Update(Vector2 playerPos)
        {
            Vector2Int center = GetChunk(playerPos);

            GenerateChunks(center);
            UnloadChunks(center);
        }

        void GenerateChunks(Vector2Int center)
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    Vector2Int coord = center + new Vector2Int(x, y);

                    if (!chunks.ContainsKey(coord))
                        CreateChunk(coord);
                }
            }
        }

        void UnloadChunks(Vector2Int center)
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();

            foreach (var pair in chunks)
            {
                Vector2Int coord = pair.Key;

                int dx = Mathf.Abs(coord.x - center.x);
                int dy = Mathf.Abs(coord.y - center.y);

                if (dx > renderDistance + 1 || dy > renderDistance + 1)
                    toRemove.Add(coord);
            }

            foreach (var coord in toRemove)
                chunks.Remove(coord);
        }

        void CreateChunk(Vector2Int coord)
        {

            WorldChunk chunk = new WorldChunk(1024);

            chunk.coord = coord;

            Vector2 centerPos = new Vector2(
                (coord.x + 0.5f) * chunkSize,
                (coord.y + 0.5f) * chunkSize
            );

            var biome = biomeSystem.GetBiome(centerPos, out chunk.biomeIndex);


            groundGen.Generate(chunk, chunkSize, biomeSystem);
            propGen.Generate(chunk, chunkSize, biome);

            chunks.Add(coord, chunk);
        }
        public IEnumerable<WorldChunk> GetChunks()
        {
            return chunks.Values;
        }
    }
}