using Microsoft.Extensions.DependencyInjection;
using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Infrastructure.Cloud;
using ShowMeWhere.Infrastructure.Storage;

namespace ShowMeWhere.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddShowMeWhereInfrastructure(this IServiceCollection services, string databasePath)
	{
		services.AddSingleton(new SqliteDatabase(databasePath));
		services.AddSingleton<ILevelSignatureRepository, SqliteLevelSignatureRepository>();
		services.AddSingleton<IParkingRecordRepository, SqliteParkingRecordRepository>();
		services.AddSingleton<ICloudSyncService, NoOpCloudSyncService>();
		return services;
	}
}