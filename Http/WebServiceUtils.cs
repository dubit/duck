using System.Collections.Generic;
using UnityEngine;

namespace DUCK.Http
{
	/// <summary>
	/// A utility class for webserivce, deals with response codes and error messages.
	/// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status"/>
	/// </summary>
	public static class WebServiceUtils
	{
		private static readonly Dictionary<ResponseType, string> responseTypeMessages = new Dictionary<ResponseType, string>
		{
			{ ResponseType.Unknown, "Cannot find server - {1}" },
			{ ResponseType.Successful, "Request successful with code: {0} - {1}" },
			{ ResponseType.Error, "The client encounted an error with code: {0} - {1}" },
			{ ResponseType.ServerError, "The server encounted an error and responded with code: {0} - {1}" }
		};

		public static string GetResponseTypeMessage(ResponseType responseType, long responseCode, string url)
		{
			if (!responseTypeMessages.ContainsKey(responseType))
			{
				return string.Format(responseTypeMessages[ResponseType.Unknown], responseCode, url);
			}
			return string.Format(responseTypeMessages[responseType], responseCode, url);
		}

		public static ResponseType GetResponseType(long responseCode)
		{
			if (CheckIsSuccessful(responseCode)) return ResponseType.Successful;
			if (CheckIsError(responseCode)) return ResponseType.Error;
			if (CheckIsServerError(responseCode)) return ResponseType.ServerError;

			return ResponseType.Unknown;
		}

		private static bool CheckIsSuccessful(long responseCode)
		{
			return responseCode >= 200 && responseCode <= 206;
		}

		private static bool CheckIsError(long responseCode)
		{
			return responseCode >= 400 && responseCode <= 431;
		}

		private static bool CheckIsServerError(long responseCode)
		{
			return responseCode >= 500 && responseCode <= 511;
		}

		public static T TryParse<T>(string text)
		{
			try
			{
				return JsonUtility.FromJson<T>(text);
			}
			catch
			{
				return default(T);
			}
		}
	}
}
