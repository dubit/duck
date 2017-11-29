using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	// How to use
	// First use one of Http static helper methods to create a HttpRequest. for example we will do a get request.
	// Http.Get("uri");
	// This will return a HttpRequest setup for Http Verb GET.
	// The next part is to actually send the HttpRequest, we do this by simply calling Send() on the HttpRequest.
	// Http.Get("uri").Send();
	// The Send method signature has the options for onSuccess and onError callbacks.
	// HttpRequest.Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null);
	// Both callbacks include the HttpResponse object.
	// You can get the Body of the response in 4 different forms: Text, Bytes, Texture or as an object parsed from Json.
	// Heres what it looks like parsed to an object:
	// Http.Get("uri").Send(response => { response.ParseBodyAs<User>(); });
	//
	// Alternatively you can use Http for sending UnityWebRequest with callbacks. For example:
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), (UnityWebRequest request) => { });
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onError: () => { });
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onNetworkError: () => { });
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

		private Dictionary<string, string> headers;

		private void Awake()
		{
			instance = this;

			headers = new Dictionary<string, string>();
		}

		public void SetSuperHeader(string key, string value)
		{
			headers[key] = value;
		}

		public Dictionary<string, string> GetSuperHeaders()
		{
			return headers;
		}

		public bool RemoveSuperHeader(string key)
		{
			return headers.Remove(key);
		}

		#region Creation Helpers

		/// <summary>
		///   <para>Creates a HttpRequest configured for HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>
		///   <para>A HttpRequest object configured to retrieve data from uri.</para>
		/// </returns>
		public static HttpRequest Get(string uri)
		{
			return new HttpRequest(UnityWebRequest.Get(uri));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="postData">Form body data. Will be URLEncoded via WWWTranscoder.URLEncode prior to transmission.</param>
		/// <returns>
		///   <para>A HttpRequest configured to send form data to uri via POST.</para>
		/// </returns>
		public static HttpRequest Post(string uri, string postData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, postData));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="json">Json body data.</param>
		/// <returns>
		///   <para>A HttpRequest configured to send json data to uri via POST.</para>
		/// </returns>
		public static HttpRequest PostAsJson(string uri, string json)
		{
			var unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
			{
				uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json))
				{
					contentType = "application/json"
				},
				downloadHandler = new DownloadHandlerBuffer()
			};
			return new HttpRequest(unityWebRequest);
		}

		public static HttpRequest PostAsJson<T>(string uri, T payload)
		{
			return PostAsJson(uri, JsonUtility.ToJson(payload));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
		/// <returns>
		///   <para>A HttpRequest configured to send form data to uri via POST.</para>
		/// </returns>
		public static HttpRequest Post(string uri, WWWForm formData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, formData));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields in the form of a Key Value Pair, for formatting and transmission to the remote server.</param>
		/// <returns>
		///   <para>A HttpRequest configured to send form data to uri via POST.</para>
		/// </returns>
		public static HttpRequest Post(string uri, Dictionary<string, string> formData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, formData));
		}

		public static HttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, multipartForm));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to upload raw data to a remote server via HTTP PUT.</para>
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.</param>
		/// <returns>
		///   <para>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</para>
		/// </returns>
		public static HttpRequest Put(string uri, byte[] bodyData)
		{
			return new HttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		/// <summary>
		///   <para>Create a HttpRequest configured to upload raw data to a remote server via HTTP PUT.</para>
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.
		/// The string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
		/// <returns>
		///   <para>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</para>
		/// </returns>
		public static HttpRequest Put(string uri, string bodyData)
		{
			return new HttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		/// <summary>
		///   <para>Creates a HttpRequest configured for HTTP DELETE.</para>
		/// </summary>
		/// <param name="uri">The URI to which a DELETE request should be sent.</param>
		/// <returns>
		///   <para>A HttpRequest configured to send an HTTP DELETE request.</para>
		/// </returns>
		public static HttpRequest Delete(string uri)
		{
			return new HttpRequest(UnityWebRequest.Delete(uri));
		}

		/// <summary>
		///   <para>Creates a HttpRequest configured to send a HTTP HEAD request.</para>
		/// </summary>
		/// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
		/// <returns>
		///   <para>A HttpRequest configured to transmit a HTTP HEAD request.</para>
		/// </returns>
		public static HttpRequest Head(string uri)
		{
			return new HttpRequest(UnityWebRequest.Head(uri));
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

		#region Send HttpRequest methods

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

		#region Send UnityWebRequest methods

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