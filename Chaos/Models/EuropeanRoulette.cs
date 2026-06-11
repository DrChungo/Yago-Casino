using Chaos.Api.Enums.EnumsEuropeanRoulette;

namespace Chaos.Api.Models
{
    public class EuropeanRoulette
    {
        public RedNumbers RedNumbers { get; set; }
        public BlackNumbers BlackNumbers { get; set; }
        public FirstDozen FirstDozen { get; set; }
        public SecondDozen SecondDozen { get; set; }
        public ThirdDozen ThirdDozen { get; set; }
        public FirstHalf FirstHalf { get; set; }
        public SecondHalf SecondHalf { get; set; }
        public FirstRow FirstRow { get; set; }
        public SecondRow SecondRow { get; set; }
        public ThirdRow ThirdRow { get; set; }


    }
}
