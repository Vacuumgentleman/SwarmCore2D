using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Spatial
{
    /// <summary>
    /// Performs neighbor queries using the spatial grid.
    /// </summary>
    public class SpatialQuery
    {
        SpatialGrid grid;

        public SpatialQuery(SpatialGrid grid)
        {
            this.grid = grid;
        }

        public int QueryNeighbors(
            SwarmState state,
            Vector2 position,
            float radius,
            int[] results)
        {
            float cellSize = grid.CellSize;

            int minX = Mathf.FloorToInt((position.x - radius) / cellSize);
            int maxX = Mathf.FloorToInt((position.x + radius) / cellSize);

            int minY = Mathf.FloorToInt((position.y - radius) / cellSize);
            int maxY = Mathf.FloorToInt((position.y + radius) / cellSize);

            int count = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    int hash = Hash(x, y);

                    var cell = grid.GetCell(hash);

                    for (int i = 0; i < cell.count; i++)
                    {
                        if (count >= results.Length)
                            return count;

                        results[count++] = cell.entities[i];
                    }
                }
            }

            return count;
        }

        int Hash(int x, int y)
        {
            int hash = x * 73856093 ^ y * 19349663;

            hash %= SwarmConstants.MaxGridCells;

            if (hash < 0)
                hash += SwarmConstants.MaxGridCells;

            return hash;
        }
    }
}