using Garage.Entity;

namespace Garage.Validation;

using FluentValidation;

public class LicensePlateValidator : AbstractValidator<string> {
    public LicensePlateValidator() {
        // Rule for license plate format: Three uppercase letters followed by three digits
        RuleFor(x => x)
            .Matches(@"^[A-Z]{3}\d{3}$")
            .WithMessage("License plate must be three capital letters followed by three digits.");
    }
}

public class VehicleValidator : AbstractValidator<Vehicle> {
    public VehicleValidator() {
        RuleFor(v => v.LicencePlate)
            .NotNull()
            .Matches(@"^[A-Z]{3}\d{3}$");
        RuleFor(v => v.NumWheels)
            .NotNull()
            .GreaterThan(0);
    }
}

public class NumericStringValidator : AbstractValidator<string?> {
    public NumericStringValidator(int min, int max) {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotNull()
            .Must(x => IsValidInteger(x!))
            .WithMessage("Must be able to read as number")
            .Must(x => IsWithinRange(x!, min, max))
            .WithMessage($"Input must be between {min} and {max}.")
            ; 

        // RuleFor(x => x)
        // .Must(x => IsValidInteger(x!))
        // .WithMessage("Must be able to read as number")
        // .When(x => x is not null); 

        // RuleFor(x => x)
        // .Must(x => IsWithinRange(x!, min, max))
        // .WithMessage($"Input must be between {min} and {max}.")
        // .When(x => x is not null); 


        // .Must(x => IsValidInteger(x))
        // .WithMessage("Must be able to read as number")
        // .Must(x => IsWithinRange(x, min, max))
        // .WithMessage($"Value must be a number between {min} and {max}");
    }

    private static bool IsValidInteger(string value) {
        return int.TryParse(value, out _);
    }

    private static bool IsWithinRange(string value, int min, int max) {
        if (int.TryParse(value, out var number)) {
            return number >= min && number <= max;
        }

        // If parsing fails, we don't need to validate the range,
        // because the IsAValidInteger rule already covers the scenario of invalid integers.
        return false;
    }
}

// public class NumericStringInputValidator : AbstractValidator<Program.NumericStringInput> {
//     // todo implementation
// }

public class IntegerInputValidator : AbstractValidator<string> {
}


// .Must(s => int.TryParse(s, out _))


// .WithMessage("Must be able to read as number.")
// .DependentRules(() => {
// RuleFor(v => v)
// });


// {


// return int.TryParse(value, out var number) && number >= min && number <= max;
// })
// .WithMessage($"Value must be a number between {min} and {max}.");


// public class VehiclePropertiesValidator : AbstractValidator<Dictionary<string, object>> {
//     public VehiclePropertiesValidator(Type vehicleType) {
//         var biggestConstructor = vehicleType.GetConstructors()
//             .MaxBy(c => c.GetParameters().Length);
//
//
//         // RuleFor(biggestConstructor => biggestConstructor).NotNull(); 
//
//
//         // other rulefor
//     }
// }
//
//
// // .SelectMany(c => c.GetParameters())
// // .dis