using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class clsCommon
    {
        public static string SerializeToJsonString(object objectToSerialize)
        {
            // Create a stream to serialize the object to.
            using (var ms = new MemoryStream())
            {
                // Serializer the User object to the stream.
                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(objectToSerialize.GetType());
                ser.WriteObject(ms, objectToSerialize);
                byte[] json = ms.ToArray();
                ms.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public static string GetSASToken(string resourceUri, string keyName, string key, TimeSpan ttl)
        {
            var expiry = GetExpiry(ttl);
            //string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            //NOTE: UrlEncode is not supported in CRM, use System.Uri.EscapeDataString instead
            string stringToSign = Uri.EscapeDataString(resourceUri).ToLowerInvariant() + "\n"
                 + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

            var signature =
            Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", Uri.EscapeDataString(resourceUri).ToLowerInvariant(),
       Uri.EscapeDataString(signature), expiry, keyName);
            return sasToken;
        }

        public static string GetExpiry(TimeSpan ttl)
        {
            var expirySinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1) + ttl;
            return Convert.ToString((int)expirySinceEpoch.TotalSeconds);
        }
    }
}
