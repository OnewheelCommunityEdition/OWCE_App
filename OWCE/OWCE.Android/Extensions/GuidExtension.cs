using System;
using Android.OS;
using Java.Util;

namespace OWCE.Droid.Extensions
{
    public static class GuidExtension
    {
        public static ParcelUuid ToParcelUuid(this Guid guid)
        {
            return ParcelUuid.FromString(guid.ToString());
        }

        public static UUID ToUUID(this Guid guid)
        {
            return UUID.FromString(guid.ToString());
        }


        public static Guid ToGuid(this ParcelUuid parcelUuid)
        {
            return parcelUuid.Uuid.ToGuid();
        }

        public static Guid ToGuid(this UUID uuid)
        {
            return Guid.Parse(uuid.ToString());
        }
    }
}
