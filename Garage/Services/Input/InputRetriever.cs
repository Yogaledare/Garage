using LanguageExt.Common;
using static Garage.Services.Input.InputValidator;

namespace Garage.Services.Input;

public static class InputRetriever {
    public static T RetrieveInput<T>(string prompt, Func<string?, Result<T>> validator) {
        T? output = default;

        while (true) {
            Console.Write(prompt);
            var input = Console.ReadLine();
            var result = validator(input);

            bool shouldBreak = result.Match(
                Succ: validatedSentence => {
                    output = validatedSentence;
                    return true;
                },
                Fail: ex => {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            );

            if (shouldBreak) {
                break;
            }
        }

        if (output == null) throw new InvalidOperationException("Parsing failed");
        return output;
    }


    public static T SelectFromMenu<T>(IEnumerable<(string Description, T Value)> options) {
        Console.WriteLine($"Select {typeof(T).Name}");

        var index = 0;
        var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
        foreach (var option in enumerable) {
            Console.WriteLine($"{index++}. {option.Description}");
        }

        var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));

        return enumerable.ElementAt(input).Value;
    }
    
    //
    // public static T SelectFromMenu<T>(IEnumerable<T> options) {
    //     Console.WriteLine($"Select {typeof(T).Name}");
    //
    //     var index = 0;
    //     var enumerable = options as (string Description, T Value)[] ?? options.ToArray();
    //     foreach (var option in enumerable) {
    //         Console.WriteLine($"{index++}. {option.Description}");
    //     }
    //
    //     var input = RetrieveInput("Choice: ", s => ValidateNumberBounded(s, 0, index - 1));
    //
    //     return enumerable.ElementAt(input).Value;
    // }
    //
    
}