using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Utilities
{
    internal static class ResourceAPI
    {
        public static string GetStringResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(resourceStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static byte[] GetByteResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                return ReadAllBytes(resourceStream);
            }
        }

        public static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static List<string> ReadAllLines(string nameSpace, string resource)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            StreamReader reader = getResourceStream(nameSpace, resource, assembly);
            return extractValidlines(reader);
        }

        public static string ReadAllText(string nameSpace, string resource)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            StreamReader reader = getResourceStream(nameSpace, resource, assembly);
            return reader.ReadToEnd();
        }

        private static StreamReader getResourceStream(string nameSpace, string resource, Assembly assembly)
        {
            resource = resource.Replace('\\', '.').Replace('/', '.');
            return new StreamReader(assembly.GetManifestResourceStream(nameSpace + '.' + resource));
        }

        private static List<string> extractValidlines(StreamReader reader)
        {
            List<string> ret = new List<string>();
            string s = null;
            while ((s = reader.ReadLine()) != null)
            {
                //if its not a comment or empty line
                if (s.Trim().Length > 0 && s[0] != '#')
                {
                    ret.Add(s.Trim());
                }
            }
            reader.Close();
            return ret;
        }

    }
}
