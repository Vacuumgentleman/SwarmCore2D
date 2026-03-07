using System.Collections.Generic;

namespace SwarmCore2D.Replay
{
    /// <summary>
    /// Stores deterministic input commands indexed by simulation tick.
    /// </summary>
    public class InputBuffer
    {
        Dictionary<int, InputCommand> commands =
            new Dictionary<int, InputCommand>();

        public void Add(InputCommand command)
        {
            commands[command.tick] = command;
        }

        public bool TryGet(int tick, out InputCommand command)
        {
            return commands.TryGetValue(tick, out command);
        }

        public void Clear()
        {
            commands.Clear();
        }
    }
}