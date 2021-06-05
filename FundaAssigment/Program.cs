using FundaAssigment.FundaAPIService;
using FundaAssigment.FundaAPIService.Entities;
using System;
using System.Threading.Tasks;

namespace FundaAssigment
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            try
            {
                using (var fundaService = new FundaService())
                {
                    var top10Agents = await fundaService.GetTopAgentsAsync();

                    var top10GardenAgents = await fundaService.GetTopAgentsAsync(housingFeatures: HousingFeature.Garden);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting top agents from api. {ex.Message}", ex);
            }
        }
    }
}
