using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class SqliteLastDetectionRepository : ILastDetectionRepository
{
	private readonly SqliteDatabase _database;

	public SqliteLastDetectionRepository(SqliteDatabase database)
	{
		_database = database;
	}

	public async Task<LastDetection?> GetLatestAsync(CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		var entity = await connection.Table<LastDetectionEntity>()
			.OrderByDescending(item => item.TimestampUnixTimeSeconds)
			.FirstOrDefaultAsync();
		return entity is null ? null : Map(entity);
	}

	public async Task SaveAsync(LastDetection detection, CancellationToken cancellationToken)
	{
		var connection = await _database.GetConnectionAsync();
		await connection.InsertOrReplaceAsync(new LastDetectionEntity
		{
			LevelName = detection.LevelName,
			Confidence = detection.Confidence,
			TimestampUnixTimeSeconds = detection.Timestamp.ToUnixTimeSeconds()
		});
	}

	private static LastDetection Map(LastDetectionEntity entity)
	{
		return new LastDetection(
			entity.LevelName,
			entity.Confidence,
			DateTimeOffset.FromUnixTimeSeconds(entity.TimestampUnixTimeSeconds));
	}
}
