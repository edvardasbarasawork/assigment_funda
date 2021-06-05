using FundaAssigment.FundaAPIService.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FundaAssigment.FundaAPIService
{
    public class FundaService : IDisposable
    {
        private HttpClient _httpClient;

        private static readonly int _maxPageSize = 25;
        private static readonly int _apiRequestDelay = (int)(60 / 100m * 1000);
        private static readonly string _apiKey = "ac1b0b1572524640a0ecc54de453ea9f";
        private static readonly string _apiUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json";

        public FundaService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{_apiUrl}/{_apiKey}/?")
            };
        }

        public async Task<List<int>> GetTopAgentsAsync(int agentsCount = 10,
            City city = City.Amsterdam,
            HousingFeature? housingFeatures = null)
        {
            var pageNumber = 0;
            var completedIn = _apiRequestDelay;

            var offers = new List<Offer>();
            var agents = new Dictionary<int, int>();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            do
            {
                if (_apiRequestDelay - completedIn > 0)
                {
                    await Task.Delay(_apiRequestDelay - completedIn);
                }

                watch.Restart();

                pageNumber++;
                offers = await GetOffers(pageNumber, city, housingFeatures);

                foreach (var offer in offers)
                {
                    if (!agents.TryGetValue(offer.AgentId, out int offersCount))
                    {
                        agents.Add(offer.AgentId, 0);
                    }

                    agents[offer.AgentId]++;
                }

                watch.Stop();
                completedIn = (int)watch.ElapsedMilliseconds;
            }
            while (offers.Count > 0);

            return agents.OrderByDescending(x => x.Value).Take(agentsCount).Select(i => i.Key).ToList();
        }

        private async Task<List<Offer>> GetOffers(int pageNumber,
            City city,
            HousingFeature? housingFeatures)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["type"] = "koop";
            query["page"] = pageNumber.ToString();
            query["pagesize"] = _maxPageSize.ToString();
            query["zo"] = $"/{city}/{housingFeatures?.ToApiName() ?? string.Empty}";

            var response = await _httpClient.GetAsync($"?{query}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting data from Api. Request {query}. Response {response.StatusCode} {responseContent}.");
            }

            return JsonConvert.DeserializeObject<ApiResponse>(responseContent).Objects;
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }
    }
}
