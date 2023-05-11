using ConfigurationSubstitution;
using Cronos;
using GeradorDeDados.Authentication;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models.Settings;
using GeradorDeDados.Services;
using GeradorDeDados.Services.Mocks;
using GeradorDeDados.Works;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RestEase.HttpClientFactory;
using StringPlaceholder.FluentPattern;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeradorDeDados
{
    public static class DependencyInjection
    {
        public static void CriarInjecao(this IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                 .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                 .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .EnableSubstitutions("%", "%")
                 .Build();

            services.InjetarConfiguracoes(config);
            services.InjetarAutenticacoes();
            services.InjetarServicosDeArmazenamento(config);
            services.InjetarServicos();
            services.InjetarStringPlaceHolderService();
            services.InjetarSwagger();
            services.InjetarPoliticaCors();
        }
        private static void InjetarServicos(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddSingleton<IRedisService, RedisService>();
            services.AddSingleton<IDadosReceitaWS, DadosReceitaWS>();
            services.AddSingleton<IGeradorDocumentoMock, GeradorDocumentoMock>();
            services.AddHostedService<ReceitaWSWorker>();
            services.AddHostedService<ReceitaWSAutoFillWorker>();
            services.AddRestEaseClient<IReceitaWS>("https://receitaws.com.br");
            services.AddSingleton<ApiCicloDeVida>();
            services.AddSingleton<ReceitaWSConfig>();

        }

        private static void InjetarStringPlaceHolderService(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dadosReceitaWS = serviceProvider.GetRequiredService<IDadosReceitaWS>();

            var stringPlaceHolder = new ExecutorCreator().Init()
                .AddRange(Placeholder.ObterExecutores(dadosReceitaWS))
                .BuildDescription();

            services.AddSingleton<Placeholder>();
            services.AddSingleton(stringPlaceHolder);
        }
        private static void InjetarAutenticacoes(this IServiceCollection services)
        {
            services.AddAuthentication("ApiKey")
                 .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
                 ("ApiKey", null);

        }
        private static void InjetarServicosDeArmazenamento(this IServiceCollection services, IConfigurationRoot config)
        {
            var serviceProvider = services.BuildServiceProvider();
            var apiConfig = serviceProvider.GetRequiredService<IOptions<ApiConfig>>().Value;
            var cronParsed = CronExpression.Parse(apiConfig.CacheConfig.ExpireEvery);
            var distributedCacheEntry = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = cronParsed.GetNextOccurrence(DateTime.UtcNow),
            };
            services.AddSingleton(distributedCacheEntry);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = ObterRedisContext();
            });

            string ObterRedisContext()
            {
                var redisContextUrl = config.GetConnectionString("Redis");
                Uri redisUrl;
                bool isRedisUrl = Uri.TryCreate(redisContextUrl, UriKind.Absolute, out redisUrl);
                if (isRedisUrl)
                {
                    redisContextUrl = string.Format("{0}:{1},password={2}", redisUrl.Host, redisUrl.Port, redisUrl.UserInfo.Split(':')[1]);
                }
                return redisContextUrl;
            }
        }
        private static void InjetarConfiguracoes(this IServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
        }

        private static void InjetarPoliticaCors(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var apiConfig = serviceProvider.GetRequiredService<IOptions<ApiConfig>>().Value;
            services.AddCors(p => p.AddPolicy("corsapp",
            builder =>
            {
                builder
                                .WithOrigins(apiConfig.CorsOrigin)

                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
        }

        private static void InjetarSwagger(this IServiceCollection services)
        {
            var contact = new OpenApiContact()
            {
                Name = "Roberto Paes",
                Email = "contato@robertinho.net",
                Url = new Uri("https://robertocpaes.dev")
            };
            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "Gerador de documentos (CNPJ)",
                Description = "Minimal API para gerar lista de cnpj validados pela receitaWS",
                Contact = contact
            };
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EnumSchemaFilter>();
                c.SwaggerDoc("v1", info);
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "Insira a ApiKey abaixo.",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "ApiKey",
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
                c.AddSecurityRequirement(requirement);
            });
        }
        internal sealed class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema model, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    model.Enum.Clear();
                    Enum
                       .GetNames(context.Type)
                       .ToList()
                       .ForEach(name => model.Enum.Add(new OpenApiString($"{name}")));
                    model.Type = "string";
                    model.Format = string.Empty;
                }

            }
        }
    }
}
