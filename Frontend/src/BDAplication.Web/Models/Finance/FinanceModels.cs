namespace BDAplication.Web.Models.Finance;

public class AccountModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class TypeConceptModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class MovementModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Concept { get; set; } = string.Empty;
    public int? TypeConceptId { get; set; }
    public string TypeConceptCode { get; set; } = string.Empty;
    public string TypeConceptDescription { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = "I";
    public string TypeLabel { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsTransfer { get; set; }
    public string TransferSourceDestiny { get; set; } = "X";
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

// Requests
public class CreateAccountRequest { public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; }
public class UpdateAccountRequest { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; }

public class CreateTypeConceptRequest { public string Description { get; set; } = string.Empty; }
public class UpdateTypeConceptRequest { public int Id { get; set; } public string Description { get; set; } = string.Empty; }

public class CreateMovementRequest
{
    public int AccountId { get; set; }
    public string Concept { get; set; } = string.Empty;
    public int? TypeConceptId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public string Type { get; set; } = "I";
}

public class UpdateMovementRequest
{
    public int Id { get; set; }
    public string Concept { get; set; } = string.Empty;
    public int? TypeConceptId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
}

public class CreateTransferRequest
{
    public int SourceAccountId { get; set; }
    public int DestinyAccountId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public string Concept { get; set; } = "Transferencia";
}
