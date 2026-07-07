using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface IWifiScanner
{
	Task<IReadOnlyList<WifiNetworkReading>> GetNetworksAsync(CancellationToken cancellationToken);
}