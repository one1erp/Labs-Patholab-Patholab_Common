
using System;
using System.Windows.Forms;

namespace Patholab_Common
{
    public static class zLang
    {

        public static void Hebrew()
        {

            try
            {
                const string myLanguage = "HE-IL";
                InputLanguage.CurrentInputLanguage =
                   InputLanguage.FromCulture(new System.Globalization.CultureInfo(myLanguage));

            }
            catch (Exception e)
            {

                Logger.WriteLogFile(e);
            }
        }

        public static void English()
        {
            try
            {


                const string myLanguage = "EN";
                InputLanguage.CurrentInputLanguage =
                   InputLanguage.FromCulture(new System.Globalization.CultureInfo(myLanguage));
            }
            catch (Exception e)
            {

                Logger.WriteLogFile(e);
            }
        }
    }
}
