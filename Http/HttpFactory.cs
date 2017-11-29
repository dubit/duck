using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public partial class Http
	{
/*
		public static HttpRequest Put(string url, byte[] rawData)
		{
			return new HttpRequest(UnityWebRequest.Put(url, rawData));
		}

		/// <summary>
		/// Creates a WebRequest configured for HTTP POST.
		/// </summary>
		/// <param name="url">The target URI to which form data will be transmitted.</param>
		/// <param name="payload">The payload to be converted to json and transmitted</param>
		/// <param name="onSuccess">The delegate for when the request was succesful.</param>
		/// <param name="onError">The delegate for when the request encountered an error.</param>
		public static HttpRequest PostAsJson(string url, object payload)
		{
			return PostAsJson(url, JsonUtility.ToJson(payload));
		}

		/// <summary>
		/// Creates a WebRequest configured for HTTP POST.
		/// </summary>
		/// <param name="url">The target URI to which form data will be transmitted.</param>
		/// <param name="json">The payload in json to be transmitted</param>
		/// <param name="onSuccess">The delegate for when the request was succesful.</param>
		/// <param name="onError">The delegate for when the request encountered an error.</param>
		public static HttpRequest Post(string url, string payload)
		{
			var unityWebRequest = UnityWebRequest.Post(url, payload);
			var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			unityWebRequest.uploadHandler = uploadHandler;
			return new HttpRequest(unityWebRequest);
		}

		public static HttpRequest Delete(string url)
		{
			return new HttpRequest(UnityWebRequest.Delete(url));
		}

*/
	}
}