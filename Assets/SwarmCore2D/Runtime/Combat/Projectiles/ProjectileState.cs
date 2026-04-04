using UnityEngine;

namespace SwarmCore2D.Combat
{
    public class ProjectileState
    {
        public const int MaxProjectiles = 4096;

        public int count;

        public Vector2[] position = new Vector2[MaxProjectiles];
        public Vector2[] direction = new Vector2[MaxProjectiles];

        public float[] speed = new float[MaxProjectiles];
        public float[] damage = new float[MaxProjectiles];

        public float[] lifetime = new float[MaxProjectiles];
        public float[] maxDistance = new float[MaxProjectiles];

        public float[] size = new float[MaxProjectiles];
        public Vector2[] startPos = new Vector2[MaxProjectiles];

        public bool[] pierce = new bool[MaxProjectiles];

        public void Clear()
        {
            count = 0;
        }

        public int Spawn(
                Vector2 pos,
                Vector2 dir,
                float spd,
                float dmg,
                float life,
                float dist,
                bool pierceEnemies,
                float projectileSize
            )
        {
            if (count >= MaxProjectiles)
                return -1;

            int id = count++;

            position[id] = pos;
            direction[id] = dir;
            speed[id] = spd;
            damage[id] = dmg;
            lifetime[id] = life;
            maxDistance[id] = dist;
            startPos[id] = pos;
            pierce[id] = pierceEnemies;

            size[id] = projectileSize;

            return id;
        }

        public void Remove(int id)
        {
            int last = count - 1;

            position[id] = position[last];
            direction[id] = direction[last];
            speed[id] = speed[last];
            damage[id] = damage[last];
            lifetime[id] = lifetime[last];
            maxDistance[id] = maxDistance[last];
            startPos[id] = startPos[last];
            pierce[id] = pierce[last];

            count--;
        }
    }
}