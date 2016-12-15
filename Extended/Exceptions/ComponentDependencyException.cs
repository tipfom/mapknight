using mapKnight.Extended.Components;
using System;

namespace mapKnight.Extended.Exceptions {
    public class ComponentDependencyException : Exception {
        public ComponentDependencyException (Type resolvingType, Type dependencyType) : base($"Error while resolving the dependencies for component of type {resolvingType.ToString( )}. The type {dependencyType.ToString( )} couldnt be initialized with the default constructor.", new ComponentLoadException(resolvingType, "entity definition isnt correct")) {

        }
    }
}