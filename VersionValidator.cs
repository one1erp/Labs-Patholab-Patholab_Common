using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Patholab_Common
{
    public static class VersionValidator
    {
        private static Configuration cfg;

        // Dictionary to hold reference names and their expected versions
        private static Dictionary<string, string> LoadReferenceVersionsFromConfig()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();

                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = assemblyPath + ".config" };
                cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                var appSettings = cfg.AppSettings;

                foreach (var key in appSettings.Settings.AllKeys)
                {
                    if (key.StartsWith("rv."))
                    {
                        string cleanKey = key.Substring(3);
                        dictionary[cleanKey] = appSettings.Settings[key].Value;
                    }
                }

                return dictionary;
            }
            catch
            {
                throw new Exception();
            }

        }

        // Public method to validate the versions of referenced assemblies
        public static void ValidateRefVersions(Assembly callingAssembly)
        {
            try
            {
                var name = callingAssembly.GetName().Name;

                // Get all referenced assemblies
                var referencedAssemblies = callingAssembly.GetReferencedAssemblies();

                var expectedVersions = LoadReferenceVersionsFromConfig();

                foreach (var kvp in expectedVersions)
                {
                    string assemblyName = kvp.Key;
                    string expectedVersion = kvp.Value;

                    // Retrieve the assembly with the specified name
                    var assembly = referencedAssemblies.FirstOrDefault(a => a.Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

                    if (assembly != null && !assembly.Version.ToString().Equals(expectedVersion))
                    {
                        MessageBox.Show($"Version mismatch for assembly '{assemblyName}': expected {expectedVersion}, got {assembly.Version}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionToLog(ex);
            }

        }
    }
}
