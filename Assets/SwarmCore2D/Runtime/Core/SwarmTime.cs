namespace SwarmCore2D.Core
{
    /// <summary>
    /// Deterministic simulation time.
    /// Independent from Unity Time.
    /// </summary>
    public static class SwarmTime
    {
        /// <summary>
        /// Current simulation tick.
        /// </summary>
        public static int Tick { get; private set; }
        public static bool Paused = false;
        /// <summary>
        /// Fixed delta time used by the simulation.
        /// </summary>
        public static float FixedDelta => SwarmConstants.FixedDeltaTime;

        /// <summary>
        /// Total simulation time.
        /// </summary>
        public static float Time => Tick * FixedDelta;

        /// <summary>
        /// Advance simulation one tick.
        /// </summary>
        public static void Step()
        {
            if (Paused)
                return;

            Tick++;
        }

        /// <summary>
        /// Reset simulation time.
        /// </summary>
        public static void Reset()
        {
            Tick = 0;
        }
    }
}