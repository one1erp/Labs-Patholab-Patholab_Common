using System;

namespace Patholab_Common
{
    public  static class   NautilsuBoolean
    {
        public static bool ConvertToBoolean(this char value)
        {      
            switch (value)
            {
                case 'F':
                    return false;
                case 'T':
                    return true;
                default:
                    throw new Exception(); 

            }
        
        }
    
        public static string ConvertBack(bool value)
        {
            return  value?
                             "T" : "F";       

        }
    }
}