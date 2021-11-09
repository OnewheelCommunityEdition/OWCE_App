using System;
using CoreBluetooth;

namespace OWCE.MacOS.Extensions
{
    public static class GuidExtension
    {
        public static CBUUID ToCBUUID(this Guid guid)
        {
            var bytes = guid.ToByteArray();
            return CBUUID.FromString(guid.ToString());
            //return CBUUID.FromBytes(guid.ToByteArray());
        }
    }
}
