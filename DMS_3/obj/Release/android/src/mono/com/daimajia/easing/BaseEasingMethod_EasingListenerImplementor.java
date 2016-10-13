package mono.com.daimajia.easing;


public class BaseEasingMethod_EasingListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.daimajia.easing.BaseEasingMethod.EasingListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_on:(FFFFF)V:GetOn_FFFFFHandler:AndroidEasingFunctions.BaseEasingMethod/IEasingListenerInvoker, AndroidEasingFunctions\n" +
			"";
		mono.android.Runtime.register ("AndroidEasingFunctions.BaseEasingMethod+IEasingListenerImplementor, AndroidEasingFunctions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BaseEasingMethod_EasingListenerImplementor.class, __md_methods);
	}


	public BaseEasingMethod_EasingListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BaseEasingMethod_EasingListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidEasingFunctions.BaseEasingMethod+IEasingListenerImplementor, AndroidEasingFunctions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void on (float p0, float p1, float p2, float p3, float p4)
	{
		n_on (p0, p1, p2, p3, p4);
	}

	private native void n_on (float p0, float p1, float p2, float p3, float p4);

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
