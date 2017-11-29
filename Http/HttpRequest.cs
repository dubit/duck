using System;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest
	{
		public UnityWebRequest UnityWebRequest { get; private set; }

		public HttpRequest(UnityWebRequest unityWebRequest)
		{
			UnityWebRequest = unityWebRequest;
		}

		public void SetHeader(string name, string value)
		{
			UnityWebRequest.SetRequestHeader(name, value);
		}

		public void Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			Http.Instance.Send(this, onSuccess, onError);
		}
	}
}