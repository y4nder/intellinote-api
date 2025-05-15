using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi.Extensions;

public static class JsonExtensions
{
    public static string ToCamelCaseJson(this object obj) =>
        JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });
}
