using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public partial class Http
	{

		/// <summary>
		/// Basic Get Request
		/// </summary>
		/// <param name="url">The url</param>
		/// <returns>A HttpRequest with Http Verb Get</returns>
		public static HttpRequest Get(string url)
		{
			return new HttpRequest(UnityWebRequest.Get(url));
		}

		public static HttpRequest<TSuccess, TError> Get<TSuccess, TError>(string url, MarkUpType markUpType = MarkUpType.Json)
			where TSuccess : class where TError : class
		{
			return new HttpRequest<TSuccess, TError>(UnityWebRequest.Get(url), markUpType);
		}

		public static HttpRequest Post(string url, string payload)
		{
			return new HttpRequest(UnityWebRequest.Post(url, payload));
		}

		public static HttpRequest Post(string url, WWWForm form)
		{
			return new HttpRequest(UnityWebRequest.Post(url, form));
		}

		public static HttpRequest Post(string url, Dictionary<string, string> formFields)
		{
			return new HttpRequest(UnityWebRequest.Post(url, formFields));
		}

		public static HttpRequest Post(string url, List<IMultipartFormSection> multipartForm)
		{
			return new HttpRequest(UnityWebRequest.Post(url, multipartForm));
		}

		public static HttpRequest<TSuccess, TError> Post<TSuccess, TError, TSend>(string url, TSend payload,
			MarkUpType markUpType = MarkUpType.Json) where TSuccess : class where TError : class
		{
			// TODO parse to other markup types
			return Post<TSuccess, TError>(url, JsonUtility.ToJson(payload));
		}

		public static HttpRequest<TSuccess, TError> Post<TSuccess, TError>(string url, string payload,
			MarkUpType markUpType = MarkUpType.Json) where TSuccess : class where TError : class
		{
			var unityWebRequest = UnityWebRequest.Post(url, payload);
			var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
			if (markUpType == MarkUpType.Json)
			{
				unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			}
			unityWebRequest.uploadHandler = uploadHandler;
			return new HttpRequest<TSuccess, TError>(unityWebRequest, markUpType);
		}

		public static HttpRequest<TSuccess, TError> Post<TSuccess, TError>(string url, WWWForm formData,
			MarkUpType markUpType = MarkUpType.Json) where TSuccess : class where TError : class
		{
			return new HttpRequest<TSuccess, TError>(UnityWebRequest.Post(url, formData), markUpType);
		}

		public static HttpRequest<TSuccess, TError> Post<TSuccess, TError>(string url,
			List<IMultipartFormSection> multipartFormSections,
			MarkUpType markUpType = MarkUpType.Json) where TSuccess : class where TError : class
		{
			return new HttpRequest<TSuccess, TError>(UnityWebRequest.Post(url, multipartFormSections), markUpType);
		}

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