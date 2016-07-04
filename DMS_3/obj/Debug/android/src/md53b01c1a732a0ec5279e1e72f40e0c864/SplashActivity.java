package md53b01c1a732a0ec5279e1e72f40e0c864;


public class SplashActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;Landroid/os/PersistableBundle;)V:GetOnCreate_Landroid_os_Bundle_Landroid_os_PersistableBundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("DMS_3.SplashActivity, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SplashActivity.class, __md_methods);
	}


	public SplashActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SplashActivity.class)
			mono.android.TypeManager.Activate ("DMS_3.SplashActivity, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0, android.os.PersistableBundle p1)
	{
		n_onCreate (p0, p1);
	}

	private native void n_onCreate (android.os.Bundle p0, android.os.PersistableBundle p1);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
