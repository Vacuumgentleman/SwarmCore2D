using UnityEngine;
using SwarmCore2D.Simulation;

public class SwarmDebugRenderer : MonoBehaviour
{
    public SwarmSimulationController simulation;

    void OnDrawGizmos()
    {
        // evitar ejecución fuera de Play
        if (!Application.isPlaying)
            return;

        if (simulation == null)
            return;

        var state = simulation.WorldState;

        if (state == null)
            return;

        int cap = state.Capacity;

        for (int i = 0; i < cap; i++)
        {
            if (!state.active[i])
                continue;

            Vector2 pos = state.positions[i];

            Gizmos.color = (state.type[i] == 1) ? Color.red : Color.white;

            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}