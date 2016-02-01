package md5e27915f66df85f12998702822c7d6fd2;


public class CGLView
	extends android.opengl.GLSurfaceView
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("mapKnight.Android.CGL.CGLView, mapKnight.Android, Version=2.0.5869.33436, Culture=neutral, PublicKeyToken=null", CGLView.class, __md_methods);
	}


	public CGLView (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == CGLView.class)
			mono.android.TypeManager.Activate ("mapKnight.Android.CGL.CGLView, mapKnight.Android, Version=2.0.5869.33436, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public CGLView (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == CGLView.class)
			mono.android.TypeManager.Activate ("mapKnight.Android.CGL.CGLView, mapKnight.Android, Version=2.0.5869.33436, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
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
