namespace ListViewColumnStore
{
    using System.IO;
    using System.Reflection;
    using System.Text.Json;

    public static class Serializer
    {
        public static string SettingsDirectory(string gridLayoutName)
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), assemblyName, $"{gridLayoutName}.json");
        }

        public static void ToJson<T>(T obj, string fileOrContent)
        {
            string resultJson = JsonSerializer.Serialize<T>(obj);
            using (StreamWriter textWriter = new StreamWriter(fileOrContent))
            {
                textWriter.Write(resultJson);
                textWriter.Flush();
                textWriter.Close();
            }
        }

        public static T FromJson<T>(string fileOrContent)
        {
            if (File.Exists(fileOrContent) == false)
            {
                return default;
            }

            string jsonString = File.ReadAllText(fileOrContent);
            return (T)JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
