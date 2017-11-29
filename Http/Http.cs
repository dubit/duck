using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public sealed partial class Http : MonoBehaviour
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
		}

		#region Creation Helpers

		public static HttpRequest Get(string url)
		{
			return new HttpRequest(UnityWebRequest.Get(url));
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

		public static HttpRequest Post<T>(string url, T payload)
		{
			return Post(url, JsonUtility.ToJson(payload));
		}

		#endregion

		#region None Static Send Methods

		public void Send(HttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(request, onSuccess, onError));
		}

		public void Send(UnityWebRequest unityWebRequest, Action<UnityWebRequest> onSuccess = null,
			Action<UnityWebRequest> onError = null, Action<UnityWebRequest> onNetworkError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		public void Send(UnityWebRequest unityWebRequest, Action onSuccess = null,
			Action onError = null, Action onNetworkError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		public void Send(UnityWebRequest unityWebRequest, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError));
		}

		#endregion

		#region Http Request

		private static IEnumerator SendCoroutine(HttpRequest request, Action<HttpResponse> onSuccess,
			Action<HttpResponse> onError)
		{
			var unityWebRequest = request.UnityWebRequest;
			yield return unityWebRequest.SendWebRequest();
			var response = new HttpResponse(unityWebRequest);

			if (onError != null && (unityWebRequest.isNetworkError || unityWebRequest.isHttpError))
			{
				onError.Invoke(response);
			}
			else if (onSuccess != null)
			{
				onSuccess.Invoke(response);
			}
		}

		#endregion

		#region Unity Web Request

		private static IEnumerator SendCoroutine(UnityWebRequest unityWebRequest,
			Action onSuccess = null, Action onError = null, Action onNetworkError = null)
		{
			yield return unityWebRequest.SendWebRequest();
			if (unityWebRequest.isNetworkError)
			{
				if (onNetworkError != null)
				{
					onNetworkError.Invoke();
				}
			}
			else if (unityWebRequest.isHttpError)
			{
				if (onError != null)
				{
					onError.Invoke();
				}
			}
			else
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke();
				}
			}
		}

		private static IEnumerator SendCoroutine(UnityWebRequest unityWebRequest,
			Action<UnityWebRequest> onSuccess = null,
			Action<UnityWebRequest> onError = null, Action<UnityWebRequest> onNetworkError = null)
		{
			yield return unityWebRequest.SendWebRequest();
			if (unityWebRequest.isNetworkError)
			{
				if (onNetworkError != null)
				{
					onNetworkError.Invoke(unityWebRequest);
				}
			}
			else if (unityWebRequest.isHttpError)
			{
				if (onError != null)
				{
					onError.Invoke(unityWebRequest);
				}
			}
			else
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke(unityWebRequest);
				}
			}
		}

		private static IEnumerator SendCoroutine(UnityWebRequest request,
			Action<HttpResponse> onSuccess,
			Action<HttpResponse> onError)
		{
			yield return request.SendWebRequest();
			var response = new HttpResponse(request);

			if (onError != null && (request.isNetworkError || request.isHttpError))
			{
				onError.Invoke(response);
			}
			else if (onSuccess != null)
			{
				onSuccess.Invoke(response);
			}
		}

		#endregion
	}
}