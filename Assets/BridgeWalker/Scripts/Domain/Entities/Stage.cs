using System.Collections.Generic;
using BridgeWalker.Scripts.Domain.ValueObjects;

namespace BridgeWalker.Scripts.Domain.Entities
{
    public class Stage
    {
        public string Id { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public string DefaultCell { get; private set; }
        public List<StageCell> Cells { get; private set; }

        public Stage(string id, int width, int height, string defaultCell, List<StageCell> cells)
        {
            Id = id;
            Width = width;
            Height = height;
            DefaultCell = defaultCell;
            Cells = cells ?? new List<StageCell>();
        }

        /// <summary>
        /// 指定された座標のセルの種類を取得する
        /// </summary>
        /// <param name="x">水平方向のインデックス</param>
        /// <param name="y">垂直方向のインデックス</param>
        /// <returns>セルの種類</returns>
        public string GetCellType(int x, int y)
        {
            var cell = Cells.Find(c => c.X == x && c.Y == y);
            return cell?.CellType ?? DefaultCell;
        }
    }
}