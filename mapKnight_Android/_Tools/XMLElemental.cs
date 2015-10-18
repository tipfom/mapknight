using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace mapKnight_Android
{
	namespace Utils
	{
		public class XMLElemental : Object
		{

			private List<XMLElemental> Childs;
			private List<string> Comments;

			public XMLElemental Parent { get; private set; }

			public Dictionary<string,string> Attributes { get; private set; }
			public string Value { get; set; }
			public string Name { get; set; }
			public string Root { get; private set; }
			public int Depth { get; private set; }

			public XMLElemental (XMLElemental parent, string name, string value, Dictionary<string,string> attributes)
			{
				Name = name;
				Parent = parent;
				Value = value;
				Attributes = attributes;
				Root = Parent.Root+ "?" + Parent.GetAll (((XMLElemental elemental) => elemental.Root.StartsWith (Parent.Root + name))).Count.ToString () + "~";;

				Depth = Parent.Depth + 1;

				Childs = new List<XMLElemental> ();
				Comments = new List<string> ();
			}
		
			public XMLElemental (XMLElemental parent, string name) : this (parent, name, "", new Dictionary<string,string> ()) { }

			protected XMLElemental (string name)
			{
				Name = name;
				Parent = null;
				Value = "";
				Root = name + "?0~";

				this.Depth = 0;

				Attributes = new Dictionary<string, string> ();
				Childs = new List<XMLElemental> ();
				Comments = new List<string> ();
			}

			public static XMLElemental EmptyRootElemental()
			{
				return new XMLElemental ("root");
			}

			public static XMLElemental EmptyRootElemental(string rootname)
			{
				return new XMLElemental (rootname);
			}

			public static XMLElemental Load(Stream FileStream, bool LoadComments)
			{
				using (StreamReader streamreader = new StreamReader (FileStream)) {
					return Load (streamreader.ReadToEnd (), LoadComments);
				}
			}

			public static XMLElemental Load(string XMLData, bool LoadComments)
			{

				XMLElemental LoadedElemetal = null;

				using (XmlReader xmlreader = XmlReader.Create (new StringReader (XMLData))) {

					XMLElemental CurrentElemental = null;

					while (xmlreader.Read ()) {
						switch (xmlreader.NodeType) {
						case XmlNodeType.Element:
							if (LoadedElemetal == null) {
								LoadedElemetal = XMLElemental.EmptyRootElemental (xmlreader.Name);
								CurrentElemental = LoadedElemetal;
							} else {
								CurrentElemental.AddChild (new XMLElemental (CurrentElemental, xmlreader.Name));
								CurrentElemental = CurrentElemental.GetLastChild ();
							}

							bool IsEmpty = xmlreader.IsEmptyElement;

							if (xmlreader.HasAttributes) {
								while (xmlreader.MoveToNextAttribute ()) {
									CurrentElemental.Attributes.Add (xmlreader.Name, xmlreader.Value);
								}
							}
							if (IsEmpty) {
								CurrentElemental = CurrentElemental.Parent;
							}
							break;
						case XmlNodeType.EndElement:
							CurrentElemental = CurrentElemental.Parent;

							break;
						case XmlNodeType.Text:
							CurrentElemental.Value = xmlreader.Value;
							break;
						case XmlNodeType.Comment:
							if (LoadComments)
								CurrentElemental.AddComment (xmlreader.Value);
							break;
						}
					}
				}

				return LoadedElemetal;
			}

			public XMLElemental this[string name] 
			{
				get {
					return Childs.Find ((XMLElemental elemental) => elemental.Name == name);
				}
			}

			public XMLElemental this[Predicate<XMLElemental> predicate] 
			{
				get {
					return Childs.Find (predicate);
				}
			}

			public List<XMLElemental> GetAll()
			{
				return Childs;	
			}

			public List<XMLElemental> GetAll(string name)
			{
				return Childs.FindAll ((XMLElemental elemental) => elemental.Name == name);
			}

			public List<XMLElemental> GetAll(Predicate<XMLElemental> predicate)
			{
				return Childs.FindAll (predicate);
			}

			public XMLElemental Get(string name)
			{
				return Childs.Find ((XMLElemental elemental) => elemental.Name == name);
			}

			public XMLElemental Get(Predicate<XMLElemental> predicate)
			{
				return Childs.Find (predicate);
			}

			public XMLElemental GetLastChild ()
			{
				if (Childs.Count > 0) {
					return Childs [Childs.Count - 1];
				} else {
					return null;
				}
			}

			public bool HasChild(string name)
			{
				return Childs.Find ((XMLElemental elemental) => elemental.Name == name) != null;
			}

			public bool HasChild(Predicate<XMLElemental> predicate)
			{
				return Childs.Find (predicate) != null;
			}

			public bool HasChild(XMLElemental child)
			{
				return Childs.Contains (child);
			}

			public XMLElemental AddChild(XMLElemental child)
			{
				Childs.Add (child);
				return child;
			}

			public XMLElemental AddChild(string name)
			{
				Childs.Add (new XMLElemental (this, name));
				return GetLastChild ();
			}

			public void AddChilds(IEnumerable<XMLElemental> childs)
			{
				Childs.AddRange (childs);
			}

			public void ClearChilds()
			{
				Childs.Clear ();	
			}

			public void RemoveChild(string name)
			{
				Childs.Remove (Childs.Find ((XMLElemental elemental) => elemental.Name == name));
			}

			public void RemoveChild(Predicate<XMLElemental> predicate)
			{
				Childs.Remove (Childs.Find (predicate));
			}

			public void RemoveChild(XMLElemental elemental)
			{
				if (Childs.Contains (elemental))
					Childs.Remove (elemental);
			}

			public void RemoveChild(int index)
			{
				if (Childs.Count > index)
					Childs.RemoveAt (index);
			}

			public void RemoveLastChild()
			{
				RemoveChild (this.GetLastChild ());
			}

			public void RemoveChilds(string name)
			{
				Childs.RemoveAll ((XMLElemental elemental) => elemental.Name == name);
			}

			public void RemoveChilds(Predicate<XMLElemental> predicate)
			{
				Childs.RemoveAll (predicate);
			}

			public int ChildCount { get{ return Childs.Count; } }

			public void AddComment(string content)
			{
				Comments.Add (content);
			}

			public void RemoveComment(string content)
			{
				if (Comments.Contains (content))
					Comments.Remove (content);
			}

			public void RemoveComment(int index)
			{
				Comments.RemoveAt (index);
			}

			public string Flush()
			{
				XmlWriterSettings settings = new XmlWriterSettings ();
				settings.Indent = true;

				StringBuilder builder = new StringBuilder ();

				using (XmlWriter xmlwriter = XmlWriter.Create (builder, settings)) {
					xmlwriter.WriteStartDocument ();

					xmlwriter.WriteStartElement (this.Name);
					foreach (KeyValuePair<string,string> Attribute in Attributes) {
						xmlwriter.WriteAttributeString (Attribute.Key, Attribute.Value);
					}
					foreach (string Comment in Comments) {
						xmlwriter.WriteComment (Comment);
					}
					if (this.Value != "")
						xmlwriter.WriteString (this.Value);

					foreach (XMLElemental Child in Childs) {
						Child.Flush (xmlwriter);
					}

					xmlwriter.WriteEndElement ();

					xmlwriter.WriteEndDocument ();
				}

				return builder.ToString ();
			}

			protected void Flush(XmlWriter xmlwriter)
			{
				xmlwriter.WriteStartElement (this.Name);

				foreach (KeyValuePair<string,string> Attribute in Attributes) {
					xmlwriter.WriteAttributeString (Attribute.Key, Attribute.Value);
				}
				foreach (string Comment in Comments) {
					xmlwriter.WriteComment (Comment);
				}
				if (this.Value != "")
					xmlwriter.WriteString (this.Value);

				foreach (XMLElemental Child in Childs) {
					Child.Flush (xmlwriter);
				}

				xmlwriter.WriteEndElement ();
			}

			public override string ToString ()
			{
				string ReturnedValue = Name + "@" + Root + " : " + Value + " with " +"{" + string.Join(",", Attributes.Select(kv => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray()) + "}" + "\n";
				foreach (XMLElemental Child in Childs) {
					for (int i = 0; i < Depth + 1; i++) {
						ReturnedValue += "\t";
					}
					ReturnedValue += Child.ToString ();
				}
				return ReturnedValue;
			}
		}
	}
}