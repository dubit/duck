using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public sealed class Http : MonoBehaviour
	{
		private static Http instance;

		public static Http Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GameObject("Http").AddComponent<Http>();
				}

				return instance;
			}
		}

		private void Awake()
		{
			instance = this;

			// Get settings. or if none exist set to default
		}

		public static HttpRequest Put(string url, byte[] rawData)
		{
			return new HttpRequest(UnityWebRequest.Put(url, rawData));
		}

		/// <summary>
		/// Creates a WebRequest configured for HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the resource to retrieve via HTTP GET.</param>
		/// <param name="onSuccess">The delegate for when the request was succesful.</param>
		/// <param name="onError">The delegate for when the request encountered an error.</param>
		public static HttpRequest Get(string url)
		{
			return new HttpRequest(UnityWebRequest.Get(url));
		}

		public static HttpRequest<T> Get<T>(string url)
		{
			return new HttpRequest<T>(UnityWebRequest.Get(url));
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
		public static HttpRequest PostAsJson(string url, string json)
		{
			var unityWebRequest = UnityWebRequest.Post(url, json);
			var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			unityWebRequest.uploadHandler = uploadHandler;
			return new HttpRequest(unityWebRequest);
		}

		public static HttpRequest Delete(string url)
		{
			return new HttpRequest(UnityWebRequest.Delete(url));
		}

		public void Send(UnityWebRequest webRequest, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(webRequest, onSuccess, onError));
		}

		public void Send<T>(UnityWebRequest webRequest, Action<HttpResponse<T>> onSuccess = null, Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(webRequest, onSuccess, onError));
		}

		private static IEnumerator SendCoroutine<T>(UnityWebRequest webRequest, Action<HttpResponse<T>> onSuccess = null,
			Action<HttpResponse> onError = null)
		{
			yield return webRequest.SendWebRequest();

			var responseCode = webRequest.responseCode;
			var responseType = WebServiceUtils.GetResponseType(responseCode);
			var message = WebServiceUtils.GetResponseTypeMessage(responseType, responseCode, webRequest.url);

			if (responseType == ResponseType.Successful)
			{
				var body = WebServiceUtils.TryParse<T>(webRequest.downloadHandler.text);
				var response = new HttpResponse<T>
				{
					ResponseCode = webRequest.responseCode,
					Message = body != null ? message : message + " - Could not parse body",
					Body = body
				};

				if (onSuccess != null)
				{
					onSuccess.Invoke(response);
				}
			}
			else if (webRequest.isHttpError || webRequest.isNetworkError)
			{
				var response = new HttpResponse
				{
					ResponseCode = webRequest.responseCode,
					Message = message
				};
				if (onError != null)
				{
					onError.Invoke(response);
				}
			}
		}

		private static IEnumerator SendCoroutine(UnityWebRequest webRequest, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null)
		{
			yield return webRequest.SendWebRequest();

			var responseCode = webRequest.responseCode;
			var responseType = WebServiceUtils.GetResponseType(responseCode);
			var message = WebServiceUtils.GetResponseTypeMessage(responseType, responseCode, webRequest.url);

			var response = new HttpResponse
			{
				ResponseCode = webRequest.responseCode,
				Message = message
			};

			if (webRequest.isHttpError || webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError.Invoke(response);
				}
			}
			else
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke(response);
				}
			}
		}
	}
}