using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Garage.Entity;
using Garage.Entity.Vehicles;
using LanguageExt.Common;

namespace Garage.Services.Conversion;

public class TypeConversionService : ITypeConversionService {
    private readonly Dictionary<Type, Func<string, object?>> _converters;

    public TypeConversionService() {
        _converters = new Dictionary<Type, Func<string, object?>> {
            {typeof(int), s => int.TryParse(s, out int i) ? i : null},
            {typeof(double), s => double.TryParse(s, out double d) ? d : null},
            {typeof(VehicleColor), s => Enum.TryParse(s, true, out VehicleColor vc) ? vc : null},
            {typeof(string), s => s},
        };
    }


    public Result<object> TryConvert(string input, PropertyInfo property) {
        if (!_converters.TryGetValue(property.PropertyType, out var converter)) {
            var error = new ValidationException($"No converter available for type: {property.PropertyType.Name}.");
            return new Result<object>(error);
        }

        var convertedValue = converter(input);

        if (convertedValue is null) {
            var error = new ValidationException($"Conversion failed for input: {input}.");
            return new Result<object>(error);
        }

        return new Result<object>(convertedValue);
    }
}




//
// using System.ComponentModel.DataAnnotations;
// using System.Reflection;
// using Garage.Entity;
// using LanguageExt.Common;
//
// namespace Garage.Services.Conversion;
//
// public class TypeConversionService : ITypeConversionService {
//     private readonly Dictionary<Type, Delegate> _converters;
//
//     public TypeConversionService() {
//         _converters = new Dictionary<Type, Delegate> {
//             {typeof(int), new Func<string, int?>(s => int.TryParse(s, out int i) ? i : null)},
//             {typeof(double), new Func<string, double?>(s => double.TryParse(s, out double d) ? d : null)},
//             {typeof(VehicleColor), new Func<string, VehicleColor?>(s => Enum.TryParse(s, true, out VehicleColor vc) ? vc : null)},
//             {typeof(string), new Func<string, string?>(s => s)},
//         };
//     }
//
//
//     public Result<T> TryConvert<T>(string input) {
//         if (!_converters.TryGetValue(typeof(T), out var converter)) {
//             var error = new ValidationException($"No converter available for type: {typeof(T).Name}.");
//             return new Result<T>(error);
//         }
//         
//         var typedConverter = (Func<string, T>) converter;
//         var convertedValue = typedConverter(input);
//
//         if (convertedValue is null) {
//             var error = new ValidationException($"Conversion failed for input: {input}.");
//             return new Result<T>(error);
//         }
//
//         return new Result<T>(convertedValue);
//     }
// }


// private readonly Dictionary<Type, Func<string, object?>> _converters;
//
//
// public TypeConversionService() {
//     _converters = new Dictionary<Type, Func<string, object?>> {
//         {typeof(int), s => int.TryParse(s, out int i) ? i : null},
//         {typeof(double), s => double.TryParse(s, out double d) ? d : null},
//         {typeof(VehicleColor), s => Enum.TryParse(s, true, out VehicleColor vc) ? vc : null},
//         {typeof(string), s => s},
//     };
// }