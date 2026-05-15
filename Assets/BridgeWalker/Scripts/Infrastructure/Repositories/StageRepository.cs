using System.Collections.Generic;
using BridgeWalker.Scripts.Application.DTOs;
using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using BridgeWalker.Scripts.Infrastructure.Factories;
using BridgeWalker.Scripts.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BridgeWalker.Scripts.Infrastructure.Repositories
{
    public class StageRepository : IStageRepository
    {
        private readonly JsonUtilityService _jsonUtilityService;
        private readonly BridgeFactory _bridgeFactory = new BridgeFactory();
        
        public Stage CurrentStage { get; set; }
        
        public StageRepository(JsonUtilityService jsonUtilityService)
        {
            _jsonUtilityService = jsonUtilityService;
        }

        public async UniTask LoadStage(string addressableKey)
        {
            StageData dto = await _jsonUtilityService.ConvertJsonToAnyObjectAsync<StageData>(addressableKey);
            CurrentStage = ConvertDtoToDomain(dto);
        }

        public async UniTask<GameObject> CreateStage(Transform stageParent)
        {
            if (CurrentStage is null)
            {
                Debug.LogError("[StageRepository] Stage is not loaded. Call LoadStage() before creating stage.");
                return null;
            }
            
            // Bridgeを生成
            for (int x = 0; x < CurrentStage.Width; x++)
            {
                for (int y = 0; y < CurrentStage.Height; y++)
                {
                    string cellType = CurrentStage.GetCellType(x, y);
                    if (cellType != "Bridge")
                        continue;
                    
                    Bridge bridge = CurrentStage.Bridges.Find(b => b.X == x && b.Y == y);
                    if (bridge is null)
                        continue;

                    _bridgeFactory.Create(bridge, stageParent);
                }
            }

            return stageParent.gameObject;
        } 

        private Stage ConvertDtoToDomain(StageData dto)
        {
            var bridges = new List<Bridge>();
            foreach (var cellDto in dto.cells)
            {
                bridges.Add(new Bridge(cellDto.x, cellDto.y, cellDto.cellType, cellDto.bridgeSize));
            }

            return new Stage(
                id: dto.stageId,
                width: dto.width,
                height: dto.height,
                defaultCell: "Bridge",
                cells: bridges
            );
        }
    }
}