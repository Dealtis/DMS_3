package md53b01c1a732a0ec5279e1e72f40e0c864;


public class FlashageQuaiActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		koamtac.kdc.sdk.KDCConnectionListenerEx,
		koamtac.kdc.sdk.KDCBarcodeDataReceivedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onActivityResult:(IILandroid/content/Intent;)V:GetOnActivityResult_IILandroid_content_Intent_Handler\n" +
			"n_ConnectionChanged:(I)V:GetConnectionChanged_IHandler:Koamtac.Kdc.Sdk.IKDCConnectionListenerExInvoker, KoamtacBinding\n" +
			"n_BarcodeDataReceived:(Lkoamtac/kdc/sdk/KDCData;)V:GetBarcodeDataReceived_Lkoamtac_kdc_sdk_KDCData_Handler:Koamtac.Kdc.Sdk.IKDCBarcodeDataReceivedListenerInvoker, KoamtacBinding\n" +
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


	public void onActivityResult (int p0, int p1, android.content.Intent p2)
	{
		n_onActivityResult (p0, p1, p2);
	}

	private native void n_onActivityResult (int p0, int p1, android.content.Intent p2);


	public void ConnectionChanged (int p0)
	{
		n_ConnectionChanged (p0);
	}

	private native void n_ConnectionChanged (int p0);


	public void BarcodeDataReceived (koamtac.kdc.sdk.KDCData p0)
	{
		n_BarcodeDataReceived (p0);
	}

	private native void n_BarcodeDataReceived (koamtac.kdc.sdk.KDCData p0);

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
