using BDAplication.Domain.Entities.Finance;

namespace BDAplication.Domain.Interfaces.Finance;

public interface IAccountStatementRepository
{
    Task<Account?> GetAccountAsync(int accountId);

    /// <summary>
    /// Saldo acumulado de la cuenta considerando todos los movimientos anteriores a startDate.
    /// Retorna el Balance del último movimiento antes de startDate, o 0 si no existe ninguno.
    /// </summary>
    Task<decimal> GetPreviousBalanceAsync(int accountId, DateTime startDate);

    /// <summary>
    /// Movimientos en el rango [startDate, endDate] ordenados por Fecha, Id.
    /// Incluye la información de TypeConcept.
    /// </summary>
    Task<IEnumerable<Movement>> GetMovementsInRangeAsync(int accountId, DateTime startDate, DateTime endDate);
}
