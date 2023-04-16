using System;
using System.Buffers;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
	public class ByteToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Google.Protobuf.ByteString byteString)
            {
                //return byteString.ToString();
                
                byte[] byteArray = ArrayPool<byte>.Shared.Rent(byteString.Length);
                try
                {
                    byteString.CopyTo(byteArray, 0);
                    return BitConverter.ToString(byteArray, 0, byteString.Length);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteArray);
                }
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

