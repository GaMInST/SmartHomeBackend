using Newtonsoft.Json;

namespace SmartHomeBackend.Models
{
    public class TokenResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expire_time")]
        public int ExpireTime { get; set; }
    }
}
