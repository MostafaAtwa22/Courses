using System.Reflection;

namespace Domain.Shared
{
    public abstract class Enumeration<TEnum>(int value, string name) : IEquatable<Enumeration<TEnum>>
        where TEnum : Enumeration<TEnum>
    {
        private static readonly Dictionary<int, TEnum> _enumerations = CreateEnumerations();

        public int Value { get; protected init; } = value;
        public string Name { get; protected init; } = name;

        public static TEnum? FromValue(int value)
            => _enumerations.TryGetValue(value, out var enumeration) ? enumeration : null;
        
        public static TEnum? FromName(string name)
            => _enumerations.Values.SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public static IReadOnlyCollection<TEnum> GetValues() =>
            _enumerations.Values.ToList().AsReadOnly();

        public bool Equals(Enumeration<TEnum>? other) =>
            other is not null &&
            GetType() == other.GetType() &&
            Value == other.Value;

        public override bool Equals(object? obj) =>
            obj is Enumeration<TEnum> other && Equals(other);

        override public int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Name;

        private static Dictionary<int, TEnum> CreateEnumerations()
        {
            var enumerationType = typeof(TEnum);

            var fieldsForType = enumerationType
                .GetFields(
                    BindingFlags.Public | 
                    BindingFlags.Static | 
                    BindingFlags.DeclaredOnly)
                .Where(f => enumerationType.IsAssignableFrom(f.FieldType))
                .Select(f => (TEnum)f.GetValue(null)!);

            return fieldsForType.ToDictionary(e => e.Value);
        }
    }
}