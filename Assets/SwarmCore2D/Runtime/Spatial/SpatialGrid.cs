using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Spatial
{
    /// <summary>
    /// Deterministic spatial hash grid for fast neighbor lookup.
    /// </summary>
    public class SpatialGrid
    {
        float cellSize;
        int gridSize;

        GridCell[] cells;

        public SpatialGrid(float cellSize = SwarmConstants.DefaultCellSize)
        {
            this.cellSize = cellSize;

            gridSize = SwarmConstants.MaxGridCells;

            cells = new GridCell[gridSize];

            for (int i = 0; i < gridSize; i++)
                cells[i] = new GridCell();
        }

        public void Clear()
        {
            for (int i = 0; i < gridSize; i++)
                cells[i].Clear();
        }

        public void Build(SwarmState state)
        {
            Clear();

            var positions = state.positions;
            var active = state.active;

            int capacity = state.Capacity;

            for (int i = 0; i < capacity; i++)
            {
                if (!active[i])
                    continue;

                int hash = Hash(positions[i]);

                cells[hash].Add(i);
            }
        }

        public GridCell GetCell(int hash)
        {
            return cells[hash];
        }

        public int Hash(Vector2 pos)
        {
            int x = Mathf.FloorToInt(pos.x / cellSize);
            int y = Mathf.FloorToInt(pos.y / cellSize);

            int hash = x * 73856093 ^ y * 19349663;

            hash %= gridSize;

            if (hash < 0)
                hash += gridSize;

            return hash;
        }

        public float CellSize => cellSize;
    }
}