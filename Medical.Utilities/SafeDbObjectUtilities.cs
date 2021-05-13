using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Utilities
{
    public static class SafeDbObjectUtilities
    {
        public static object SafeDbObject(object input)
        {
            if (input == null)
            {
                return DBNull.Value;
            }
            else
            {
                return input;
            }
        }
    }
}
