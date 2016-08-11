package md53b01c1a732a0ec5279e1e72f40e0c864;


public class FlashageQuaiActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onCreatePanelMenu:(ILandroid/view/Menu;)Z:GetOnCreatePanelMenu_ILandroid_view_Menu_Handler\n" +
			"n_onOptionsItemSelected:(Landroid/view/MenuItem;)Z:GetOnOptionsItemSelected_Landroid_view_MenuItem_Handler\n" +
			"";
		mono.android.Runtime.register ("DMS_3.FlashageQuaiActivity, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FlashageQuaiActivity.class, __md_methods);
	}


	public FlashageQuaiActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == FlashageQuaiActivity.class)
			mono.android.TypeManager.Activate ("DMS_3.FlashageQuaiActivity, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public boolean onCreatePanelMenu (int p0, android.view.Menu p1)
	{
		return n_onCreatePanelMenu (p0, p1);
	}

	private native boolean n_onCreatePanelMenu (int p0, android.view.Menu p1);


	public boolean onOptionsItemSelected (android.view.MenuItem p0)
	{
		return n_onOptionsItemSelected (p0);
	}

	private native boolean n_onOptionsItemSelected (android.view.MenuItem p0);

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
