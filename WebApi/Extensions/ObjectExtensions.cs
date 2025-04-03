namespace WebApi.Extensions;

public static class ObjectExtensions
{
    public static T Omit<T>(this T source, params string[] propertiesToOmit) where T : class, new()
    {
        var result = new T();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            if (!propertiesToOmit.Contains(prop.Name) && prop.CanWrite)
            {
                prop.SetValue(result, prop.GetValue(source));
            }
        }

        return result;
    }
}
