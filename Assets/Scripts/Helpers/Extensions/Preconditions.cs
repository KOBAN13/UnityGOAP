using System;
using Object = UnityEngine.Object;

namespace Customs
{
    public class Preconditions
    {
        public static T CheckNotNull<T>(T reference)
        {
            return CheckNotNull(reference, "Default message");
        }

        public static T CheckNotNull<T>(T reference, string message)
        {
            if (reference is Object obj && obj ? obj : null) throw new ArgumentException(message);
            if (reference == null) throw new AggregateException(message);
            return reference;
        }

        public static void CheckValidateData(float data)
        {
            if (data < 0) throw new FormatException("Format Exception");
        }
        
        public static void CheckValidateData(int data)
        {
            if (data < 0) throw new FormatException("Format Exception");
        }
    }
}