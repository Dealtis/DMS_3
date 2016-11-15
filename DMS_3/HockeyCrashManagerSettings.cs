using System;
using HockeyApp.Android;

namespace DMS_3
{
	public class HockeyCrashManagerSettings : CrashManagerListener
	{
		public override bool ShouldAutoUploadCrashes()
		{
			return true;
		}

		public string getUserID()
		{
			return Data.userAndsoft;
		}
	}

}
