using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LanguageExt.Common;

namespace Garage.Services;

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