using UnityEngine;

namespace SwarmCore2D.World
{
    public struct PropBatchKey
    {
        public readonly Mesh mesh;
        public readonly Material material;
        public readonly int sortingOrder;

        public PropBatchKey(Mesh mesh, Material material, int sortingOrder)
        {
            this.mesh = mesh;
            this.material = material;
            this.sortingOrder = sortingOrder;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PropBatchKey))
                return false;

            var other = (PropBatchKey)obj;

            return mesh == other.mesh &&
                   material == other.material &&
                   sortingOrder == other.sortingOrder;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 486187739;

                hash = hash * 16777619 ^ (mesh ? mesh.GetInstanceID() : 0);
                hash = hash * 16777619 ^ (material ? material.GetInstanceID() : 0);
                hash = hash * 16777619 ^ sortingOrder;

                return hash;
            }
        }
    }
}