using System;
using System.Net;

public class TimeoutWebclient : WebClient
{
	protected override WebRequest GetWebRequest(Uri uri)
	{
		WebRequest w = base.GetWebRequest(uri);
		w.Timeout = 20 * 60 * 1000;
		return w;
	}
}