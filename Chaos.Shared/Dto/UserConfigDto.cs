using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chaos.Shared.Dto
{
    public class UserConfigDto
    {
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("wallet")]
        public decimal Balance { get; set; }

        [JsonPropertyName("isAlive")]
        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }
    }
}
