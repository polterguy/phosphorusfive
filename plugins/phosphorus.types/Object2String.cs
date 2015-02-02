
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// helper class for converting from object to string representation
    /// </summary>
    internal static class Object2String
    {
        private static ApplicationContext _context;

        public static string ToString (Node value)
        {
            Node tmp = new Node ();
            tmp.Add (value.Clone ());
            if (_context == null)
                _context = Loader.Instance.CreateApplicationContext ();
            _context.Raise ("lambda2code", tmp);
            return tmp.Get<string> ();
        }

        public static string ToString (Boolean value)
        {
            return value.ToString ().ToLower ();
        }
        
        public static string ToString (Byte[] value)
        {
            return Convert.ToBase64String (value);
        }

        public static string ToString (DateTime value)
        {
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0 && value.Millisecond == 0)
                return value.ToString ("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (value.Millisecond == 0)
                return value.ToString ("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            else
                return value.ToString ("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
        
        public static string ToString (TimeSpan value)
        {
            return value.ToString ("c", CultureInfo.InvariantCulture);
        }
    }
}
