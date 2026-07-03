namespace BDAplication.Application.DTOs.Finance;

public record AccountDto(
    int Id,
    string Code,
    string Name,
    string Description,
    decimal Balance,
    DateTime CreatedDate,
    string CreatedBy);

public record CreateAccountRequest(string Name, string Description);
public record UpdateAccountRequest(int Id, string Name, string Description);
