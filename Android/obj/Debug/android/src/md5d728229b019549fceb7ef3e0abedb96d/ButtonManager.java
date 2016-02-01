package md5d728229b019549fceb7ef3e0abedb96d;


public class ButtonManager
	extends md5d728229b019549fceb7ef3e0abedb96d.TouchManager
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5869.32995, Culture=neutral, PublicKeyToken=null", ButtonManager.class, __md_methods);
	}


	public ButtonManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ButtonManager.class)
			mono.android.TypeManager.Activate ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5869.32995, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
