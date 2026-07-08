using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Services;

public sealed class LevelSimilarityService : ILevelSimilarityService
{
	public double Compare(LevelSignature left, LevelSignature right)
	{
		var vectorA = new List<double>();
		var vectorB = new List<double>();

		AppendNetworkFeatures(
			left.Wifi,
			right.Wifi,
			entry => entry.Ssid,
			entry => NormalizeRssi(entry.Rssi),
			vectorA,
			vectorB);

		AppendNetworkFeatures(
			left.Cell,
			right.Cell,
			entry => entry.TowerId,
			entry => NormalizeRssi(entry.Rssi),
			vectorA,
			vectorB);

		AppendScalar(vectorA, vectorB, NormalizePressure(left.Pressure), NormalizePressure(right.Pressure));
		AppendScalar(vectorA, vectorB, NormalizeNoise(left.BtNoise), NormalizeNoise(right.BtNoise));
		AppendScalar(vectorA, vectorB, NormalizeMagnetic(left.MagX), NormalizeMagnetic(right.MagX));
		AppendScalar(vectorA, vectorB, NormalizeMagnetic(left.MagY), NormalizeMagnetic(right.MagY));
		AppendScalar(vectorA, vectorB, NormalizeMagnetic(left.MagZ), NormalizeMagnetic(right.MagZ));
		AppendScalar(vectorA, vectorB, CompassSin(left.Compass), CompassSin(right.Compass));
		AppendScalar(vectorA, vectorB, CompassCos(left.Compass), CompassCos(right.Compass));
		AppendScalar(vectorA, vectorB, NormalizeAcceleration(left.AccX), NormalizeAcceleration(right.AccX));
		AppendScalar(vectorA, vectorB, NormalizeAcceleration(left.AccY), NormalizeAcceleration(right.AccY));
		AppendScalar(vectorA, vectorB, NormalizeAcceleration(left.AccZ), NormalizeAcceleration(right.AccZ));

		return CosineSimilarity(vectorA, vectorB);
	}

	private static void AppendNetworkFeatures<T>(
		IReadOnlyList<T> left,
		IReadOnlyList<T> right,
		Func<T, string> keySelector,
		Func<T, double> valueSelector,
		ICollection<double> vectorA,
		ICollection<double> vectorB)
	{
		var leftMap = left
			.GroupBy(keySelector, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => valueSelector(group.First()), StringComparer.OrdinalIgnoreCase);
		var rightMap = right
			.GroupBy(keySelector, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => valueSelector(group.First()), StringComparer.OrdinalIgnoreCase);
		var keys = leftMap.Keys.Union(rightMap.Keys, StringComparer.OrdinalIgnoreCase).OrderBy(key => key, StringComparer.OrdinalIgnoreCase);

		foreach (var key in keys)
		{
			vectorA.Add(leftMap.TryGetValue(key, out var leftValue) ? leftValue : 0d);
			vectorB.Add(rightMap.TryGetValue(key, out var rightValue) ? rightValue : 0d);
		}
	}

	private static void AppendScalar(ICollection<double> vectorA, ICollection<double> vectorB, double left, double right)
	{
		vectorA.Add(left);
		vectorB.Add(right);
	}

	private static double CosineSimilarity(IReadOnlyList<double> left, IReadOnlyList<double> right)
	{
		double dotProduct = 0d;
		double leftMagnitude = 0d;
		double rightMagnitude = 0d;

		for (var index = 0; index < left.Count; index++)
		{
			dotProduct += left[index] * right[index];
			leftMagnitude += left[index] * left[index];
			rightMagnitude += right[index] * right[index];
		}

		if (leftMagnitude == 0d && rightMagnitude == 0d)
		{
			return 1d;
		}

		if (leftMagnitude == 0d || rightMagnitude == 0d)
		{
			return 0d;
		}

		return dotProduct / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
	}

	private static double NormalizeMagnetic(double? value)
	{
		return value is null ? 0d : Math.Clamp(value.Value / 100d, -1d, 1d);
	}

	private static double CompassSin(double? degrees)
	{
		return degrees is null ? 0d : Math.Sin(degrees.Value * Math.PI / 180d);
	}

	private static double CompassCos(double? degrees)
	{
		return degrees is null ? 0d : Math.Cos(degrees.Value * Math.PI / 180d);
	}

	private static double NormalizeAcceleration(double? value)
	{
		return value is null ? 0d : Math.Clamp(value.Value, -1d, 1d);
	}

	private static double NormalizeNoise(double? value)
	{
		return value is null ? 0d : Math.Clamp((value.Value + 100d) / 100d, 0d, 1d);
	}

	private static double NormalizePressure(double? value)
	{
		return value is null ? 0d : Math.Clamp((value.Value - 850d) / 250d, 0d, 1d);
	}

	private static double NormalizeRssi(double value)
	{
		return Math.Clamp((value + 100d) / 100d, 0d, 1d);
	}
}