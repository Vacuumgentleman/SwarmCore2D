using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.MultiplayerReady
{
    /// <summary>
    /// Validates deterministic simulation by comparing checksums.
    /// Useful for lockstep multiplayer debugging.
    /// </summary>
    public class LockstepValidator
    {
        uint lastChecksum;

        public bool Validate(SwarmState state)
        {
            uint checksum = SimulationChecksum.Compute(state);

            bool valid = checksum == lastChecksum || lastChecksum == 0;

            lastChecksum = checksum;

            return valid;
        }

        public uint GetChecksum(SwarmState state)
        {
            return SimulationChecksum.Compute(state);
        }

        public void Reset()
        {
            lastChecksum = 0;
        }
    }
}