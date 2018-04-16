using System;
using System.Security.Cryptography;
using System.Text;

namespace DUCK.Crypto
{
	public static class SimpleAESEncryption
	{
		public struct AESEncryptedText
		{
			public string IV;
			public string EncryptedText;
		}

		public static AESEncryptedText Encrypt(string plainText, string password)
		{
			using (Aes aes = Aes.Create())
			{
				aes.GenerateIV();
				aes.Key = ConvertToKeyBytes(aes, password);

				var textBytes = Encoding.UTF8.GetBytes(plainText);

				var aesEncryptor = aes.CreateEncryptor();
				var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

				return new AESEncryptedText
				{
					IV = Convert.ToBase64String(aes.IV),
					EncryptedText = Convert.ToBase64String(encryptedBytes)
				};
			}
		}

		public static string Decrypt(AESEncryptedText encryptedText, string password)
		{
			return Decrypt(encryptedText.EncryptedText, encryptedText.IV, password);
		}

		public static string Decrypt(string encryptedText, string iv, string password)
		{
			using (Aes aes = Aes.Create())
			{
				var ivBytes = Convert.FromBase64String(iv);
				var encryptedTextBytes = Convert.FromBase64String(encryptedText);

				var decryptor = aes.CreateDecryptor(ConvertToKeyBytes(aes, password), ivBytes);
				var decryptedBytes = decryptor.TransformFinalBlock(encryptedTextBytes, 0, encryptedTextBytes.Length);

				return Encoding.UTF8.GetString(decryptedBytes);
			}
		}

		// Ensure the AES key byte-array is the right size - AES will reject it otherwise
		private static byte[] ConvertToKeyBytes(SymmetricAlgorithm algorithm, string password)
		{
			algorithm.GenerateKey();

			var keyBytes = Encoding.UTF8.GetBytes(password);
			var validKeySize = algorithm.Key.Length;

			if (keyBytes.Length != validKeySize)
			{
				var newKeyBytes = new byte[validKeySize];
				Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
				keyBytes = newKeyBytes;
			}

			return keyBytes;
		}
	}
}

