package md5a107dd07e4107653063c816ecb9af8b6;


public class ImageActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Sample.Android.ImageActivity, ZXingSampleAndroid, Version=2.1.12.0, Culture=neutral, PublicKeyToken=null", ImageActivity.class, __md_methods);
	}


	public ImageActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ImageActivity.class)
			mono.android.TypeManager.Activate ("Sample.Android.ImageActivity, ZXingSampleAndroid, Version=2.1.12.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
