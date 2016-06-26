using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Configs {
    public class UserControlComponentConfig : ComponentConfig {
        public override int Priority { get { return 2; } }

        private UserControlComponent.IInputProvider inputProvider;

        public UserControlComponentConfig (UserControlComponent.IInputProvider inputprovider) {
            inputProvider = inputprovider;
        }

        public override Component Create (Entity owner) {
            return new UserControlComponent(owner, inputProvider);
        }
    }
}
