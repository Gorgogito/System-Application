namespace BDAplication.Domain.Entities.Finance;

public class Account
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;       // AC00000001
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
