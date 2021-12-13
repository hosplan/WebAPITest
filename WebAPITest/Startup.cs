using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Hubs;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            //signalR ���� �߰�
            services.AddSignalR();
            //dbContext ���� �߰�
            services.AddDbContext<GardenUserContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("GardenUserContext")));

            //interface �߰�
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<HashService, HashService>();
            services.AddScoped<IUserService, UserService>();

            //email Setting
            services.Configure<MailSetting>(Configuration.GetSection("MailSettings"));
    
            //cors �߰�
            services.AddCors();
            //JWT �߰�
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Token = context.Request.Headers["Authorization"];
                        }
                        else if (context.Request.Query.ContainsKey("token"))
                        {
                            context.Token = context.Request.Query["token"];
                        }

                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //Issuer�� ��ȿ�� ����
                    ValidateIssuer = true,
                    //Audience�� ��ȿ�� ����
                    ValidateAudience = true,
                    //Token�� �����ֱ�
                    ValidateLifetime = true,
                    //Token�� ��ȿ���� ����
                    ValidateIssuerSigningKey = true,
                    //Token�� ������
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    //û���� �����Ѵ� Ư���� ��찡 �ƴϸ� JWT������ �����ϴ� ������ ����
                    ValidAudience = Configuration["Jwt:Audience"],
                    //Token�� ������ ��ȣȭŰ ����
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    //�ð��� Ȯ���� �� ������ Ŭ�� ��ť ���� �ð� ����
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleGrade", policy =>
                    policy.Requirements.Add(new AuthService(3)));
            });
            services.AddScoped<IAuthorizationHandler, AuthServiceHandler>();
            services.AddDbContext<GardenUserContext>(options =>
            options.UseSqlServer("Server=192.168.0.6,1433;Database=GardenUserSDB;User Id=SA;Password=emth022944w!;"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPITest", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPITest v1"));
            }


            app.UseCors(builder =>
                builder.WithOrigins("https://localhost:49154")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());
            //app.UseCors(builder =>
            //    builder.WithOrigins("https://localhost:44380").AllowAnyHeader().AllowCredentials());
            app.UseRouting();

            //���� ���� ����
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RealTimeCheckHub>("/realtimeCheckHub");
                endpoints.MapControllers();
            });
        }
    }
}
