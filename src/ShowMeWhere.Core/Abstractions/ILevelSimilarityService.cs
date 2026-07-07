using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ILevelSimilarityService
{
	double Compare(LevelSignature left, LevelSignature right);
}