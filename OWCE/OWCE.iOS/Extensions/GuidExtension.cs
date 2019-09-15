using System;
using CoreBluetooth;

#if __IOS__
namespace OWCE.iOS.Extensions
#elif __MACOS__
namespace OWCE.MacOS.Extensions
#endif
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
