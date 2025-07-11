using HelsiListOfTasks.Infrastructure.Mongo;
using MongoDB.Driver;

namespace HelsiListOfTasks.WebApi.Extensions;

public static class MongoDbServiceExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config,
        bool isDevelopment)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            string? connectionString;

            if (isDevelopment)
            {
                connectionString = config.GetValue<string>("MongoDbSettings:ConnectionString");
            }
            else
            {
                connectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString")
                                   ?? throw new InvalidOperationException(
                                       "MongoDB connection string not set in environment variables");
            }

            return new MongoClient(connectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            string? databaseName;

            if (isDevelopment)
            {
                databaseName = config.GetValue<string>("MongoDbSettings:DatabaseName");
            }
            else
            {
                databaseName = Environment.GetEnvironmentVariable("MongoDbSettings__DatabaseName")
                               ?? throw new InvalidOperationException(
                                   "MongoDB database name not set in environment variables");
            }

            return client.GetDatabase(databaseName);
        });

        services.AddSingleton<MongoDbContext>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoDbContext(database);
        });

        return services;
    }
}