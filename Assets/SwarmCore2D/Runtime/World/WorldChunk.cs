using UnityEngine;

namespace SwarmCore2D.World
{
    public class WorldChunk
    {
        public Vector2Int coord;

        public Matrix4x4[] groundMatrices;
        public int groundCount;

        public Matrix4x4[][] propMatrices;
        public int[] propCounts;

        public int propLayers;

        public int biomeIndex;

        public WorldChunk(int groundCap, int propLayers, int propCapPerLayer)
        {
            groundMatrices = new Matrix4x4[groundCap];
            groundCount = 0;

            this.propLayers = propLayers;

            propMatrices = new Matrix4x4[propLayers][];
            propCounts = new int[propLayers];

            for (int i = 0; i < propLayers; i++)
            {
                propMatrices[i] = new Matrix4x4[propCapPerLayer];
                propCounts[i] = 0;
            }
        }

        public void Clear()
        {
            groundCount = 0;

            for (int i = 0; i < propLayers; i++)
                propCounts[i] = 0;
        }
    }
}