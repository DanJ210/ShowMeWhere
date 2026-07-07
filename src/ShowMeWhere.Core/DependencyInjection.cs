using Microsoft.Extensions.DependencyInjection;
using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Services;

namespace ShowMeWhere.Core;

public static class DependencyInjection
{
	public static IServiceCollection AddShowMeWhereCore(this IServiceCollection services)
	{
		services.AddSingleton<ILevelSignatureFactory, LevelSignatureFactory>();
		services.AddSingleton<ILevelSimilarityService, LevelSimilarityService>();
		services.AddSingleton<ILevelDetectionService, LevelDetectionService>();
		services.AddSingleton<IParkingWorkflowService, ParkingWorkflowService>();
		return services;
	}
}