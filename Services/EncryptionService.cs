using System.Security.Cryptography;
using System.Text;

namespace ChildCareConnect.Services;

public class EncryptionService
{
    private static readonly string EncryptionKey = "ChildCareConnect2024SecureKey!!"; // 32 bytes for AES256

    public static string EncryptSSN(string ssn)
    {
        if (string.IsNullOrEmpty(ssn))
            return "";

        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(ssn);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        catch
        {
            return ssn; // Fallback to plain text if encryption fails
        }
    }

    public static string DecryptSSN(string encryptedSSN)
    {
        if (string.IsNullOrEmpty(encryptedSSN))
            return "";

        try
        {
            var buffer = Convert.FromBase64String(encryptedSSN);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var iv = new byte[aes.IV.Length];
                Array.Copy(buffer, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            return ""; // Return empty if decryption fails
        }
    }

    public static string FormatPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return "";

        // Remove all non-digit characters
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        // Format as (XXX) XXX-XXXX if we have 10 digits
        if (digits.Length == 10)
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";

        return phone; // Return original if not 10 digits
    }

    public static string UnformatPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return "";

        return new string(phone.Where(char.IsDigit).ToArray());
    }
}
