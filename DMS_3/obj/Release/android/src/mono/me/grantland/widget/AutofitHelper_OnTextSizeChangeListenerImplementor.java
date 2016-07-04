package mono.me.grantland.widget;


public class AutofitHelper_OnTextSizeChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		me.grantland.widget.AutofitHelper.OnTextSizeChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTextSizeChange:(FF)V:GetOnTextSizeChange_FFHandler:Grantland.Widget.AutofitHelper/IOnTextSizeChangeListenerInvoker, AutoFitTextView\n" +
			"";
		mono.android.Runtime.register ("Grantland.Widget.AutofitHelper+IOnTextSizeChangeListenerImplementor, AutoFitTextView, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null", AutofitHelper_OnTextSizeChangeListenerImplementor.class, __md_methods);
	}


	public AutofitHelper_OnTextSizeChangeListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AutofitHelper_OnTextSizeChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Grantland.Widget.AutofitHelper+IOnTextSizeChangeListenerImplementor, AutoFitTextView, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTextSizeChange (float p0, float p1)
	{
		n_onTextSizeChange (p0, p1);
	}

	private native void n_onTextSizeChange (float p0, float p1);

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
