using System.Collections.Generic;
using BridgeWalker.Scripts.Application.DTOs;
using BridgeWalker.Scripts.Application.Interfaces;
using BridgeWalker.Scripts.Domain.Entities;
using BridgeWalker.Scripts.Domain.ValueObjects;
using Cysharp.Threading.Tasks;

namespace BridgeWalker.Scripts.Infrastructure.Services
{
    public class StageCreationService : IStageCreationService
    {
        private readonly JsonUtilityService _jsonLoader;
        
        public StageCreationService(JsonUtilityService jsonLoader)
        {
            _jsonLoader = jsonLoader;
        }

        public async UniTask<Stage> LoadStage(string addressableKey)
        {
            StageData dto = await _jsonLoader.ConvertJsonToAnyObjectAsync<StageData>(addressableKey);
            return ConvertDtoToDomain(dto);
        }

        private Stage ConvertDtoToDomain(StageData dto)
        {
            var cells = new List<StageCell>();
            foreach (var cellDto in dto.cells)
            {
                cells.Add(new StageCell(cellDto.x, cellDto.y, cellDto.cellType));
            }

            return new Stage(
                id: dto.stageId,
                width: dto.width,
                height: dto.height,
                defaultCell: "Bridge",
                cells: cells
            );
        }
    }
}