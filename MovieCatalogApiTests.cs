using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using MovieCatalogExam.DTOs;

namespace MovieCatalogExam
{
    [TestFixture]
    [NonParallelizable]
    public class MovieCatalogTests
    {
        private RestClient client;
        private static string token;
        private static string createdMovieId;

        [OneTimeSetUp]
        public void Setup()
        {
            client = new RestClient("http://144.91.123.158:5000");

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                email = "emanuil64@abv.bg",
                password = "246773Hyper"
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var json = JsonConvert.DeserializeObject<dynamic>(response.Content!);
            Assert.That(json.accessToken.ToString(), Is.Not.Null.And.Not.Empty);

            token = json.accessToken.ToString();
        }

        [Test, Order(1)]
        public void CreateMovie()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");

            request.AddJsonBody(new
            {
                title = $"Test Movie {DateTime.Now.Ticks}",
                description = "Test Description",
                posterUrl = "",
                trailerLink = "",
                isWatched = false
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var data = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content!);

            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Movie, Is.Not.Null);
            Assert.That(data.Movie.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(data.Msg, Is.EqualTo("Movie created successfully!"));

            createdMovieId = data.Movie.Id;
        }

        [Test, Order(2)]
        public void EditMovie()
        {
            Assert.That(createdMovieId, Is.Not.Null.And.Not.Empty);

            var request = new RestRequest($"/api/Movie/Edit?movieId={createdMovieId}", Method.Put);
            request.AddHeader("Authorization", $"Bearer {token}");

            request.AddJsonBody(new
            {
                title = "Edited Movie",
                description = "Edited Description",
                posterUrl = "",
                trailerLink = "",
                isWatched = true
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var data = JsonConvert.DeserializeObject<ApiResponseDTO>(response.Content!);

            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Msg, Is.EqualTo("Movie edited successfully!"));
        }

        [Test, Order(3)]
public void GetAllMovies()
{
    var request = new RestRequest("/api/Catalog/All", Method.Get);
    request.AddHeader("Authorization", $"Bearer {token}");

    var response = client.Execute(request);

    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

    var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(response.Content);

    Assert.That(movies, Is.Not.Null);
    Assert.That(movies!.Count, Is.GreaterThan(0));
}

        [Test, Order(4)]
        public void DeleteMovie()
        {
            Assert.That(createdMovieId, Is.Not.Null.And.Not.Empty);

            var request = new RestRequest($"/api/Movie/Delete?movieId={createdMovieId}", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            Assert.That(response.Content, Does.Contain("Movie deleted successfully!"));
        }

        [Test, Order(5)]
        public void CreateMovieWithoutRequiredFields()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");

            request.AddJsonBody(new
            {
                posterUrl = "",
                trailerLink = "",
                isWatched = false
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditNonExistingMovie()
        {
            var request = new RestRequest("/api/Movie/Edit?movieId=invalid-id", Method.Put);
            request.AddHeader("Authorization", $"Bearer {token}");

            request.AddJsonBody(new
            {
                title = "Test",
                description = "Test",
                posterUrl = "",
                trailerLink = "",
                isWatched = false
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            Assert.That(
                response.Content,
                Does.Contain("Unable to edit the movie! Check the movieId parameter or user verification!"));
        }

        [Test, Order(7)]
        public void DeleteNonExistingMovie()
        {
            var request = new RestRequest("/api/Movie/Delete?movieId=invalid-id", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            Assert.That(
                response.Content,
                Does.Contain("Unable to delete the movie! Check the movieId parameter or user verification!"));
        }
    }
}
