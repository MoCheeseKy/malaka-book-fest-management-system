using NpgsqlTypes;

namespace MalakaBookFest.Core.Enums;

public enum BoothCategory
{
    [PgName("Publisher")] Publisher,
    [PgName("IndieAuthor")] IndieAuthor,
    [PgName("Merchandise")] Merchandise,
    [PgName("FoodBeverage")] FoodBeverage,
    [PgName("Other")] Other
}