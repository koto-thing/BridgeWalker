namespace BridgeWalker.Scripts.Domain.Entities
{
    public class Bridge
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public string CellType { get; private set; }
        public float BridgeSize { get; private set; }

        public Bridge(int x, int y, string cellType, float bridgeSize)
        {
            X = x;
            Y = y;
            CellType = cellType;
            BridgeSize = bridgeSize > 0 ? bridgeSize : 1.0f;
        }
    }
}