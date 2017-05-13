#if __ANDROID__
using OpenTK.Graphics.ES20;
#endif

namespace mapKnight.Core.Graphics {
    public enum InterpolationMode {
        PixelPerfect
#if __ANDROID__
            = (int)All.Nearest
#endif
            ,

        Linear
#if __ANDROID__
            = (int)All.Linear
#endif
            ,
    }
}
