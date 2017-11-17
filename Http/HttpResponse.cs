namespace DUCK.Http
{
	public class HttpResponse : HttpResponse<object>
	{
	}

	public class HttpResponse<T>
	{
		public long ResponseCode { get; set; }
		public string Message { get; set; }
		public T Body { get; set; }
	}
}