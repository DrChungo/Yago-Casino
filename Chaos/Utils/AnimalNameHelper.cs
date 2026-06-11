namespace Chaos.Api.Utils
{
    public class AnimalNameHelper
    {
        /// <summary>
        /// Utility for generate random names
        /// </summary>
        /// <param name="random"></param>
        /// <returns>Random name</returns>
        public static string GetRandomName(Random random)
        {
            string[] nombres = ["Pepe", "Manolo", "Nil", "Maria", "Sofia", "Pere", "Yago"];
            return nombres[random.NextInt64(0, nombres.Length)];
        }
    }
}


