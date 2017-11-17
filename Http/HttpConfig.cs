using UnityEngine;

namespace DUCK.Http
{
	public class HttpConfig : ScriptableObject
	{
		[Tooltip("The expected markup language to parse")]
		[SerializeField]
		private MarkUpType markupType;

		[Tooltip("The object that represents an error from the server.")]
		[SerializeField]
		private HttpError errorType;
	}
}