using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.World
{
    public class WorldChunk
    {
        public Vector2Int coord;

        public Matrix4x4[] groundMatrices;
        public int groundCount;

        public Dictionary<PropData, List<PropInstance>> propBatches;

        public int biomeIndex;

        public WorldChunk(int groundCap)
        {
            groundMatrices = new Matrix4x4[groundCap];
            groundCount = 0;

            propBatches = new Dictionary<PropData, List<PropInstance>>();
        }

        public void Clear()
        {
            groundCount = 0;
            propBatches.Clear();
        }
    }
}