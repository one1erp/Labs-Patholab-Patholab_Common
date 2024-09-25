using System;
using System.Drawing;
using System.Linq;
using LSSERVICEPROVIDERLib;
using Microsoft.Win32;
//using Oracle.DataAccess.Client;
using Oracle.ManagedDataAccess.Client;

namespace Patholab_Common
{
    public static class Utils
    {
        private static string _conString;
        private static INautilusDBConnection _ntlsCon;

        public static string ConString
        {
            get { return _conString; }
        }

        public static INautilusDBConnection NautilusDbConnection
        {
            get { return _ntlsCon; }
        }

        public static void CreateConstring(INautilusDBConnection ntlsCon)
        {           
            if (ntlsCon != null)
            {
                OracleConnectionStringBuilder oraBuilder = new OracleConnectionStringBuilder();

                var username = ntlsCon.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    username = "/";

                }
                //Ashi check check
                oraBuilder.UserID = username;
                oraBuilder.Password = ntlsCon.GetPassword();
                oraBuilder.DataSource = ntlsCon.GetServerDetails();

                _conString = oraBuilder.ConnectionString;

            }
        }

        public static INautilusProcessXML GetXmlProcessor(INautilusServiceProvider sp)
        {
            if (sp != null)
                return sp.QueryServiceProvider("ProcessXML") as NautilusProcessXML;
            else
                return null;
        }

        public static INautilusDBConnection GetNtlsCon(INautilusServiceProvider sp)
        {
            if (sp != null)

                return sp.QueryServiceProvider("DBConnection") as NautilusDBConnection;
            else
                return null;
        }

        public static NautilusUser GetNautilusUser(INautilusServiceProvider sp)
        {
            if (sp != null)

                return sp.QueryServiceProvider("User") as NautilusUser;
            else
                return null;
        }

        public static INautilusSchema GetSchema(INautilusServiceProvider sp)
        {
            if (sp != null)


                return sp.QueryServiceProvider("Schema") as INautilusSchema;
            else
                return null;
        }

        public static INautilusInternationalise GetInternationalise(INautilusServiceProvider sp)
        {
            if (sp != null)
                return sp.QueryServiceProvider("Internationalise") as LSSERVICEPROVIDERLib.INautilusInternationalise;
            return null;

        }

        public static INautilusPopupMenu GetPopupMenu(INautilusServiceProvider sp)
        {
            if (sp != null)
                return sp.QueryServiceProvider("PopupMenu") as INautilusPopupMenu;

            return null;

        }

        public static INautilusExplorer NautilusExplorer(INautilusServiceProvider sp)
        {
            if (sp != null)
                return sp.QueryServiceProvider("Explorer") as INautilusExplorer;

            return null;
        }

        public static string GetResourcePath()
        {
            try
            {
                string resourcePath = String.Empty;
                if (Environment.MachineName == "one1pc2619" || Environment.MachineName == "one1pc2123")
                    resourcePath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Thermo\Nautilus\9.4\Directory";
                else
                    resourcePath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Thermo\Nautilus\9.5\Directory";

                var path = (string)Registry.GetValue(resourcePath, "Resource", null);

                if (path != null)
                {
                    path += "\\";
                }
                return path;

            }
            catch (Exception ex)
            {

                return null;
            }
        }


    }


}