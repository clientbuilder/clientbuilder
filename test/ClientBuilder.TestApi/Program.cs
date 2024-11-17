using ClientBuilder.Extensions;
using ClientBuilder.TestAssembly.Modules.EmptyTest;
using ClientBuilder.TestAssembly.Modules.SimpleTest;
using ClientBuilder.TestAssembly.Modules.TestWithError;
using ClientBuilder.TestAssembly.Modules.TestWithPartialError;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddClientBuilder(options =>
    {
        options.ContentRootPath = builder.Environment.ContentRootPath;

        options.AddAssembly("ClientBuilder.TestAssembly");
        options.AddAssembly("ClientBuilder.TestApi");

        options.AddClient("test.client", "Test", 1, "GenerationResultFolder");
        options.AddModule<EmptyTestModule>();
        options.AddModule<SimpleTestModule>();
        options.AddModule<TestWithErrorModule>();
        options.AddModule<TestWithPartialErrorModule>();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseClientBuilder();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();