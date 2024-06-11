using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public class DbService : IDbService
    {
        private readonly DatabaseContext _context;

        public DbService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesPersonExists(int characterId)
        {
            return await _context.Characters.AnyAsync(c => c.Id == characterId);
        }

        public async Task<IEnumerable<CharacterInfoDto>> GetPersonInfo(int characterId)
        {
            var characters = await _context.Characters
                .Where(c => c.Id == characterId)
                .Include(c => c.Backpacks)
                .ThenInclude(b => b.Item)
                .Include(c => c.CharacterTitles)
                .ThenInclude(ct => ct.Title)
                .ToListAsync();

            return characters.Select(c => new CharacterInfoDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                CurrentWeight = c.CurrentWeight,
                MaxWeight = c.MaxWeight,
                BackpackItems = c.Backpacks.Select(b => new BackpackItemDto
                {
                    ItemName = b.Item.Name,
                    ItemWeight = b.Item.Weight,
                    Amount = b.Amount
                }).ToList(),
                Titles = c.CharacterTitles.Select(t => new CharacterTitleDto
                {
                    Title = t.Title.Name,
                    AcquiredAt = t.AcquiredAt
                }).ToList()
            });
        }

        public async Task AddItemsToPerson(int characterId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                var characterBackpack = new Backpack
                {
                    CharacterId = characterId,
                    ItemId = itemId,
                    Amount = 1
                };

                await _context.Backpacks.AddAsync(characterBackpack);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanPersonCarryMore(int characterId, List<int> itemIds)
        {
            var currentWeight = await _context.Characters
                .Where(c => c.Id == characterId)
                .Select(c => c.CurrentWeight)
                .FirstOrDefaultAsync();

            var maximumWeight = await _context.Characters
                .Where(c => c.Id == characterId)
                .Select(c => c.MaxWeight)
                .FirstOrDefaultAsync();

            var totalItemsWeight = await CalculateItemsWeight(itemIds);

            return totalItemsWeight + currentWeight <= maximumWeight;
        }

        public async Task<bool> DoesItemExist(int itemId)
        {
            return await _context.Items.AnyAsync(c => c.Id == itemId);
        }

        public async Task<int> CalculateItemsWeight(List<int> itemIds)
        {
            int totalWeight = 0;

            foreach (var itemId in itemIds)
            {
                totalWeight += await _context.Items
                    .Where(i => i.Id == itemId)
                    .Select(i => i.Weight)
                    .FirstOrDefaultAsync();
            }

            return totalWeight;
        }

        public async Task UpdatePersonsWeight(int characterId, List<int> itemIds)
        {
            var totalItemsWeight = await CalculateItemsWeight(itemIds);

            var character = await _context.Characters
                .Where(c => c.Id == characterId)
                .FirstOrDefaultAsync();

            if (character != null)
            {
                character.CurrentWeight += totalItemsWeight;
                await _context.SaveChangesAsync();
            }
        }
    }
}