using System;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest<TSuccess, TError> where TSuccess : class where TError : class
	{
		public UnityWebRequest UnityWebRequest { get; private set; }
		public MarkUpType MarkUpType { get; private set; }

		public HttpRequest(UnityWebRequest unityWebRequest, MarkUpType markUpType)
		{
			UnityWebRequest = unityWebRequest;
			MarkUpType = markUpType;
		}

		public void SetHeader(string name, string value)
		{
			UnityWebRequest.SetRequestHeader(name, value);
		}

		public void Send(Action<HttpResponse, TSuccess> onSuccess = null, Action<HttpResponse, TError> onError = null)
		{
			Http.Instance.Send(this, onSuccess, onError);
		}

		public void Send(Action<TSuccess> onSuccess = null, Action<TError> onError = null)
		{
			Http.Instance.Send(this, onSuccess, onError);
		}
	}
}