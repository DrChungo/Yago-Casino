namespace Chaos.Api.Enums
{
    public enum HealthEnum
    {
        YAGUETE = 100, // 100%
        EXCELENT = 75, //  50%
        HEALTHY = 50, // 0%
        RECOVERING = 30,
        SICK = 25, // -30%
        CRITICAL = 10, // -80%
    }
}
