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
            //signalR 서비스 추가
            services.AddSignalR();
            //dbContext 서비스 추가
            services.AddDbContext<GardenUserContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("GardenUserContext")));

            //interface 추가
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<HashService, HashService>();
            services.AddScoped<IUserService, UserService>();

            //email Setting
            services.Configure<MailSetting>(Configuration.GetSection("MailSettings"));
    
            //cors 추가
            services.AddCors();
            //JWT 추가
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
                    //Issuer의 유효성 여부
                    ValidateIssuer = true,
                    //Audience의 유효성 여부
                    ValidateAudience = true,
                    //Token의 생명주기
                    ValidateLifetime = true,
                    //Token의 유효성을 검증
                    ValidateIssuerSigningKey = true,
                    //Token의 발행자
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    //청중을 지정한다 특별한 경우가 아니면 JWT인증을 수행하는 도메인 지정
                    ValidAudience = Configuration["Jwt:Audience"],
                    //Token을 발행할 암호화키 지정
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    //시간을 확인할 대 적용할 클럭 스큐 단위 시간 설정
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

            //인증 서비스 제공
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
