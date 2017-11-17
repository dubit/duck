using System;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest<T>
	{
		protected readonly UnityWebRequest unityWebRequest;

		public HttpRequest(UnityWebRequest unityWebRequest)
		{
			this.unityWebRequest = unityWebRequest;
		}

		public void AddHeader(string name, string value)
		{
			unityWebRequest.SetRequestHeader(name, value);
		}

		public void Send(Action<HttpResponse<T>> onSuccess = null, Action<HttpResponse> onError = null)
		{
			Http.Instance.Send(unityWebRequest, onSuccess, onError);
		}
	}

	public class HttpRequest : HttpRequest<object>
	{
		public HttpRequest(UnityWebRequest unityWebRequest) : base(unityWebRequest)
		{
		}

		public void Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			Http.Instance.Send(unityWebRequest, onSuccess, onError);
		}
	}
}