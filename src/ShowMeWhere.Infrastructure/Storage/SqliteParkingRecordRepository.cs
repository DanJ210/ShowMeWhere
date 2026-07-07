using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class SqliteParkingRecordRepository : IParkingRecordRepository
{
	private readonly SqliteDatabase _database;

	public SqliteParkingRecordRepository(SqliteDatabase database)
	{
		_database = database;
	}

	public async Task<IReadOnlyList<ParkingRecord>> GetAllAsync(CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		var entities = await connection.Table<ParkingRecordEntity>().ToListAsync();
		return entities.Select(Map).ToArray();
	}

	public async Task<ParkingRecord?> GetCurrentAsync(CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		var entity = await connection.Table<ParkingRecordEntity>()
			.OrderByDescending(item => item.TimestampUnixTimeSeconds)
			.FirstOrDefaultAsync();
		return entity is null ? null : Map(entity);
	}

	public async Task SaveAsync(ParkingRecord record, CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		await connection.InsertOrReplaceAsync(new ParkingRecordEntity
		{
			Id = record.Id,
			LevelName = record.LevelName,
			SignatureId = record.SignatureId,
			TimestampUnixTimeSeconds = record.Timestamp.ToUnixTimeSeconds()
		});
	}

	private static ParkingRecord Map(ParkingRecordEntity entity)
	{
		return new ParkingRecord(
			entity.Id,
			entity.LevelName,
			entity.SignatureId,
			DateTimeOffset.FromUnixTimeSeconds(entity.TimestampUnixTimeSeconds));
	}
}