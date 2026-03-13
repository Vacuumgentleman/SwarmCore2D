using UnityEngine;

namespace SwarmCore2D.World
{
    public class ProceduralGround
    {
        float tileSize = 1f;

        public void Generate(WorldChunk chunk, int chunkSize)
        {
            int index = 0;

            Vector2 basePos =
                new Vector2(
                    chunk.coord.x * chunkSize,
                    chunk.coord.y * chunkSize
                );

            float half = tileSize * 0.5f;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3 pos = new Vector3(
                        basePos.x + x + half,
                        basePos.y + y + half,
                        0f
                    );

                    // rotación procedural estable
                    int hash =
                        (x * 73856093) ^
                        (y * 19349663) ^
                        (chunk.coord.x * 83492791) ^
                        (chunk.coord.y * 1234567);

                    float rot = (hash & 3) * 90f;

                    chunk.groundMatrices[index] =
                        Matrix4x4.TRS(
                            pos,
                            Quaternion.Euler(0, 0, rot),
                            Vector3.one * tileSize
                        );

                    index++;
                }
            }

            chunk.groundCount = index;
        }
    }
}