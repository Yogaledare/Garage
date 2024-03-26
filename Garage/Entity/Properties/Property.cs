using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Entity.Properties;

public class Property<T> 
{
    public T? Value { get; set; }
    public string Prompt { get; private set; }
    public Func<string, Result<T>> Parser { get; private set; }

    public Property(string prompt, Func<string, Result<T>> parser)
    {
        Prompt = prompt;
        Parser = parser;
    }
}



