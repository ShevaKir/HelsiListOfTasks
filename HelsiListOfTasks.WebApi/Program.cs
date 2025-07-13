using HelsiListOfTasks.Application.Interfaces;
using HelsiListOfTasks.Application.Services;
using HelsiListOfTasks.Domain.Repositories;
using HelsiListOfTasks.Infrastructure.Mongo;
using HelsiListOfTasks.WebApi.Extensions;
using Microsoft.OpenApi.Models;

namespace HelsiListOfTasks.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Task Lists API",
                Version = "v1"
            });
        });
        
        builder.Services.AddMongoDb(builder.Configuration, builder.Environment.IsDevelopment());
        
        builder.Services.AddScoped<ITaskListRepository, MongoTaskListRepository>();
        builder.Services.AddScoped<ITaskListSharingRepository, MongoTaskListSharingRepository>();
        builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
        
        builder.Services.AddScoped<ITaskListSharingService, TaskListSharingService>();
        builder.Services.AddScoped<ITaskListService, TaskListService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}