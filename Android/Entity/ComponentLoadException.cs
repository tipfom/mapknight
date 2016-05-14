using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace mapKnight.Android.Entity {
    public class ComponentLoadException : TypeLoadException {
        public ComponentLoadException(Component.Type type, string errormessage) : base ($"Error while loading {type.ToString()}Component : {errormessage}") {

        }
    }
}