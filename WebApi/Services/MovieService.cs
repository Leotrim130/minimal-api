using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public class MovieService
    {
        private readonly IOptions<DatabaseSettings> _options;

        public MovieService(IOptions<DatabaseSettings> options)
        {
            _options = options;
        } 

        public async Task<string> CheckDatabaseConnectionAsync()
        {
            try
            {
                var client = new MongoClient(_options.Value.ConnectionString);

                var dbsList = await client.ListDatabasesAsync();

                string dbs = "";

                foreach(var db in dbsList.ToList())
                {
                    dbs += db["name"] + ", ";
                }

                return $"Zugriff auf MongoDB ok. Vorhandene DBs: {dbs.Remove(dbs.Length - 2)}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task InsertMovieAsync(Movie movie)
        {
                var client = new MongoClient(_options.Value.ConnectionString);
                var database = client.GetDatabase("library");   
                var collection = database.GetCollection<Movie>("movies");

                await collection.InsertOneAsync(movie);
        }

        public async Task<List<Movie>?> GetMovies()
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase("library");   
            var collection = database.GetCollection<Movie>("movies");

            var result = await collection.FindAsync(new BsonDocument());

            var movies = await result.ToListAsync();

            return movies;
        }

        public async Task<Movie?> GetMovieByIdAsync(string id)
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase("library");   
            var collection = database.GetCollection<Movie>("movies");

            var filter = Builders<Movie>.Filter.Eq(m => m.Id, id);
            var result = await collection.FindAsync(filter);

            var movie = await result.FirstOrDefaultAsync();

            return movie;
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase("library");   
            var collection = database.GetCollection<Movie>("movies");

            var filter = Builders<Movie>.Filter.Eq(m => m.Id, movie.Id);

            var result = await collection.ReplaceOneAsync(filter, movie);
        }

        public async Task DeleteMovieAsync(string id)
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase("library");   
            var collection = database.GetCollection<Movie>("movies");

            var filter = Builders<Movie>.Filter.Eq(m => m.Id, id);

            var result = await collection.DeleteOneAsync(filter);
        }


    }
}