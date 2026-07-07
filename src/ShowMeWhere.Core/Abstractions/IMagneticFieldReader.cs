using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface IMagneticFieldReader
{
	Task<MagneticVector?> GetMagneticFieldAsync(CancellationToken cancellationToken);
}