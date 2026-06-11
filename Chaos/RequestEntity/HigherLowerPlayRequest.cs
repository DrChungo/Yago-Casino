using Chaos.Api.Enums;

namespace Chaos.Api.RequestEntity
{
    public class HigherLowerPlayRequest
    {
        public Guid GameId { get; set; }
        public HigherOrLowerChoiceEnum Choice { get; set; }
    }
}
