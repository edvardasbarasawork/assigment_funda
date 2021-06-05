using Newtonsoft.Json;

namespace FundaAssigment.FundaAPIService.Entities
{
    public class Offer
    {
        /// <summary>
        /// Id of real estate agent
        /// </summary>
        [JsonProperty("MakelaarId")]
        public int AgentId { get; set; }
    }
}
