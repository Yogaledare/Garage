using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Garage.Entity;
using Garage.Services.Conversion;
using Garage.Services.GarageHandler;
using LanguageExt.Common;

namespace Garage.Services.Input;

public static class InputValidator {



    // public static Result<double> ValidateSpeed(string? input, ) {
    //     
    // }
    

    public static Result<string> ValidateLicensePlate(string? input, IGarageHandler<IVehicle> garageHandler) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<string>(error);
        }
        
        var pattern = "^[A-Z]{3}[0-9]{3}$";
        if (!Regex.IsMatch(input, pattern)) {
            var error = new ValidationException("License plate must be three capital letters followed by three digits.");
            return new Result<string>(error);
        }
        
        return input;
    }


    public static Result<string> ValidateLicensePlateSearch(string? input) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<string>(error);
        }

        var pattern = "^[A-Za-z]{3}[0-9]{3}$";
        if (!Regex.IsMatch(input, pattern)) {
            var error = new ValidationException("License plate must be three letters followed by three digits.");
            return new Result<string>(error);
        }

        return input;
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


    public static Result<double> ValidateDoubleBounded(string? input, double min, double max) {
        if (string.IsNullOrEmpty(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<double>(error);
        }
        
        var tokens = input.Split(' ');

        if (tokens.Length > 1) {
            var error = new ValidationException("Error: too many inputs");
            return new Result<double>(error);
        }
        
        if (!double.TryParse(tokens[0], out var number)) {
            var error = new ValidationException("Error: cannot parse integer");
            return new Result<double>(error);
        }
        
        if (number < min || number > max) {
            var error = new ValidationException($"Must be within [{min}, {max}] (inclusive)");
            return new Result<double>(error);
        }
        
        return number;
        
        
        
    }
}

//
//
// public static Result<object> ValidateProperty(string? input, PropertyInfo propertyInfo, ITypeConversionService converter,
//     IGarageHandler<IVehicle> garageHandler) {
//     if (string.IsNullOrEmpty(input)) {
//         var error = new ValidationException("Error: null or empty input");
//         return new Result<object>(error);
//     }
//
//     var conversionResult = converter.TryConvert(input, propertyInfo);
//
//     return conversionResult.Match(
//         Succ: convertedValue => {
//             var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
//             foreach (var attribute in validationAttributes) {
//                 if (!attribute.IsValid(convertedValue)) {
//                     var error = new ValidationException(attribute.ErrorMessage);
//                     return new Result<object>(error);
//                 }
//             }
//                 
//             if (propertyInfo.Name.Equals(nameof(IVehicle.LicencePlate), StringComparison.OrdinalIgnoreCase)) {
//                 var exists = garageHandler.DoesLicencePlateExist(convertedValue as string);
//                 if (exists) {
//                     var error = new ValidationException($"License plate {input} is already in use.");
//                     return new Result<object>(error);
//                 }
//             }
//
//             return new Result<object>(convertedValue);
//         },
//         Fail: e => new Result<object>(e)
//     );
// }
//
