using UnityEngine;

namespace SwarmCore2D.Combat
{
    public class AttackVisualSystem
    {
        public readonly AttackVisualState state = new AttackVisualState();

        public int Spawn(Vector2 pos, float angle, float scale,
                         Material mat, int frameCount, float frameRate, bool loop = false)
            => state.Spawn(pos, angle, scale, mat, frameCount, frameRate, loop);

        public void UpdatePosition(int id, Vector2 pos)
        {
            if (id >= 0 && id < AttackVisualState.Capacity && state.active[id])
                state.positions[id] = pos;
        }

        public void UpdateScale(int id, float scale)
        {
            if (id >= 0 && id < AttackVisualState.Capacity && state.active[id])
                state.scales[id] = scale;
        }

        public void Deactivate(int id) => state.Deactivate(id);

        public void Update(float dt)
        {
            for (int a = state.activeCount - 1; a >= 0; a--)
            {
                int id = state.activeList[a];

                state.timer[id] += dt;
                float frameDuration = state.frameRate[id] > 0f ? 1f / state.frameRate[id] : 0.083f;

                if (state.timer[id] >= frameDuration)
                {
                    state.timer[id] -= frameDuration;
                    state.frame[id]++;

                    if (state.frame[id] >= state.frameCount[id])
                    {
                        if (state.loop[id])
                            state.frame[id] = 0;
                        else
                            state.Deactivate(id);
                    }
                }
            }
        }
    }
}
