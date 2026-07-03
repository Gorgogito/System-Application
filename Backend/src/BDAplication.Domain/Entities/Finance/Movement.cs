namespace BDAplication.Domain.Entities.Finance;

public class Movement
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;       // MV00000001
    public int AccountId { get; set; }
    public string Concept { get; set; } = string.Empty;
    public int? TypeConceptId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = "I";               // I=Ingreso  O=Salida
    public decimal Balance { get; set; }                   // saldo resultante
    public bool IsTransfer { get; set; }
    public string TransferSourceDestiny { get; set; } = "X"; // O=Origen D=Destino X=No aplica
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public Account Account { get; set; } = default!;
    public TypeConcept? TypeConcept { get; set; }
}
