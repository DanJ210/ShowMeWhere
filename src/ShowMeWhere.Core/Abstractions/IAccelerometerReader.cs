using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface IAccelerometerReader
{
	Task<AccelerometerVector?> GetGravityAsync(CancellationToken cancellationToken);
}
