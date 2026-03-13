using UnityEngine;

namespace SwarmCore2D.World
{
    public class ProceduralProps
    {
        float noiseScale = 0.15f;

        public void Generate(WorldChunk chunk, int chunkSize)
        {
            int index = 0;

            Vector2 basePos =
                new Vector2(
                    chunk.coord.x * chunkSize,
                    chunk.coord.y * chunkSize);

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    float noise =
                        Mathf.PerlinNoise(
                            (basePos.x + x) * noiseScale,
                            (basePos.y + y) * noiseScale);

                    if (noise < 0.75f)
                        continue;

                    Vector3 pos =
                        new Vector3(
                            basePos.x + x,
                            basePos.y + y,
                            0);

                    chunk.propMatrices[index++] =
                        Matrix4x4.TRS(
                            pos,
                            Quaternion.identity,
                            Vector3.one * 1.5f);
                }
            }

            chunk.propCount = index;
        }
    }
}