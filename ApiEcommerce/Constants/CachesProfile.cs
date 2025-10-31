

namespace ApiEcommerce.Constants;

using Microsoft.AspNetCore.Mvc;
public class CacheProfiles
{

    public const string Default10 = "Default10";
    public const string Default20 = "Default20";

    public static readonly CacheProfile Default10Profile = new()
    {
        Duration = 10
    };

    public static readonly CacheProfile Default20Profile = new()
    {
        Duration = 20
    };
    
    


}
