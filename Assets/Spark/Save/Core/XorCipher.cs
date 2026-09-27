using System.Text;

namespace Spark.Save.Core
{
    internal static class XorCipher
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("sad");

        public static byte[] Apply(byte[] data)
        {
            var result = new byte[data.Length];
            for (var i = 0; i < data.Length; i++) 
                result[i] = (byte)(data[i] ^ Key[i % Key.Length]);

            return result;
        }
    }
}