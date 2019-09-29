using System;
using Newtonsoft.Json;

namespace IntakeApi.Entities
{
    public class IntakeEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal InitialDeposit { get; set; }
        public int HorizonMonth { get; set; }
        public int HorizonYear { get; set; }
        public decimal GoalAmount { get; set; }
        public InvestmentProfile InvestmentProfile { get; set; }
    }

    public enum InvestmentProfile
    {
        Defensive = 1,
        Medium,
        Offensive
    }
}