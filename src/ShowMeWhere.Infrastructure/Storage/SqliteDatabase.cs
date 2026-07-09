using SQLite;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class SqliteDatabase
{
	private readonly Lazy<Task<SQLiteAsyncConnection>> _connectionFactory;

	public SqliteDatabase(string databasePath)
	{
		DatabasePath = databasePath;
		_connectionFactory = new Lazy<Task<SQLiteAsyncConnection>>(InitializeAsync);
	}

	public string DatabasePath { get; }

	public Task<SQLiteAsyncConnection> GetConnectionAsync()
	{
		return _connectionFactory.Value;
	}

	private async Task<SQLiteAsyncConnection> InitializeAsync()
	{
		var directory = Path.GetDirectoryName(DatabasePath);
		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}

		var connection = new SQLiteAsyncConnection(
			DatabasePath,
			SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

		await connection.MigrateAsync<LevelSignatureEntity>();
		await connection.MigrateAsync<ParkingRecordEntity>();
		return connection;
	}
}