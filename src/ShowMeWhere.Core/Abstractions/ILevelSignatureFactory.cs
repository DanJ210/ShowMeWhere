using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ILevelSignatureFactory
{
	LevelSignature Create(SensorSnapshot snapshot);
}