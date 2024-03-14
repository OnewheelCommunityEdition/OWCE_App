using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace OWCE
{
    public class BoardSecureStorage
    {
        public static Task<string> GetBoardKeyAsync(string board)
            => GetAsync(board, "key");

        public static Task SetBoardKeyAsync(string board, string value)
            => SetAsync(board, "key", value);

        public static bool RemoveBoardKey(string board)
            => Remove(board, "key");
        public static Task<string> GetBoardTokenAsync(string board)
            => GetAsync(board, "token");
        public static Task<bool> HasBoardTokenAsync(string board)
            => GetAsync(board, "token").ContinueWith(token => !String.IsNullOrEmpty(token.Result));

        public static Task SetBoardTokenAsync(string board, string value)
            => SetAsync(board, "token", value);

        public static bool RemoveBoardToken(string board)
            => Remove(board, "token");

        private static Task<string> GetAsync(string board, string entry)
            => SecureStorage.GetAsync(EntryKey(board, entry));

        private static Task SetAsync(string board, string entry, string value)
            => SecureStorage.SetAsync(EntryKey(board, entry), value);

        private static bool Remove(string board, string entry)
            => SecureStorage.Remove(EntryKey(board, entry));

        private static string EntryKey(string board, string entry)
        {
            if (string.IsNullOrWhiteSpace(board))
                throw new ArgumentNullException(nameof(board));

            string deviceName = board.ToLower();
            string deviceId = deviceName.Replace("ow", String.Empty);

            return $"board_{deviceId}_{entry}";
        }
    }
}
