using Azure.BlobStorage.API_Sample.Services;
using Microsoft.AspNetCore.Mvc;

namespace Azure.BlobStorage.API_Sample.Endpoints;

public static class FileEndpoints
{

    public static WebApplication AddFileEndpoints(this WebApplication app)
    {
        app.MapGet("file", async (
            [FromServices] IFileService _fileService,
            [FromQuery] string fileName,
            [FromQuery] string? containerName) =>
        {

            var file = await _fileService.GetFileAsync(fileName, containerName: containerName);

            if (file is null)
                return Results.NotFound("The file was not found");
            var memoryStream = new MemoryStream(file);
            return Results.File(fileStream: memoryStream, fileDownloadName: fileName);


        });
        app.MapPost("file", async (
            [FromServices] IFileService _fileService,
            [FromQuery] string? containerName,
            [FromForm] IFormFile file) =>
        {



            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var response = await _fileService.UploadFileAsync(stream.ToArray(), file.FileName, containerName);

                if (response)
                    return Results.Ok("file uploaded");

            }

            return Results.BadRequest("File not uploaded");


        }).DisableAntiforgery();


        app.MapDelete("file", async (
            [FromServices] IFileService _fileService,
            [FromQuery] string containerName,
            [FromQuery] string fileName) =>
        {

            return await _fileService.DeleteFileAsync(fileName, containerName)
             ? Results.Ok("The file was deleted")
             : Results.BadRequest();

        });



        return app;

    }
}







