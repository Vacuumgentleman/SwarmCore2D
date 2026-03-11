using System.Collections.Generic;
using UnityEngine;

namespace SwarmCore2D.Core
{
    public class SpatialHashGrid
    {
        float cellSize;

        Dictionary<int, List<int>> grid =
            new Dictionary<int, List<int>>();

        public SpatialHashGrid(float cellSize)
        {
            this.cellSize = cellSize;
        }

        int Hash(int x, int y)
        {
            unchecked
            {
                return x * 73856093 ^ y * 19349663;
            }
        }

        Vector2Int Cell(Vector2 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / cellSize),
                Mathf.FloorToInt(pos.y / cellSize)
            );
        }

        public void Clear()
        {
            grid.Clear();
        }

        public void Add(int id, Vector2 pos)
        {
            Vector2Int c = Cell(pos);

            int hash = Hash(c.x, c.y);

            if (!grid.TryGetValue(hash, out var list))
            {
                list = new List<int>(8);
                grid.Add(hash, list);
            }

            list.Add(id);
        }

        public List<int> Query(Vector2 pos)
        {
            Vector2Int c = Cell(pos);

            List<int> result = new List<int>(16);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int hash = Hash(c.x + x, c.y + y);

                    if (grid.TryGetValue(hash, out var list))
                    {
                        result.AddRange(list);
                    }
                }
            }

            return result;
        }
    }
}