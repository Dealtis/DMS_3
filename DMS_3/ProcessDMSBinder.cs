using System;
using Android.Content;
using Android.OS;

namespace DMS_3
{
	public class ProcessDMSBinder : Binder
	{
		ProcessDMS service;

		public ProcessDMSBinder(ProcessDMS service)
		{
			this.service = service;
		}

		public ProcessDMS GetDemoService()
		{
			return service;
		}

		public ProcessDMS StopService()
		{
			return service;
		}
	}

	public class ProcessDMSBConnection : Java.Lang.Object, IServiceConnection
	{
		public bool IsConnected { get; private set; }
		public ProcessDMSBinder Binder { get; private set; }

		public ProcessDMSBConnection()
		{
			IsConnected = false;
			Binder = null;
		}

		public void OnServiceConnected(ComponentName name, IBinder binder)
		{
			Binder = binder as ProcessDMSBinder;
			IsConnected = Binder != null;
		}

		public void OnServiceDisconnected(ComponentName name)
		{
			IsConnected = false;
			Binder = null;
		}
	}
}
