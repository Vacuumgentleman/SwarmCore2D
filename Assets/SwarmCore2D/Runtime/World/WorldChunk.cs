using UnityEngine;

namespace SwarmCore2D.World
{
    public class WorldChunk
    {
        public Vector2Int coord;

        public Matrix4x4[] groundMatrices;
        public Matrix4x4[] propMatrices;

        public int groundCount;
        public int propCount;

        public WorldChunk(int groundCap, int propCap)
        {
            groundMatrices = new Matrix4x4[groundCap];
            propMatrices = new Matrix4x4[propCap];
        }
    }
}