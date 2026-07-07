using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Services;

public sealed class LevelSignatureFactory : ILevelSignatureFactory
{
	public LevelSignature Create(SensorSnapshot snapshot)
	{
		var wifi = snapshot.WifiNetworks
			.OrderByDescending(network => network.Rssi)
			.Take(12)
			.Select(network => new WifiNetworkReading(network.Ssid.Trim(), Math.Round(network.Rssi, 2)))
			.ToArray();

		var cell = snapshot.CellTowers
			.OrderByDescending(tower => tower.Rssi)
			.Take(8)
			.Select(tower => new CellTowerReading(tower.TowerId.Trim(), Math.Round(tower.Rssi, 2)))
			.ToArray();

		var magneticField = snapshot.MagneticField;
		var canonicalPayload = JsonSerializer.Serialize(new
		{
			Wifi = wifi,
			Cell = cell,
			Pressure = Round(snapshot.Pressure),
			BtNoise = Round(snapshot.BluetoothNoiseFloor),
			MagX = Round(magneticField?.X),
			MagY = Round(magneticField?.Y),
			MagZ = Round(magneticField?.Z)
		});

		var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalPayload))).ToLowerInvariant();

		return new LevelSignature(
			hash,
			wifi,
			cell,
			Round(snapshot.Pressure),
			Round(snapshot.BluetoothNoiseFloor),
			Round(magneticField?.X),
			Round(magneticField?.Y),
			Round(magneticField?.Z),
			snapshot.CapturedAt);
	}

	private static double? Round(double? value)
	{
		return value is null ? null : Math.Round(value.Value, 3);
	}
}