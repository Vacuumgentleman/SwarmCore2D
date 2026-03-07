using System.Collections.Generic;

namespace SwarmCore2D.Replay
{
    /// <summary>
    /// Records input commands for replay.
    /// </summary>
    public class ReplayRecorder
    {
        List<InputCommand> commands;

        public IReadOnlyList<InputCommand> Commands => commands;

        public ReplayRecorder()
        {
            commands = new List<InputCommand>(1024);
        }

        public void Record(InputCommand command)
        {
            commands.Add(command);
        }

        public void Clear()
        {
            commands.Clear();
        }
    }
}