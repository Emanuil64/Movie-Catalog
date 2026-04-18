using Newtonsoft.Json;

namespace MovieCatalogExam.DTOs
{
    public class ApiResponseDTO
    {
        [JsonProperty("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonProperty("movie")]
        public MovieDTO Movie { get; set; } = new MovieDTO();
    }
}
