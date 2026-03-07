using System.Collections.Generic;

namespace SwarmCore2D.Replay
{
    /// <summary>
    /// Plays back recorded input commands deterministically.
    /// </summary>
    public class ReplayPlayer
    {
        List<InputCommand> commands;

        int index;

        public ReplayPlayer(List<InputCommand> commands)
        {
            this.commands = commands;
            index = 0;
        }

        public bool TryGetCommand(int tick, out InputCommand command)
        {
            if (index >= commands.Count)
            {
                command = default;
                return false;
            }

            command = commands[index];

            if (command.tick == tick)
            {
                index++;
                return true;
            }

            command = default;
            return false;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}