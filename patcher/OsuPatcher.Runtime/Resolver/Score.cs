using System.Linq;
using System.Reflection;

namespace OsuPatcher.Runtime.Resolver
{
    internal static class Score
    {
        private static FieldInfo _currentScoreField;

        public static FieldInfo GetCurrentScoreField(object playerInstance)
        {
            if (_currentScoreField != null)
                return _currentScoreField;

            var playerType = playerInstance.GetType();
            _currentScoreField = playerType
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(f =>
                {
                    var fieldType = f.FieldType;
                    var fields = fieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    return fields.Count(field => field.FieldType == typeof(ushort)) >= 6;
                });

            return _currentScoreField;
        }
    }
}
