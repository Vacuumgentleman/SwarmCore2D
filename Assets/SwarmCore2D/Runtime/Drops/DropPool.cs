using UnityEngine;

namespace SwarmCore2D.Drops
{
    public class DropPool
    {
        public const int Capacity = 256;

        public Vector2[]  positions  = new Vector2[Capacity];
        public DropData[] data       = new DropData[Capacity];
        public float[]    animTimer  = new float[Capacity];
        public int[]      frame      = new int[Capacity];
        public bool[]     active     = new bool[Capacity];
        public int[]      activeList = new int[Capacity];
        public int        activeCount;

        int[] freeStack = new int[Capacity];
        int   freeTop;
        int   nextId;

        public int Spawn(Vector2 pos, DropData dropData)
        {
            int id = Allocate();
            if (id < 0) return -1;

            positions[id]  = pos;
            data[id]       = dropData;
            animTimer[id]  = 0f;
            frame[id]      = 0;
            active[id]     = true;
            activeList[activeCount++] = id;
            return id;
        }

        public void Despawn(int id)
        {
            active[id] = false;
            data[id]   = null;
            Free(id);

            // swap-and-pop
            for (int i = 0; i < activeCount; i++)
            {
                if (activeList[i] == id)
                {
                    activeList[i] = activeList[--activeCount];
                    break;
                }
            }
        }

        int Allocate()
        {
            if (freeTop > 0)
                return freeStack[--freeTop];
            if (nextId < Capacity)
                return nextId++;
            return -1;
        }

        void Free(int id)
        {
            if (freeTop < Capacity)
                freeStack[freeTop++] = id;
        }
    }
}
