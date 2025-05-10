using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartHomeBackend.Models; // TokenApiResponse, TokenResult

namespace SmartHomeBackend.Services
{
    public class TuyaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _clientId;
        private readonly string _secret;
        private readonly string _baseUrl;

        public TuyaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            _clientId = _config["Tuya:ClientId"]!;
            _secret = _config["Tuya:ClientSecret"]!;
            _baseUrl = _config["Tuya:BaseUrl"]!;
        }

        public async Task<string> GetTokenAsync()
        {
            string signUrl = "/v1.0/token?grant_type=1";
            string method = "GET";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string signature = GenerateSignature(
                clientId: _clientId,
                secret: _secret,
                accessToken: "",
                timestamp: timestamp.ToString(),
                method: method,
                signUrl: signUrl,
                body: ""
            );

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + signUrl);
            AddHeaders(request, timestamp, signature, accessToken: null);

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendTuyaRequestAsync(string signUrl, string method, string? bodyJson = null, string? accessToken = null)
        {
            var httpMethod = new HttpMethod(method.ToUpper());
            var request = new HttpRequestMessage(httpMethod, _baseUrl + signUrl);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string signature = GenerateSignature(
                clientId: _clientId,
                secret: _secret,
                accessToken: accessToken ?? "",
                timestamp: timestamp.ToString(),
                method: method,
                signUrl: signUrl,
                body: bodyJson ?? ""
            );

            AddHeaders(request, timestamp, signature, accessToken);

            if (!string.IsNullOrEmpty(bodyJson) && (method == "POST" || method == "PUT"))
            {
                request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private void AddHeaders(HttpRequestMessage request, long timestamp, string signature, string? accessToken)
        {
            request.Headers.Add("client_id", _clientId);
            request.Headers.Add("sign", signature);
            request.Headers.Add("t", timestamp.ToString());
            request.Headers.Add("sign_method", "HMAC-SHA256");
            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Add("access_token", accessToken);
        }

        private string GenerateSignature(string clientId, string secret, string accessToken, string timestamp, string method, string signUrl, string body)
        {
            string bodyHash = "";
            if (!string.IsNullOrEmpty(body))
            {
                using var sha256 = SHA256.Create();
                var bodyBytes = Encoding.UTF8.GetBytes(body);
                var hashBytes = sha256.ComputeHash(bodyBytes);
                bodyHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            string stringToSign = method.ToUpper() + "\n" + bodyHash + "\n\n" + signUrl;
            string fullSignStr = clientId + accessToken + timestamp + stringToSign;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(fullSignStr));
            return BitConverter.ToString(signBytes).Replace("-", "").ToUpper();
        }
    }
}


