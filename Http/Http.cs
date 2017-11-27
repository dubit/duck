using System;
using System.Collections;
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

		public void Send(HttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(request, onSuccess, onError));
		}

		public void Send<TSuccess, TError>(HttpRequest<TSuccess, TError> request, Action<TSuccess> onSuccess = null,
			Action<TError> onError = null) where TSuccess : class where TError : class
		{
			StartCoroutine(SendCoroutine(request, (response, success) =>
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke(success);
				}
			}, (response, error) =>
			{
				if (onError != null)
				{
					onError.Invoke(error);
				}
			}));
		}

		/// <summary>
		/// Send a HttpRequest with callbacks for onSuccess and onError with templatized
		/// </summary>
		public void Send<TSuccess, TError>(HttpRequest<TSuccess, TError> request, Action<HttpResponse, TSuccess> onSuccess,
			Action<HttpResponse, TError> onError) where TSuccess : class where TError : class
		{
			StartCoroutine(SendCoroutine(request, onSuccess, onError));
		}

		/// <summary>
		/// Send a unity web request with callbacks for onSuccess, onError and onNetworkError
		/// </summary>
		public void Send(UnityWebRequest unityWebRequest, Action<UnityWebRequest> onSuccess = null,
			Action<UnityWebRequest> onError = null, Action<UnityWebRequest> onNetworkError = null)
		{
			StartCoroutine(SendBasicCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		public void Send(UnityWebRequest unityWebRequest, Action onSuccess = null,
			Action onError = null, Action onNetworkError = null)
		{
			StartCoroutine(SendBasicCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		/// <summary>
		/// Send a Unity web request with callbacks for onSuccess and onError with an HttpResponse
		/// </summary>
		public void Send(UnityWebRequest unityWebRequest, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendBasicCoroutine(unityWebRequest, onSuccess, onError));
		}

		private static IEnumerator SendBasicCoroutine(UnityWebRequest unityWebRequest,
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

		private static IEnumerator SendBasicCoroutine(UnityWebRequest unityWebRequest,
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

		private static IEnumerator SendBasicCoroutine(UnityWebRequest request,
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

		private static IEnumerator SendCoroutine<TSuccess, TError>(HttpRequest<TSuccess, TError> request,
			Action<HttpResponse, TSuccess> onSuccess,
			Action<HttpResponse, TError> onError) where TSuccess : class where TError : class
		{
			var unityWebRequest = request.UnityWebRequest;
			yield return unityWebRequest.SendWebRequest();
			var response = new HttpResponse(request.UnityWebRequest);

			Debug.Log(HttpUtils.GetResponseTypeMessage(response, unityWebRequest.url));

			if (onError != null && (unityWebRequest.isNetworkError || unityWebRequest.isHttpError))
			{
				var error = HttpUtils.TryParse<TError>(unityWebRequest.downloadHandler.text, request.MarkUpType);
				onError.Invoke(response, error);
			}
			else if (onSuccess != null)
			{
				if (typeof(TSuccess) == typeof(Texture2D))
				{
					var downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
					if (downloadHandlerTexture != null && downloadHandlerTexture.texture != null)
					{
						onSuccess.Invoke(response, downloadHandlerTexture.texture as TSuccess);
					}
					else
						throw new NullReferenceException("Http was successful with code " + response.ResponseCode +
						                                 " but could not cast to Texture2D");
				}
				else
				{
					var success = HttpUtils.TryParse<TSuccess>(request.UnityWebRequest.downloadHandler.text, request.MarkUpType);
					onSuccess.Invoke(response, success);
				}
			}
		}
	}
}