using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.IO;
using System.Security.AccessControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace FixEaglesoftServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

			textBox1.Text = " -ek " + GenerateEcryptionKey() + " ";
			textBox2.Text = " -ek " + CreateEncryptionKeysBackup() + " "; 
			textBox3.Text = " -ek " + GetCurrentEncryptionKey() + " ";
		}

		private string GenerateEcryptionKey(bool primary = true)
		{
			string text = "";
			try
			{
				string baseCode = "PattersonPM.db Encryption";
				string currentEncryptionKeyGenHelper = "";
				string hklmRegistryValue = GetHklmRegistryValue("SOFTWARE\\Eaglesoft", "LicenseNumber");
                //SetEncryptionLicense(hklmRegistryValue, primary);
                MessageBox.Show(hklmRegistryValue);
                text = GetHash(baseCode + currentEncryptionKeyGenHelper + hklmRegistryValue);
                MessageBox.Show(text);
				text = ((text.Length < 32) ? text.PadLeft(32, '0') : text.Substring(0, 32));
                MessageBox.Show(text);
            }
			catch (Exception)
			{
				text = "";
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "P@tterson";
			}
			//SetEncryptionKey(text, primary);
			return text;
		}

		public static string GetHklmRegistryValue(string string_0, string string_1)
		{
			string result = "";
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string_0);
				if (registryKey != null)
				{
					return registryKey.GetValue(string_1)?.ToString();
				}
				return result;
			}
			catch (Exception ex)
			{
				//XtraMessageBox.Show(ex.Message);
				throw;
			}
		}

		public static string GetHash(string targetToHash)
		{
			SHA256Managed sHA256Managed = new SHA256Managed();
			byte[] bytes = Encoding.UTF8.GetBytes(targetToHash);
			return smethod_32(sHA256Managed.ComputeHash(bytes));
		}

		static string smethod_32(byte[] byte_0)
		{
			StringBuilder stringBuilder = new StringBuilder(byte_0.Length * 2);
			foreach (byte b in byte_0)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}


		private static byte[] byte_0 = new byte[32]
		{
			31, 34, 254, 91, 234, 137, 180, 33, 199, 13,
			111, 213, 195, 246, 131, 12, 239, 3, 130, 43,
			153, 178, 200, 179, 225, 137, 190, 18, 83, 165,
			238, 61
		};

		private static byte[] byte_1 = new byte[16]
		{
		239, 210, 209, 60, 181, 97, 208, 135, 127, 254,
		15, 92, 81, 216, 40, 203
		};

		public static string Decrypt(string value)
		{
			string result = value;
			try
			{
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				byte[] array = Convert.FromBase64String(value);
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(byte_0, byte_1);
				MemoryStream stream = new MemoryStream(array);
				CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				byte[] array2 = new byte[array.Length];
				cryptoStream.Read(array2, 0, array2.Length);
				char[] trimChars = new char[1];
				result = aSCIIEncoding.GetString(array2).Trim(trimChars);
				return result;
			}
			catch (Exception ex)
			{
				
				return result;
			}
		}


		private string CreateEncryptionKeysBackup()
		{
			string path_to_data = GetHKLMValue32("HKEY_LOCAL_MACHINE\\Software\\EagleSoft\\Paths", "Data", (object)"C:\\Eaglesoft\\Data\\").ToString();

			string encryptedfile = File.ReadAllText(path_to_data + "keybackup.data");

			string decryptedbytes = Decrypt(encryptedfile);

			byte[] decryptedbytes_default = Encoding.Default.GetBytes(decryptedbytes);

			//Making sure we are UTF-8
			string decryptedbytes_utf8 = Encoding.UTF8.GetString(decryptedbytes_default);

			//Running this to fix string characters with JSON stuff like ^ \/ //
			dynamic jsonResponse = JsonConvert.DeserializeObject(decryptedbytes_utf8);

			//Search for string!!
			String License = getBetween(JsonConvert.SerializeObject(jsonResponse), "DbEncryptLicensePrimary\":\"", "\",");

			string encryptionkey = "";
			try
			{
				string baseCode = "PattersonPM.db Encryption";
				string currentEncryptionKeyGenHelper = "";
				//SetEncryptionLicense(hklmRegistryValue, primary);
				encryptionkey = GetHash(baseCode + currentEncryptionKeyGenHelper + License);
				encryptionkey = ((encryptionkey.Length < 32) ? encryptionkey.PadLeft(32, '0') : encryptionkey.Substring(0, 32));
			}
			catch (Exception)
			{
				encryptionkey = "";
			}
			if (string.IsNullOrEmpty(encryptionkey))
			{
				encryptionkey = "P@tterson";
			}
			return encryptionkey;
		}

		public static string getBetween(string strSource, string strStart, string strEnd)
		{
			int Start, End;
			if (strSource.Contains(strStart) && strSource.Contains(strEnd))
			{
				Start = strSource.IndexOf(strStart, 0) + strStart.Length;
				End = strSource.IndexOf(strEnd, Start);
				return strSource.Substring(Start, End - Start);
			}
			else
			{
				return "";
			}
		}

		public static object GetHKLMValue32(string key, string item, object defaultValue)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			key = key.Replace("HKEY_LOCAL_MACHINE", "").TrimStart('\\').Trim();
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(key, RegistryKeyPermissionCheck.Default, RegistryRights.ExecuteKey))
				{
					if (registryKey2 == null)
					{
						return defaultValue;
					}
					return registryKey2.GetValue(item, defaultValue);
				}
			}
		}


		private string GetCurrentEncryptionKey()
		{
			string path_to_data = GetHKLMValue32("HKEY_LOCAL_MACHINE\\Software\\EagleSoft\\Paths", "Data", (object)"C:\\Eaglesoft\\Data\\").ToString();

			string encryptedfile = File.ReadAllText(path_to_data + "keybackup.data");

			string decryptedbytes = Decrypt(encryptedfile);

			byte[] decryptedbytes_default = Encoding.Default.GetBytes(decryptedbytes);

			//Making sure we are UTF-8
			string decryptedbytes_utf8 = Encoding.UTF8.GetString(decryptedbytes_default);

			//Running this to fix string characters with JSON stuff like ^ \/ //
			dynamic jsonResponse = JsonConvert.DeserializeObject(decryptedbytes_utf8);

			//Search for string!!
			String License = getBetween(JsonConvert.SerializeObject(jsonResponse), "DbEncryptKeyPrimary\":\"", "\",");

			if (!string.IsNullOrEmpty(License) && License.IndexOfAny("!@#$%^&*()+_-=~`HhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz:;'<>,.?".ToCharArray(), 0) > -1)
			{
				License = Decrypt(License);
			}
			return License;
		}

	}
}


