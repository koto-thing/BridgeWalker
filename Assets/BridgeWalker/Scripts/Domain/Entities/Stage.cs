using System.Collections.Generic;

namespace BridgeWalker.Scripts.Domain.Entities
{
    public class Stage
    {
        public string Id { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string DefaultCell { get; private set; }
        public List<Bridge> Bridges { get; private set; }

        public Stage(string id, int width, int height, string defaultCell, List<Bridge> cells)
        {
            Id = id;
            Width = width;
            Height = height;
            DefaultCell = defaultCell;
            Bridges = cells ?? new List<Bridge>();
        }

        /// <summary>
        /// 指定された座標のセルの種類を取得する
        /// </summary>
        /// <param name="x">水平方向のインデックス</param>
        /// <param name="y">垂直方向의インデックス</param>
        /// <returns>セルの種類</returns>
        public string GetCellType(int x, int y)
        {
            var cell = Bridges.Find(c => c.X == x && c.Y == y);
            return cell?.CellType ?? DefaultCell;
        }
    }
}