namespace BDAplication.Application.DTOs.Finance;

public record TypeConceptDto(
    int Id,
    string Code,
    string Description,
    DateTime CreatedDate,
    string CreatedBy);

public record CreateTypeConceptRequest(string Description);
public record UpdateTypeConceptRequest(int Id, string Description);
