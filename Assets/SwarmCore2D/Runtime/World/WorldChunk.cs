using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.World
{
    public class WorldChunk
    {
        public Vector2Int coord;

        public Dictionary<Material, List<Matrix4x4>> groundBatches;
        public Dictionary<PropBatchKey, List<Matrix4x4>> propBatches;

        public int biomeIndex;

        public WorldChunk(int capacity)
        {
            groundBatches = new Dictionary<Material, List<Matrix4x4>>();
            propBatches = new Dictionary<PropBatchKey, List<Matrix4x4>>();
        }

        public void Clear()
        {
            groundBatches.Clear();
            propBatches.Clear();
        }
    }
}