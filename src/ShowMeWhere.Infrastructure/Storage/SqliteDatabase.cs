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

		// CreateTableAsync with CreateFlags.None will create table if it doesn't exist,
		// and safely ignore the request if it does (allowing schema evolution via migrations)
		await connection.CreateTableAsync<LevelSignatureEntity>(CreateFlags.None);
		await connection.CreateTableAsync<ParkingRecordEntity>(CreateFlags.None);
		await connection.CreateTableAsync<LastDetectionEntity>(CreateFlags.None);
		return connection;
	}
}