using Newtonsoft.Json;

namespace SmartHomeBackend.Models
{
    public class TokenApiResponse
    {
        [JsonProperty("result")]
        public TokenResult Result { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
