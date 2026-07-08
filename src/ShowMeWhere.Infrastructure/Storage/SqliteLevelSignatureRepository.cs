using System.Text.Json;
using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class SqliteLevelSignatureRepository : ILevelSignatureRepository
{
	private readonly SqliteDatabase _database;

	public SqliteLevelSignatureRepository(SqliteDatabase database)
	{
		_database = database;
	}

	public async Task<IReadOnlyList<LevelSignature>> GetAllAsync(CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		var entities = await connection.Table<LevelSignatureEntity>().ToListAsync();
		return entities.Select(Map).ToArray();
	}

	public async Task<LevelSignature?> GetByIdAsync(string id, CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		var entity = await connection.Table<LevelSignatureEntity>().Where(item => item.Id == id).FirstOrDefaultAsync();
		return entity is null ? null : Map(entity);
	}

	public async Task SaveAsync(LevelSignature signature, CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		await connection.InsertOrReplaceAsync(new LevelSignatureEntity
		{
			Id = signature.Id,
			WifiJson = JsonSerializer.Serialize(signature.Wifi),
			CellJson = JsonSerializer.Serialize(signature.Cell),
			Pressure = signature.Pressure,
			BtNoise = signature.BtNoise,
			MagX = signature.MagX,
			MagY = signature.MagY,
			MagZ = signature.MagZ,				Compass = signature.Compass,
				AccX = signature.AccX,
				AccY = signature.AccY,
				AccZ = signature.AccZ,			CreatedAtUnixTimeSeconds = signature.CreatedAt.ToUnixTimeSeconds()
		});
	}

	private static LevelSignature Map(LevelSignatureEntity entity)
	{
		return new LevelSignature(
			entity.Id,
			JsonSerializer.Deserialize<WifiNetworkReading[]>(entity.WifiJson) ?? Array.Empty<WifiNetworkReading>(),
			JsonSerializer.Deserialize<CellTowerReading[]>(entity.CellJson) ?? Array.Empty<CellTowerReading>(),
			entity.Pressure,
			entity.BtNoise,
			entity.MagX,
			entity.MagY,
			entity.MagZ,
			entity.Compass,
			entity.AccX,
			entity.AccY,
			entity.AccZ,
			DateTimeOffset.FromUnixTimeSeconds(entity.CreatedAtUnixTimeSeconds));
	}
}