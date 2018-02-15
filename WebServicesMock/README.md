# Web Services Mock

## What is it ?
Web services mock allows you to mock a remote service provided by a back end. By using WebServiesMock you can define services like so:

```c#
WebServicesMock.Instance.Add("https://www.someurl.com/user/1", HTTPMethod.GET, (payload) =>
{
	return new MockWebResponse()
	{
		body = "{ data: 0 }",
		error = null,
		statusCode = 200,
		payload = null,
		url = url
	};
});
```

## Examples
This package comes with a mocked service - MockServerDatabase.

### Creation and Posting new entries
```c#
var url = "https://www.someurl.com/users";
var user1 = new UserData() { coins = 0 };
var user2 = new UserData() { coins = 100 };

var database = new MockServerDatabase<UserData>(url);

WebServicesMock.Instance.SendRequest(url, HTTPMethod.POST, user1, onComplete: (MockWebResponse response) =>
{
	Debug.Log(response.body);
});

WebServicesMock.Instance.SendRequest(url, HTTPMethod.POST, user2, onComplete: (response) =>
{
	WebServicesMock.Instance.SendRequest(url, HTTPMethod.GET, onComplete: (MockWebResponse userDataResponse) =>
	{
		Debug.Log(userDataResponse.body);
	});
});
```

After posting a new database entry you can now do the following requests:

```c#
//GET url/{userID} returns entry
WebServicesMock.Instance.SendRequest(url + "/" + user1.ID, HTTPMethod.GET, onComplete: (payload) => { Debug.Log(payload.body); });
//DELETE url/{userID} deletes entry and all paths using this ID
WebServicesMock.Instance.SendRequest(url + "/" + user1.ID, HTTPMethod.DELETE, onComplete: (payload) => { Debug.Log(payload.body); });
//PUT url/{userID} updates entry
WebServicesMock.Instance.SendRequest(url + "/" + user1.ID, HTTPMethod.PUT, onComplete: (payload) => { Debug.Log(payload.body); });
```

This is all achieved using WebServicesMock like so:

```c#
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
```
