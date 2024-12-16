using Azure.BlobStorage.API_Sample.Endpoints;
using Azure.BlobStorage.API_Sample.Options;
using Azure.BlobStorage.API_Sample.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<AzureBlobOptions>(builder.Configuration.GetSection("AzureBlobOptions"));

builder.Services.AddSingleton<IFileService, FileService>();




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    options.Theme = ScalarTheme.BluePlanet);
}

app.UseHttpsRedirection();

app.AddFileEndpoints();


await app.RunAsync();

