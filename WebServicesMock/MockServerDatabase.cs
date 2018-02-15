using System;
using System.Collections.Generic;
using UnityEngine;

namespace DUCK.WebServicesMock
{
	[Serializable]
	public class DatabaseEntity
	{
		public int ID { get { return id; } }

		[SerializeField]
		private int id;
		
		public void SetID(int id)
		{
			this.id = id;
		}
	}

	[Serializable]
	public struct DatabaseData<T>
	{
		[SerializeField]
		public T[] data;
	}

	public class MockServerDatabase<T> where T : DatabaseEntity
	{
		private List<T> database;
		private string url;

		public MockServerDatabase(string url)
		{
			this.url = url;
			database = new List<T>();

			WebServicesMock.Instance.Add(url, HTTPMethod.POST, HandleCreateNewEntry);
			WebServicesMock.Instance.Add(url, HTTPMethod.GET, (payload) =>
			{
				//NOTE Unity ToJSON does not allow pure arrays and must be wrapped up into another class
				var data = new DatabaseData<T>() { data = database.ToArray() };

				return new MockWebResponse()
				{
					body = JsonUtility.ToJson(data),
					error = null,
					statusCode = 200,
					payload = null,
					url = url
				};
			});
		}

		private MockWebResponse HandleCreateNewEntry(object payload)
		{
			var payloadConverted = payload as T;
			payloadConverted.SetID(database.Count);
			database.Add(payloadConverted);

			CreateNewServerPath(payloadConverted);

			return new MockWebResponse()
			{
				body = JsonUtility.ToJson(payloadConverted),
				error = null,
				statusCode = 201,
				payload = null,
				url = url
			};
		}

		private void CreateNewServerPath(T data)
		{
			var newPath = url + '/' + data.ID;
			
			WebServicesMock.Instance.Add(newPath, HTTPMethod.GET, (payload) =>
			{
				return new MockWebResponse()
				{
					body = JsonUtility.ToJson(data),
					error = null,
					statusCode = 200,
					payload = null,
					url = newPath
				};
			});
			
			WebServicesMock.Instance.Add(newPath, HTTPMethod.DELETE, (payload) =>
			{
				database.Remove(data);
				WebServicesMock.Instance.Remove(newPath, HTTPMethod.DELETE);
				WebServicesMock.Instance.Remove(newPath, HTTPMethod.GET);
				WebServicesMock.Instance.Remove(newPath, HTTPMethod.PUT);
				return new MockWebResponse()
				{
					body = null,
					error = null,
					statusCode = 200,
					payload = null,
					url = url
				};
			});

			WebServicesMock.Instance.Add(newPath, HTTPMethod.PUT, (payload) =>
			{
				var payloadConverted = payload as T;
				database[payloadConverted.ID] = payloadConverted;
				return new MockWebResponse()
				{
					body = JsonUtility.ToJson(payloadConverted),
					error = null,
					statusCode = 200,
					payload = null,
					url = newPath
				};
			});
		}

		private MockWebResponse HandleDeleteUser(object data)
		{
			return new MockWebResponse()
			{
				body = null,
				error = null,
				statusCode = 200,
				payload = null,
				url = url
			};
		}
	}
}