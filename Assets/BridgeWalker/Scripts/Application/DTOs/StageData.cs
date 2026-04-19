using System;
using System.Collections.Generic;

namespace BridgeWalker.Scripts.Application.DTOs
{
    /// <summary>
    /// ステージ全体のデータを保持するDTO
    /// </summary>
    [Serializable]
    public class StageData
    {
        public string stageId;
        public string stageName;
        public int width;
        public int height;
        public List<StageCellDto> cells = new List<StageCellDto>();
    }

    /// <summary>
    /// 各セルの情報を保持するDTO
    /// </summary>
    [Serializable]
    public class StageCellDto
    {
        public int x;
        public int y;
        public string cellType; // "Bridge", "Goal", "Empty" など
    }
}