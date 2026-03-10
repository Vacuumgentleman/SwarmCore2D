using System;
using UnityEngine;

namespace SwarmCore2D.Core
{
    /// <summary>
    /// Runs a deterministic fixed tick simulation independent from framerate.
    /// </summary>
    public class FixedTickRunner
    {
        public event Action<float> OnTick;
        public event Action<float> OnFrame;

        public float TickRate { get; private set; }
        public float TickDelta { get; private set; }

        private float accumulator;
        private int maxTicksPerFrame = 5;

        public FixedTickRunner(float tickRate = 60f)
        {
            SetTickRate(tickRate);
        }

        public void SetTickRate(float tickRate)
        {
            TickRate = tickRate;
            TickDelta = 1f / tickRate;
        }

        public void Update(float deltaTime)
        {
            accumulator += deltaTime;

            int tickCount = 0;

            while (accumulator >= TickDelta && tickCount < maxTicksPerFrame)
            {
                OnTick?.Invoke(TickDelta);
                SwarmTime.Step();
                
                accumulator -= TickDelta;
                tickCount++;
            }

            OnFrame?.Invoke(deltaTime);
        }

        public void Reset()
        {
            accumulator = 0f;
        }

        public void SetMaxTicksPerFrame(int max)
        {
            maxTicksPerFrame = Mathf.Max(1, max);
        }
    }
}