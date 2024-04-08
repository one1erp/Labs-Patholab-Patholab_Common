using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Patholab_Common
{
    public class IniFile
    {
        public const int MaxSectionSize = 32767;
        private string m_path;
        [System.Security.SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint size, string ipFileName);

            [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
            public static extern uint GetPrivateProfileString(string IpAppName, string IpKeyName, string IpDefault, StringBuilder IpReturnedString, int nSize, string IpFileName);



        }
        public string Path
        {
            get { return m_path; }
        }
        public IniFile(string path)
        {
            m_path = System.IO.Path.GetFullPath(path);
        }
        public string GetString(string SectionName, string keyName, string defaultValue)
        {
            StringBuilder retVal = new StringBuilder(IniFile.MaxSectionSize);
            NativeMethods.GetPrivateProfileString(SectionName, keyName, defaultValue, retVal, IniFile.MaxSectionSize, m_path);
            return retVal.ToString();
        }

    }
}
