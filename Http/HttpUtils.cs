using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DUCK.Http
{
	/// <summary>
	/// A utility class for webserivce, deals with response codes and error messages.
	/// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status"/>
	/// </summary>
	public static class HttpUtils
	{
		private static readonly Dictionary<ResponseType, string> responseTypeMessages = new Dictionary<ResponseType, string>
		{
			{ResponseType.Unknown, "Cannot find server - {1}"},
			{ResponseType.Successful, "Request successful with code: {0} - {1}"},
			{ResponseType.Error, "The client encounted an error with code: {0} - {1}"},
			{ResponseType.ServerError, "The server encounted an error and responded with code: {0} - {1}"}
		};

		private static readonly Dictionary<long, string> customResponseTypeMessages = new Dictionary<long, string>();

		public static string GetResponseTypeMessage(HttpResponse response)
		{
			if (!responseTypeMessages.ContainsKey(response.ResponseType))
			{
				return string.Format(responseTypeMessages[ResponseType.Unknown], response.ResponseCode, response.Url);
			}
			if(customResponseTypeMessages.ContainsKey(response.ResponseCode))
			{
				return customResponseTypeMessages[response.ResponseCode];
			}
			return string.Format(responseTypeMessages[response.ResponseType], response.ResponseCode, response.Url);
		}

		public static ResponseType GetResponseType(long responseCode)
		{
			if (CheckIsCustomResponseCode(responseCode)) return ResponseType.Custom;
			if (CheckIsSuccessful(responseCode)) return ResponseType.Successful;
			if (CheckIsError(responseCode)) return ResponseType.Error;
			if (CheckIsServerError(responseCode)) return ResponseType.ServerError;

			return ResponseType.Unknown;
		}

		private static bool CheckIsCustomResponseCode(long responseCode)
		{
			return customResponseTypeMessages.Any(customResponseTypeMessage => responseCode == customResponseTypeMessage.Key);
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