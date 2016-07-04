package md53b01c1a732a0ec5279e1e72f40e0c864;


public class ProcessDMSBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DMS_3.ProcessDMSBinder, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ProcessDMSBinder.class, __md_methods);
	}


	public ProcessDMSBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ProcessDMSBinder.class)
			mono.android.TypeManager.Activate ("DMS_3.ProcessDMSBinder, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ProcessDMSBinder (md53b01c1a732a0ec5279e1e72f40e0c864.ProcessDMS p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ProcessDMSBinder.class)
			mono.android.TypeManager.Activate ("DMS_3.ProcessDMSBinder, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "DMS_3.ProcessDMS, DMS_3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}

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
