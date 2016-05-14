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
    public class ComponentDependencyException: Exception {
        public ComponentDependencyException(Component.Type resolvingType, Component.Type dependencyType) : base ($"Error while resolving the dependencies for component of type {resolvingType.ToString()}. The type {dependencyType.ToString()} couldnt be initialized with the default constructor.", new ComponentLoadException(resolvingType,"entity definition isnt correct")) {

        }
    }
}