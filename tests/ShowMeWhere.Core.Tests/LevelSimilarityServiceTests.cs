using ShowMeWhere.Core.Models;
using ShowMeWhere.Core.Services;

namespace ShowMeWhere.Core.Tests;

public sealed class LevelSimilarityServiceTests
{
	private readonly LevelSimilarityService _service = new();

	[Fact]
	public void Compare_ReturnsHighScore_ForNearlyIdenticalSignatures()
	{
		var left = CreateSignature(
			"a",
			[new WifiNetworkReading("Garage-1", -52), new WifiNetworkReading("Garage-2", -61)],
			[new CellTowerReading("tower-10", -88)],
			1009.2,
			-72,
			13,
			-7,
			41);
		var right = CreateSignature(
			"b",
			[new WifiNetworkReading("Garage-1", -53), new WifiNetworkReading("Garage-2", -60)],
			[new CellTowerReading("tower-10", -87)],
			1009.1,
			-71,
			12.5,
			-7.1,
			41.2);

		var score = _service.Compare(left, right);

		Assert.InRange(score, 0.97, 1.0);
	}

	[Fact]
	public void Compare_ReturnsLowerScore_ForDistinctSignatures()
	{
		var left = CreateSignature(
			"a",
			[new WifiNetworkReading("Garage-A", -49)],
			[new CellTowerReading("tower-1", -70)],
			1008.6,
			-67,
			5,
			3,
			51);
		var right = CreateSignature(
			"b",
			[new WifiNetworkReading("Garage-Z", -91)],
			[new CellTowerReading("tower-88", -96)],
			987.1,
			-95,
			-47,
			22,
			-13);

		var score = _service.Compare(left, right);

		Assert.InRange(score, 0.0, 0.6);
	}

	[Fact]
	public void Compare_ReturnsHighScore_WhenAccelerometerAndCompassMatch()
	{
		var left = CreateSignature(
			"a",
			[],
			[],
			1009.2,
			-72,
			13,
			-7,
			41,
			compass: 182.4,
			accX: 0.03,
			accY: -0.01,
			accZ: 0.99);
		var right = CreateSignature(
			"b",
			[],
			[],
			1009.2,
			-72,
			13,
			-7,
			41,
			compass: 182.6,
			accX: 0.03,
			accY: -0.01,
			accZ: 0.99);

		var score = _service.Compare(left, right);

		Assert.InRange(score, 0.99, 1.0);
	}

	[Fact]
	public void Compare_ReturnsLowerScore_WhenCompassDiffers()
	{
		var left = CreateSignature(
			"a",
			[],
			[],
			1009.2,
			-72,
			13,
			-7,
			41,
			compass: 0.0,
			accX: 0.0,
			accY: 0.0,
			accZ: 1.0);
		var right = CreateSignature(
			"b",
			[],
			[],
			1009.2,
			-72,
			13,
			-7,
			41,
			compass: 180.0,
			accX: 0.0,
			accY: 0.0,
			accZ: 1.0);

		var score = _service.Compare(left, right);

		Assert.InRange(score, 0.0, 0.99);
	}

	private static LevelSignature CreateSignature(
		string id,
		IReadOnlyList<WifiNetworkReading> wifi,
		IReadOnlyList<CellTowerReading> cell,
		double pressure,
		double btNoise,
		double magX,
		double magY,
		double magZ,
		double? compass = null,
		double? accX = null,
		double? accY = null,
		double? accZ = null)
	{
		return new LevelSignature(
			id,
			wifi,
			cell,
			pressure,
			btNoise,
			magX,
			magY,
			magZ,
			compass,
			accX,
			accY,
			accZ,
			DateTimeOffset.UtcNow);
	}
}