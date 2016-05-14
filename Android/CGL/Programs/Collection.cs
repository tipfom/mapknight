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

namespace mapKnight.Android.CGL.Programs {
    public class Collection {
        public EntityProgram Entity;
        public ColorProgram Color;
        public MatrixProgram Matrix;
        public FBOProgram FBO;

        public Collection () {
            ProgramHelper.Prepare ( );

            Entity = new EntityProgram ( );
            Color = new ColorProgram ( );
            Matrix = new MatrixProgram ( );
            FBO = new FBOProgram ( );
        }
    }
}