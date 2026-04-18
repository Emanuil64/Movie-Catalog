using Newtonsoft.Json;

namespace MovieCatalogExam.DTOs
{
    public class MovieDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("posterUrl")]
        public string? PosterUrl { get; set; }

        [JsonProperty("trailerLink")]
        public string? TrailerLink { get; set; }

        [JsonProperty("isWatched")]
        public bool IsWatched { get; set; }
    }
}
