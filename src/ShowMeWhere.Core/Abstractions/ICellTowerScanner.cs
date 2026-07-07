using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ICellTowerScanner
{
	Task<IReadOnlyList<CellTowerReading>> GetCellTowersAsync(CancellationToken cancellationToken);
}