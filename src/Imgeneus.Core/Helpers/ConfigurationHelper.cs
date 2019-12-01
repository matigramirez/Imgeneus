using System.IO;
using System.Text.Json;

namespace Imgeneus.Core.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Deserealized an object from json path.
        /// </summary>
        /// <typeparam name="T">The configuration class.</typeparam>
        /// <param name="path">The configuration file path.</param>
        /// <returns>A deserealized object</returns>
        public static T Load<T>(string path) where T : class, new()
        {
            if (!File.Exists(path))
            {
                Save(path, new T());
            }

            return JsonSerializer.Deserialize<T>(File.ReadAllText(path));

        }

        /// <summary>
        /// Serialize the current object instance.
        /// </summary>
        /// <typeparam name="T">The object class.</typeparam>
        /// <param name="path">The file path.</param>
        /// <param name="value">The object instance.</param>
        public static void Save<T>(string path, T value) where T : class, new()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllText(path, JsonSerializer.Serialize(value));
        }
    }
}
