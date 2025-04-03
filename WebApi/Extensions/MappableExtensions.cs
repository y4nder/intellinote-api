using WebApi.Generics;

namespace WebApi.Extensions;

public static class MappableExtensions
{
    public static TCommand Map<TCommand>(this IMappable<TCommand> contract)
        where TCommand : new()
    {
        var command = new TCommand();
        foreach (var property in contract.GetType().GetProperties())
        {
            var commandProperty = typeof(TCommand).GetProperty(property.Name);
            if (commandProperty != null)
            {
                commandProperty.SetValue(command, property.GetValue(contract));
            }
        }
        return command;
    }
}