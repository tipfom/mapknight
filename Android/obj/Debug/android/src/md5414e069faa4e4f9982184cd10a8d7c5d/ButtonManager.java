package md5414e069faa4e4f9982184cd10a8d7c5d;


public class ButtonManager
	extends md5414e069faa4e4f9982184cd10a8d7c5d.TouchManager
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5869.33343, Culture=neutral, PublicKeyToken=null", ButtonManager.class, __md_methods);
	}


	public ButtonManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ButtonManager.class)
			mono.android.TypeManager.Activate ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5869.33343, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
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
