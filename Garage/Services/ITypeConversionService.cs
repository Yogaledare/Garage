using System.Reflection;
using LanguageExt.Common;

namespace Garage.Services;

public interface ITypeConversionService {
    Result<object> TryConvert(string input, PropertyInfo property);
}