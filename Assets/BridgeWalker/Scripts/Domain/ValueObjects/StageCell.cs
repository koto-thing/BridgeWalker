namespace BridgeWalker.Scripts.Domain.ValueObjects
{
    public class StageCell
    {
        public int X { get; }
        public int Y { get; }
        public string CellType { get; }

        public StageCell(int x, int y, string cellType)
        {
            X = x;
            Y = y;
            CellType = cellType;
        }
    }
}