namespace mapKnight.Core.Components.Configs {
    class TextureComponentConfig : ComponentConfig {
        public override int Priority { get { return 1; } }

        public string Texture = "";

        public override Component Create (Entity owner) {
            return new TextureComponent (owner, Texture);
        }
    }
}