using Application.Extensions;

namespace Infrastructure.Mapping
{
    public class ImportMap
    {
        private string? _sourceValue;
        private object? _convertedValue;

        public ImportMap() { }
        public ImportMap(object data, string propName)
        {
            IsStoredInNtp = true;
            IsRequired = true;
            PropertyName = propName;
            PropertyType = GetPropertyType(data, propName);
        }

        public bool IsInSourceData { get; set; } = false;
        public bool IsStoredInNtp { get; set; } = false;
        public string? PropertyName { get; }
        public Type? PropertyType { get; }
        public bool IsRequired { get; set; } = false;
        public bool HasConvertedValue { get { return _convertedValue != null; } }
        public string? SourceValue { get { return _sourceValue; } }
        public int? RecommendedMaxStringLength { get; set; } = null;

        private static Type? GetPropertyType(object data, string propName)
        {
            var propertyInfo = data.GetType().GetProperty(propName);
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }
            return null;
        }

        public void ClearValue()
        {
            _sourceValue = null;
            IsInSourceData = false;
            _convertedValue = null;
        }

        public void SetValue(string? sourceValue)
        {
            _sourceValue = sourceValue;
            IsInSourceData = true;
            if (!string.IsNullOrWhiteSpace(PropertyName) &&
                PropertyType != null &&
                !string.IsNullOrWhiteSpace(_sourceValue))
            {
                if (PropertyType == typeof(string))
                {
                    _convertedValue = _sourceValue;
                }
                else if (PropertyType == typeof(bool))
                {
                    _convertedValue = _sourceValue.ParseBoolean();
                }
                else if (PropertyType == typeof(DateOnly))
                {
                    _convertedValue = _sourceValue.ParseDateOnly();
                }
                else if (PropertyType == typeof(DateTime))
                {
                    _convertedValue = _sourceValue.ParseDateTime();
                }
                else if (PropertyType == typeof(decimal))
                {
                    _convertedValue = _sourceValue.ParsePrice();
                }
                else
                {
                    _convertedValue = null;
                }
            }
        }

        public void ApplyConvertedValueToProperty(object data)
        {
            if (!string.IsNullOrWhiteSpace(PropertyName) && _convertedValue != null)
            {
                var propertyInfo = data.GetType().GetProperty(PropertyName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(data, _convertedValue);
                }
            }
        }
    }
}
