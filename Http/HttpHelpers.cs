﻿using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public static class HttpHelpers
	{
		/// <summary>
		/// Create a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="json">Json body data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
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

		/// <summary>
		/// Create a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="payload">The object to be parsed to json data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
		public static HttpRequest PostAsJson<T>(string uri, T payload) where T : class
		{
			return PostAsJson(uri, JsonUtility.ToJson(payload));
		}
	}
}
