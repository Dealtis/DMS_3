package md5f3ecce746ed436115965efa790941330;


public class SwipeListViewListenerDispatcher
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.fortysevendeg.swipelistview.SwipeListViewListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onChangeSwipeMode:(I)I:GetOnChangeSwipeMode_IHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onChoiceChanged:(IZ)V:GetOnChoiceChanged_IZHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onChoiceEnded:()V:GetOnChoiceEndedHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onChoiceStarted:()V:GetOnChoiceStartedHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onClickBackView:(I)V:GetOnClickBackView_IHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onClickFrontView:(I)V:GetOnClickFrontView_IHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onClosed:(IZ)V:GetOnClosed_IZHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onDismiss:([I)V:GetOnDismiss_arrayIHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onFirstListItem:()V:GetOnFirstListItemHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onLastListItem:()V:GetOnLastListItemHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onListChanged:()V:GetOnListChangedHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onMove:(IF)V:GetOnMove_IFHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onOpened:(IZ)V:GetOnOpened_IZHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onStartClose:(IZ)V:GetOnStartClose_IZHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"n_onStartOpen:(IIZ)V:GetOnStartOpen_IIZHandler:FortySevenDeg.SwipeListView.ISwipeListViewListenerInvoker, FortySevenDeg.SwipeListView\n" +
			"";
		mono.android.Runtime.register ("FortySevenDeg.SwipeListView.SwipeListViewListenerDispatcher, FortySevenDeg.SwipeListView, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null", SwipeListViewListenerDispatcher.class, __md_methods);
	}


	public SwipeListViewListenerDispatcher () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SwipeListViewListenerDispatcher.class)
			mono.android.TypeManager.Activate ("FortySevenDeg.SwipeListView.SwipeListViewListenerDispatcher, FortySevenDeg.SwipeListView, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public SwipeListViewListenerDispatcher (com.fortysevendeg.swipelistview.SwipeListView p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == SwipeListViewListenerDispatcher.class)
			mono.android.TypeManager.Activate ("FortySevenDeg.SwipeListView.SwipeListViewListenerDispatcher, FortySevenDeg.SwipeListView, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null", "FortySevenDeg.SwipeListView.SwipeListView, FortySevenDeg.SwipeListView, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public int onChangeSwipeMode (int p0)
	{
		return n_onChangeSwipeMode (p0);
	}

	private native int n_onChangeSwipeMode (int p0);


	public void onChoiceChanged (int p0, boolean p1)
	{
		n_onChoiceChanged (p0, p1);
	}

	private native void n_onChoiceChanged (int p0, boolean p1);


	public void onChoiceEnded ()
	{
		n_onChoiceEnded ();
	}

	private native void n_onChoiceEnded ();


	public void onChoiceStarted ()
	{
		n_onChoiceStarted ();
	}

	private native void n_onChoiceStarted ();


	public void onClickBackView (int p0)
	{
		n_onClickBackView (p0);
	}

	private native void n_onClickBackView (int p0);


	public void onClickFrontView (int p0)
	{
		n_onClickFrontView (p0);
	}

	private native void n_onClickFrontView (int p0);


	public void onClosed (int p0, boolean p1)
	{
		n_onClosed (p0, p1);
	}

	private native void n_onClosed (int p0, boolean p1);


	public void onDismiss (int[] p0)
	{
		n_onDismiss (p0);
	}

	private native void n_onDismiss (int[] p0);


	public void onFirstListItem ()
	{
		n_onFirstListItem ();
	}

	private native void n_onFirstListItem ();


	public void onLastListItem ()
	{
		n_onLastListItem ();
	}

	private native void n_onLastListItem ();


	public void onListChanged ()
	{
		n_onListChanged ();
	}

	private native void n_onListChanged ();


	public void onMove (int p0, float p1)
	{
		n_onMove (p0, p1);
	}

	private native void n_onMove (int p0, float p1);


	public void onOpened (int p0, boolean p1)
	{
		n_onOpened (p0, p1);
	}

	private native void n_onOpened (int p0, boolean p1);


	public void onStartClose (int p0, boolean p1)
	{
		n_onStartClose (p0, p1);
	}

	private native void n_onStartClose (int p0, boolean p1);


	public void onStartOpen (int p0, int p1, boolean p2)
	{
		n_onStartOpen (p0, p1, p2);
	}

	private native void n_onStartOpen (int p0, int p1, boolean p2);

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
