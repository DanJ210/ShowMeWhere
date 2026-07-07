using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ILevelSignatureRepository
{
	Task<IReadOnlyList<LevelSignature>> GetAllAsync(CancellationToken cancellationToken);
	Task<LevelSignature?> GetByIdAsync(string id, CancellationToken cancellationToken);
	Task SaveAsync(LevelSignature signature, CancellationToken cancellationToken);
}