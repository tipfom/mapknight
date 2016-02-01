package md5d449d5e0b2f23fa803370e40fc7c64c5;


public class ButtonManager
	extends md5d449d5e0b2f23fa803370e40fc7c64c5.TouchManager
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5851.33009, Culture=neutral, PublicKeyToken=null", ButtonManager.class, __md_methods);
	}


	public ButtonManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ButtonManager.class)
			mono.android.TypeManager.Activate ("mapKnight.Android.ButtonManager, mapKnight.Android, Version=2.0.5851.33009, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
