
using BlueNest.API.Extentions;
using BlueNest.Core.Contracts;
using BlueNest.Core.Entities.SecurityModule;
using BlueNest.Infrastructure.Data.Contexts;
using BlueNest.Infrastructure.Data.DataSeed;
using BlueNest.Infrastructure.ExternalServices;
using BlueNest.Infrastructure.Repository;
using BlueNest.Services.Abstraction;
using BlueNest.Services.Helpers;
using BlueNest.Services.MappingProfiles;
using BlueNest.Services.Services;
using BlueNest.Shared.Message;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(A =>
            {
                A.AllowNullCollections = true;
            }, typeof(ServiceAssemblyReference).Assembly);

            //builder.Services.AddAutoMapper(X => X.AddProfile<RoomProfile>());
            //builder.Services.AddTransient<RoomImageValueResolver>();

            //builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);




            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();

            builder.Services.AddScoped<IDataIntializer, IdentityDataIntializer>();

            // Identity configuration To Inject In RunTime Usermanager,RoleManager
            builder.Services.AddIdentityCore<HotelUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HotelDbContext>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                        ValidAudience = builder.Configuration["JwtOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]!))
                    };
                });


            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings")
               );

            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddScoped<IBookingService, BookingService>();

            builder.Services.AddHttpClient<IPaymentService, PaymentService>();
            var app = builder.Build();

            await app.MigrateDatabaseAsync();

            await app.SeedingidentityDataAsync();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
