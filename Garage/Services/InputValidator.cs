using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Garage.Entity;
using LanguageExt.Common;

namespace Garage.Services;

public static class InputValidator {
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


    public static Result<object> ValidateProperty(string? input, PropertyInfo propertyInfo, ITypeConversionService converter,
        IGarageHandler<IVehicle> garageHandler) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<object>(error);
        }

        var conversionResult = converter.TryConvert(input, propertyInfo);

        return conversionResult.Match(
            Succ: convertedValue => {
                var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
                foreach (var attribute in validationAttributes) {
                    if (!attribute.IsValid(convertedValue)) {
                        var error = new ValidationException(attribute.ErrorMessage);
                        return new Result<object>(error);
                    }
                }
                
                if (propertyInfo.Name.Equals(nameof(IVehicle.LicencePlate), StringComparison.OrdinalIgnoreCase)) {
                    var exists = garageHandler.DoesLicencePlateExist(convertedValue as string);
                    if (exists) {
                        var error = new ValidationException($"License plate {input} is already in use.");
                        return new Result<object>(error);
                    }
                }

                return new Result<object>(convertedValue);
            },
            Fail: e => new Result<object>(e)
        );
    }


    public static Result<int> ValidateNumberBounded(string? input, int min, int max) {
        var normalNumberValidationResult = ValidateNumber(input);

        return normalNumberValidationResult.Match(
            Succ: number => {
                if (number < min || number > max) {
                    var error = new ValidationException($"Must be within [{min}, {max}] (inclusive)");
                    return new Result<int>(error);
                }

                return number;
            },
            Fail: _ => normalNumberValidationResult);
    }


    public static Result<int> ValidateNumber(string? input) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<int>(error);
        }

        var tokens = input.Split(' ');

        if (tokens.Length > 1) {
            var error = new ValidationException("Error: too many inputs");
            return new Result<int>(error);
        }

        if (!int.TryParse(tokens[0], out int number)) {
            var error = new ValidationException("Error: cannot parse integer");
            return new Result<int>(error);
        }

        return number;
    }
}