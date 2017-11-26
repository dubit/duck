﻿using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpResponse
	{
		public bool Ok { get; private set; }
		public bool IsHttpError { get; private set; }
		public bool IsNetworkError { get; private set; }
		public long ResponseCode { get; private set; }
		public ResponseType ResponseType { get; private set; }
		public byte[] Bytes { get; private set; }
		public string Text { get; private set; }
		public Texture Texture { get; private set; }

		public HttpResponse(UnityWebRequest unityWebRequest)
		{
			Bytes = unityWebRequest.downloadHandler.data;
			Text = unityWebRequest.downloadHandler.text;

			var downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
			if (downloadHandlerTexture != null)
			{
				Texture = downloadHandlerTexture.texture;
			}
			IsHttpError = unityWebRequest.isHttpError;
			IsNetworkError = unityWebRequest.isNetworkError;
			ResponseCode = unityWebRequest.responseCode;
			Ok = HttpUtils.GetResponseType(ResponseCode) == ResponseType.Successful;
			ResponseType = HttpUtils.GetResponseType(ResponseCode);
		}
	}
}