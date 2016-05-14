using System;

namespace mapKnight.Android.ECS {
    public class ComponentDependencyException : Exception {
        public ComponentDependencyException (ComponentType resolvingType, ComponentType dependencyType) : base ($"Error while resolving the dependencies for component of type {resolvingType.ToString ()}. The type {dependencyType.ToString ()} couldnt be initialized with the default constructor.", new ComponentLoadException (resolvingType, "entity definition isnt correct")) {

        }
    }
}