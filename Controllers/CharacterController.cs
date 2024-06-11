using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class CharacterManagementController : ControllerBase
    {
        private readonly IDbService _databaseService;

        public CharacterManagementController(IDbService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("{characterId}")]
        public async Task<IActionResult> GetCharacterInfo(int characterId)
        {
            if (!await _databaseService.DoesPersonExists(characterId))
                return NotFound($"Character with ID = {characterId} does not exist!");

            var characterInfo = await _databaseService.GetPersonInfo(characterId);
            return Ok(characterInfo);
        }

        [HttpPost("{characterId}/backpacks")]
        public async Task<IActionResult> AddItemToCharacter(int characterId, List<int> itemIds)
        {
            if (!await _databaseService.DoesPersonExists(characterId))
                return NotFound($"Character with given ID = {characterId} does not exist!");

            foreach (var itemId in itemIds)
            {
                if (!await _databaseService.DoesItemExist(itemId))
                    return NotFound($"Item with given ID = {itemId} does not exist!");
            }

            if (!await _databaseService.CanPersonCarryMore(characterId, itemIds))
                return BadRequest("Character wont be able to carry these items!");

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _databaseService.UpdatePersonsWeight(characterId, itemIds);
                await _databaseService.AddItemsToPerson(characterId, itemIds);

                transactionScope.Complete();
            }

            return Ok(itemIds);
        }
    }
}