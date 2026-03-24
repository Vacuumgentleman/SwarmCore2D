using SwarmCore2D.Core;

namespace SwarmCore2D.Spatial
{
    /// <summary>
    /// Represents a single cell in the spatial grid.
    /// Stores entity indices located in this cell.
    /// </summary>
    public class GridCell
    {
        public int[] entities;
        public int count;

        public GridCell()
        {
            entities = new int[SwarmConstants.MaxEntities];
            count = 0;
        }

        public void Clear()
        {
            count = 0;
        }

        public void Add(int entityId)
        {
            if (count >= entities.Length)
                return;

            entities[count++] = entityId;
        }
    }
}