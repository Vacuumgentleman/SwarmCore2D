using UnityEngine;

namespace SwarmCore2D.Replay
{
    /// <summary>
    /// Represents a deterministic input command executed on a specific tick.
    /// </summary>
    public struct InputCommand
    {
        public int tick;

        public Vector2 move;

        public bool action1;
        public bool action2;

        public InputCommand(
            int tick,
            Vector2 move,
            bool action1,
            bool action2)
        {
            this.tick = tick;
            this.move = move;
            this.action1 = action1;
            this.action2 = action2;
        }
    }
}