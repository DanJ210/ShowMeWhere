namespace ShowMeWhere.Core.Abstractions;

public interface IBarometerReader
{
	Task<double?> GetPressureAsync(CancellationToken cancellationToken);
}