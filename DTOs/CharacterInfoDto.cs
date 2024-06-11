namespace WebApplication1.DTOs;

public class CharacterInfoDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CurrentWeight { get; set; }
    public int MaxWeight { get; set; }
    public IEnumerable<BackpackItemDto> BackpackItems { get; set; }
    public List<CharacterTitleDto> Titles { get; set; }
}