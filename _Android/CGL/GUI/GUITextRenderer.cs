using System;
using System.Collections.Generic;
using Java.Nio;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUITextRenderer {
        public const int CHAR_WIDTH_PXL = 7;
        public const int CHAR_HEIGHT_PXL = 9;
        public const int CHAR_SPACING_PXL = 1;
        public const int MAX_CHAR_COUNT = 150;

        private CGLTexture2D fontTexture;
        private float charWidthTexCoord;
        private float charSpacingTexCoord;
        private FloatBuffer vertexBuffer;
        private float[] verticies;
        private FloatBuffer textureBuffer;
        private float[] texCoords;
        private ShortBuffer indexBuffer;
        private List<GUILabel> labels;
        private Dictionary<GUILabel, Stack<int>> aquiredIndicies;
        private Stack<int> freeInidicies;

        public GUITextRenderer () {
            labels = new List<GUILabel> ();
            freeInidicies = new Stack<int> ();
            aquiredIndicies = new Dictionary<GUILabel, Stack<int>> ();

            fontTexture = Assets.LoadTexture ("font.png");
            charWidthTexCoord = CHAR_WIDTH_PXL / (float)fontTexture.Width;
            charSpacingTexCoord = CHAR_SPACING_PXL / (float)fontTexture.Width;

            verticies = new float[MAX_CHAR_COUNT * 8];
            vertexBuffer = CGLTools.CreateBuffer (verticies);
            texCoords = new float[MAX_CHAR_COUNT * 8];
            textureBuffer = CGLTools.CreateBuffer (texCoords);

            // put default values into indexbuffer
            short[] indexBufferContent = new short[MAX_CHAR_COUNT * 6];
            for (int i = 0; i < MAX_CHAR_COUNT; i++) {
                indexBufferContent[i * 6 + 0] = (short)(i * 4 + 0);
                indexBufferContent[i * 6 + 1] = (short)(i * 4 + 1);
                indexBufferContent[i * 6 + 2] = (short)(i * 4 + 2);
                indexBufferContent[i * 6 + 3] = (short)(i * 4 + 0);
                indexBufferContent[i * 6 + 4] = (short)(i * 4 + 2);
                indexBufferContent[i * 6 + 5] = (short)(i * 4 + 3);
            }
            indexBuffer = CGLTools.CreateBuffer (indexBufferContent);
            for (int i = MAX_CHAR_COUNT - 1; i > -1; i--) {
                freeInidicies.Push (i);
            }
        }

        public void Draw () {
            Content.MatrixProgram.Begin ();
            Content.MatrixProgram.EnableAlphaBlending ();

            Content.MatrixProgram.SetMVPMatrix (Content.Camera.DefaultMVPMatrix);
            Content.MatrixProgram.SetTexture (fontTexture.Texture);
            Content.MatrixProgram.SetTextureBuffer (textureBuffer);
            Content.MatrixProgram.SetVertexBuffer (vertexBuffer);
            Content.MatrixProgram.Draw (indexBuffer);

            Content.MatrixProgram.End ();
        }

        public void Add (GUILabel label) {
            if (!labels.Contains (label)) {
                labels.Add (label);
                label.PositionChanged += label_PositionChanged;
                label.TextChanged += label_TextChanged;
                aquiredIndicies.Add (label, new Stack<int> ());
                updateLabel (label);
            } else {
                Console.WriteLine ("label " + label.ToString () + " has allready been added");
            }
        }

        public GUILabel Create (fVector2D position, float size, string text = "default") {
            GUILabel createdLabel = new GUILabel (position, size, text);
            Add (createdLabel);
            return createdLabel;
        }

        private void updateLabel (GUILabel label) {
            int time = Environment.TickCount;
            // call when text changed or a label got added
            while (aquiredIndicies[label].Count > 0) {
                // free up the aquired positions in the vertexbuffer
                int index = aquiredIndicies[label].Pop ();
                freeInidicies.Push (index);
                Array.Copy (new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f }, 0, verticies, index * 8, 8);
            }

            fVector2D currentPoint = label.Position;
            for (int i = 0; i < label.ParsedText.Length; i++) {
                if (label.ParsedText[i] == -1) {
                    // next line char (enter) 
                    // no index needed
                    currentPoint.X = label.Position.X;
                    currentPoint.Y -= label.Size;
                } else if (label.ParsedText[i] == -2) {
                    // space
                    currentPoint.X += label.CharSize.X;
                } else {
                    // normal char
                    int index = freeInidicies.Pop (); // get new index (in vertex buffer) from the free inidicies
                    aquiredIndicies[label].Push (index); // save index
                    Array.Copy (getTextureCoords (label.ParsedText[i]), 0, texCoords, index * 8, 8);
                    Array.Copy (getVerticies (currentPoint, label.CharSize), 0, verticies, index * 8, 8);
                    currentPoint.X += label.CharSize.X;
                }
            }
            updateVertexBuffer ();
            updateTextureBuffer ();
        }

        private float[] getVerticies (fVector2D currentPoint, fVector2D charSize) {
            // gets verticies of the top left point
            return new float[]{
                currentPoint.X,currentPoint.Y - charSize.Y,
                currentPoint.X ,currentPoint.Y,
                currentPoint.X+ charSize.X,currentPoint.Y,
                currentPoint.X+ charSize.X,currentPoint.Y - charSize.Y
            };
        }

        private float[] getTextureCoords (int charindex) {
            return new float[] {
                charindex * charWidthTexCoord + (charindex) * charSpacingTexCoord, 1,
                charindex * charWidthTexCoord + (charindex) * charSpacingTexCoord, 0,
                (charindex + 1) * charWidthTexCoord + (charindex) * charSpacingTexCoord, 0,
                (charindex + 1) * charWidthTexCoord + (charindex) * charSpacingTexCoord, 1
            };
        }

        private void updateVertexBuffer () {
            vertexBuffer.Put (verticies);
            vertexBuffer.Position (0);
        }

        private void updateTextureBuffer () {
            textureBuffer.Put (texCoords);
            textureBuffer.Position (0);
        }

        private void label_TextChanged (GUILabel sender) {
            //if (lengthChange > freeInidicies.Count) {
            //    Console.WriteLine ("couldnt change text of " + sender.ToString () + ", the text was to long");
            //} else {
            updateLabel (sender);
            //}
        }

        private void label_PositionChanged (GUILabel sender) {
            updateLabel (sender);
        }

        public void Remove (GUILabel label) {
            if (labels.Contains (label)) {
                labels.Remove (label);
                while (aquiredIndicies[label].Count > 0) {
                    freeInidicies.Push (aquiredIndicies[label].Pop ());
                }
                aquiredIndicies.Remove (label);
            } else {
                Console.WriteLine ("label " + label.ToString () + " was requested to be deleted while not being added");
            }
        }


        #region static methods
        public static int[] Parse (char[] chars) {
            int[] result = new int[chars.Length];
            for (int i = 0; i < chars.Length; i++) {
                result[i] = Parse (chars[i]);
            }
            return result;
        }

        public static int[] Parse (string text) {
            return Parse (text.ToCharArray ());
        }

        public static int Parse (char character) {
            switch (character) {
            case 'A':
            case 'a':
                return 0;
            case 'B':
            case 'b':
                return 1;
            case 'C':
            case 'c':
                return 2;
            case 'D':
            case 'd':
                return 3;
            case 'E':
            case 'e':
                return 4;
            case 'F':
            case 'f':
                return 5;
            case 'G':
            case 'g':
                return 6;
            case 'H':
            case 'h':
                return 7;
            case 'I':
            case 'i':
                return 8;
            case 'J':
            case 'j':
                return 9;
            case 'K':
            case 'k':
                return 10;
            case 'L':
            case 'l':
                return 11;
            case 'M':
            case 'm':
                return 12;
            case 'N':
            case 'n':
                return 13;
            case 'O':
            case 'o':
                return 14;
            case 'P':
            case 'p':
                return 15;
            case 'Q':
            case 'q':
                return 16;
            case 'R':
            case 'r':
                return 17;
            case 'S':
            case 's':
                return 18;
            case 'T':
            case 't':
                return 19;
            case 'U':
            case 'u':
                return 20;
            case 'V':
            case 'v':
                return 21;
            case 'W':
            case 'w':
                return 22;
            case 'X':
            case 'x':
                return 23;
            case 'Y':
            case 'y':
                return 24;
            case 'Z':
            case 'z':
                return 25;
            case '.':
                return 26;
            case ',':
                return 27;
            case ';':
                return 28;
            case '!':
                return 29;
            case '?':
                return 30;
            case '(':
                return 31;
            case ')':
                return 32;
            case ':':
                return 33;
            case '\'':
                return 34;
            case '1':
                return 35;
            case '2':
                return 36;
            case '3':
                return 37;
            case '4':
                return 38;
            case '5':
                return 39;
            case '6':
                return 40;
            case '7':
                return 41;
            case '8':
                return 42;
            case '9':
                return 43;
            case '0':
                return 44;
            case '\n':
                return -1;
            case ' ':
                return -2;
            default:
                Console.WriteLine ("no character " + character.ToString () + " known");
                return -3;
            }
        }
        #endregion
    }
}