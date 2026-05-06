
using BlueNest.API.Extentions;
using BlueNest.Core.Contracts;
using BlueNest.Infrastructure.Data.Contexts;
using BlueNest.Infrastructure.Repository;
using BlueNest.Services.Abstraction;
using BlueNest.Services.Helpers;
using BlueNest.Services.MappingProfiles;
using BlueNest.Services.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlueNest.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<HotelDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

            builder.Services.AddAutoMapper(A =>
            {
                A.AllowNullCollections = true;
            }, typeof(ServiceAssemblyReference).Assembly);

            //builder.Services.AddAutoMapper(X => X.AddProfile<RoomProfile>());
            //builder.Services.AddTransient<RoomImageValueResolver>();

            //builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);




            builder.Services.AddScoped<IRoomService,RoomService>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();


            var app = builder.Build();

            await app.MigrateDatabaseAsync();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
