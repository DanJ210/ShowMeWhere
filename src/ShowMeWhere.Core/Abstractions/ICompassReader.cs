namespace ShowMeWhere.Core.Abstractions;

public interface ICompassReader
{
	Task<double?> GetHeadingAsync(CancellationToken cancellationToken);
}