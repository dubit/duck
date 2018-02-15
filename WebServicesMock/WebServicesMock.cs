using DUCK.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.WebServicesMock
{
	public enum HTTPMethod
	{
		GET,
		POST,
		PUT,
		DELETE
	}

	public class MockWebResponse
	{
		public string url;
		public long statusCode;
		public string body;
		public string error;
		public object payload;
	}

	public class MockWebRequest
	{
		public Action<MockWebResponse> onComplete;

		private WebServicesMock.CallWebServiceDelegate response;
		private object payload;

		public MockWebRequest(WebServicesMock.CallWebServiceDelegate response, object payload)
		{
			this.response = response;
			this.payload = payload;
		}

		public void Send()
		{
			MonoBehaviourService.Instance.StartCoroutine(Call());
		}

		private IEnumerator Call()
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(WebServicesMock.Instance.MinLatency, WebServicesMock.Instance.MaxLatency));
			var responseObject = response(payload);
			onComplete.SafeInvoke(responseObject);
		}
	}

	public class WebServicesMock : GenericSingleton<WebServicesMock>
	{
		public delegate MockWebResponse CallWebServiceDelegate(object data);

		public float MinLatency { get { return minLatency; } }
		public float MaxLatency { get { return maxLatency; } }

		private float minLatency = 0.1f;
		private float maxLatency = 0.5f;

		private Dictionary<string, Dictionary<HTTPMethod, CallWebServiceDelegate>> api;

		private WebServicesMock()
		{
			api = new Dictionary<string, Dictionary<HTTPMethod, CallWebServiceDelegate>>();
		}

		public void SetLatency(float minLatency, float maxLatency)
		{
			if (minLatency <= 0) throw new Exception("Minimum latency must be above 0");
			this.minLatency = minLatency;

			if (maxLatency < minLatency) throw new Exception("Maximum latency must be above Minimum Latency");
			this.maxLatency = maxLatency;
		}

		/// <summary>
		/// Adds a new service to the mocked api.
		/// </summary>
		/// <param name="url">The path of this service</param>
		/// <param name="method">The HTTP method it has a repsonse to</param>
		/// <param name="response">The response it will create when called</param>
		public void Add(string url, HTTPMethod method, CallWebServiceDelegate response)
		{
			if (!api.ContainsKey(url))
			{
				api.Add(url, new Dictionary<HTTPMethod, CallWebServiceDelegate>());
			}

			if(api[url].ContainsKey(method))
			{
				throw new Exception("API already defined: " + url + " " + method);
			}

			api[url].Add(method, response);
		}

		/// <summary>
		/// Removes a service method
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		public void Remove(string url, HTTPMethod method)
		{
			if (api.ContainsKey(url) && api[url].ContainsKey(method))
			{
				api[url].Remove(method);
				return;
			}

			throw new Exception("No url and method found to remove");
		}

		/// <summary>
		/// Removes a service
		/// </summary>
		/// <param name="url"></param>
		public void Remove(string url)
		{
			if (api.ContainsKey(url))
			{
				api.Remove(url);
				return;
			}

			throw new Exception("No service found to remove");
		}

		/// <summary>
		/// Sends a fake web request to the mocked api. A 404 error will occur if service does not exist
		/// </summary>
		/// <param name="url">The path to the service</param>
		/// <param name="method">The http method</param>
		/// <param name="payload">The data you wish to send to the service</param>
		/// <param name="onComplete">Called once the web request has completed</param>
		public void SendRequest(string url, HTTPMethod method, object payload = null, Action<MockWebResponse> onComplete = null)
		{
			MockWebRequest webRequest = null;

			if (api.ContainsKey(url))
			{
				if (api[url].ContainsKey(method))
				{
					webRequest = webRequest = new MockWebRequest(api[url][method], payload);
				}
			}

			if (webRequest == null)
			{
				webRequest = new MockWebRequest((data) => new MockWebResponse()
				{
					body = "{}",
					error = "404 Not Found",
					statusCode = 404,
					url = url,
					payload = null
				}, payload);
			}

			webRequest.onComplete = onComplete;
			webRequest.Send();
		}
	}
}
