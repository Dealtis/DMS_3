package mono.koamtac.kdc.sdk;


public class KDCBarcodeDataReceivedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		koamtac.kdc.sdk.KDCBarcodeDataReceivedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_BarcodeDataReceived:(Lkoamtac/kdc/sdk/KDCData;)V:GetBarcodeDataReceived_Lkoamtac_kdc_sdk_KDCData_Handler:Koamtac.Kdc.Sdk.IKDCBarcodeDataReceivedListenerInvoker, KoamtacBinding\n" +
			"";
		mono.android.Runtime.register ("Koamtac.Kdc.Sdk.IKDCBarcodeDataReceivedListenerImplementor, KoamtacBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", KDCBarcodeDataReceivedListenerImplementor.class, __md_methods);
	}


	public KDCBarcodeDataReceivedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == KDCBarcodeDataReceivedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Koamtac.Kdc.Sdk.IKDCBarcodeDataReceivedListenerImplementor, KoamtacBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


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
