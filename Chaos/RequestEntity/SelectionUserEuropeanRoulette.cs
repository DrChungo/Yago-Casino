using Chaos.Api.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chaos.Api.RequestEntity
{
    public class SelectionUserEuropeanRoulette
    {
        public List<Dictionary<int, Guid>> SelectedNumbers { get; set; } = [];
        public Guid RedNumbers { get; set; }
        public Guid BlackNumbers { get; set; }
        public Guid FirstDozen {  get; set; }
        public Guid SecondDozen {  get; set; }
        public Guid ThirdDozen {  get; set; }
        public Guid FirstHalf {  get; set; }
        public Guid SecondHalf {  get; set; }
        public Guid FirstRow { get; set; }
        public Guid SecondRow { get; set; }
        public Guid ThirdRow { get; set; }
        public Guid EvenNumbers { get; set; }
        public Guid OddNumbers { get; set; }
    }
}
