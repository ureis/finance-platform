using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.UseCases.BuyAsset;
using Wallet.Application.UseCases.GetAssets;

namespace Wallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<GetAssetsQuery>();
        services.AddScoped<BuyAssetUseCase>();

        return services;
    }
}