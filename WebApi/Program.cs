using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

var databaseConnection = builder.Configuration.GetSection("DatabaseSettings");

builder.Services.Configure<DatabaseSettings>(databaseConnection);
builder.Services.AddScoped<MovieService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebApi"
    });

    options.DocumentFilter<Filter>();

});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Minimal API Version 1.0");

app.MapGet("/check", async (MovieService movieService) => 
{
    return await movieService.CheckDatabaseConnectionAsync();
});


app.MapPost("/api/movies", async (MovieService movieService,[FromBody] Movie movie) =>
{
    try
    {
        await movieService.InsertMovieAsync(movie);

        return Results.Ok();
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }
});


app.MapGet("api/movies/{id}", async (MovieService movieService, string id) =>
{
    Movie? movie = await movieService.GetMovieByIdAsync(id);

    if(movie != null)
    {
        return Results.Ok(movie);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapGet("api/movies", async (MovieService movieService) =>
{
    List<Movie>? movies = await movieService.GetMovies();

    if(movies != null)
    {
        return Results.Ok(movies);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPut("api/movies", async (MovieService movieService, Movie movie) => 
{
    try
    {
        await movieService.UpdateMovieAsync(movie);

        return Results.Ok();
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }
});

app.MapDelete("api/movies/{id}", async (MovieService movieService, string id) => 
{
    try
    {
        await movieService.DeleteMovieAsync(id);

        return Results.Ok();
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }
});


app.Run();
