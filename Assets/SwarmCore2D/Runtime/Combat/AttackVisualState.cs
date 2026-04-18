using UnityEngine;

namespace SwarmCore2D.Combat
{
    public class AttackVisualState
    {
        public const int Capacity = 64;

        public Vector2[]  positions  = new Vector2[Capacity];
        public float[]    angles     = new float[Capacity];
        public float[]    scales     = new float[Capacity];
        public float[]    timer      = new float[Capacity];
        public int[]      frame      = new int[Capacity];
        public float[]    frameRate  = new float[Capacity];
        public int[]      frameCount = new int[Capacity];
        public bool[]     loop       = new bool[Capacity];
        public Material[] material   = new Material[Capacity];
        public bool[]     active     = new bool[Capacity];
        public int[]      activeList = new int[Capacity];
        public int        activeCount;

        int[] freeStack = new int[Capacity];
        int   freeTop;

        public AttackVisualState()
        {
            for (int i = 0; i < Capacity; i++)
                freeStack[i] = i;
            freeTop = Capacity;
        }

        public int Spawn(Vector2 pos, float angle, float scale,
                         Material mat, int frames, float rate, bool isLoop)
        {
            if (freeTop == 0) return -1;
            int id = freeStack[--freeTop];
            positions[id]  = pos;
            angles[id]     = angle;
            scales[id]     = scale;
            timer[id]      = 0f;
            frame[id]      = 0;
            frameRate[id]  = rate;
            frameCount[id] = Mathf.Max(1, frames);
            loop[id]       = isLoop;
            material[id]   = mat;
            active[id]     = true;
            activeList[activeCount++] = id;
            return id;
        }

        public void Deactivate(int id)
        {
            if (!active[id]) return;
            active[id] = false;
            for (int i = 0; i < activeCount; i++)
            {
                if (activeList[i] == id)
                {
                    activeList[i] = activeList[--activeCount];
                    break;
                }
            }
            freeStack[freeTop++] = id;
        }
    }
}
