using System.Reflection;

namespace CSVReader.Extensions
{
    internal static class PropertyExtensions
    {
        #region Public Methods

        public static T GetAttribute<T>(this PropertyInfo property)
            where T : class
        {
            return property.GetCustomAttribute(typeof(T)) as T;
        }

        #endregion Public Methods
    }
}