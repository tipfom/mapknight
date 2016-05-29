using mapKnight.Core.Components.Communication;
using System;

namespace mapKnight.Core.Exceptions {
    public class ComponentDependencyException : Exception {
        public ComponentDependencyException (Identifier resolvingType, Identifier dependencyType) : base ($"Error while resolving the dependencies for component of type {resolvingType.ToString ()}. The type {dependencyType.ToString ()} couldnt be initialized with the default constructor.", new ComponentLoadException (resolvingType, "entity definition isnt correct")) {

        }
    }
}