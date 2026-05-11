using BlueNest.Core.Contracts;
using BlueNest.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlueNest.API.Extentions
{
    public static class WebApplicationRegister
    {
     
        public static async Task<WebApplication> MigrateDatabaseAsync( this WebApplication webApplication)
        {
            await using var scope = webApplication.Services.CreateAsyncScope();

            var hotelDbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

            var PendingMigrations = await hotelDbContext.Database.GetPendingMigrationsAsync();

            if (PendingMigrations.Any())
               await hotelDbContext.Database.MigrateAsync();

            return webApplication;
        }

        public static async Task<WebApplication> SeedingidentityDataAsync(this WebApplication  webApplication)
        {
            await using var scope = webApplication.Services.CreateAsyncScope();
            var DataIntializer = scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            await DataIntializer.InitializeAdminAndRoleAsync();

            return webApplication;
        }

    }
}
