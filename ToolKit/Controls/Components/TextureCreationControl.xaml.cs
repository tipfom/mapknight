using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Size = System.Drawing.Size;
using Newtonsoft.Json;

namespace mapKnight.ToolKit {

    /// <summary>
    /// Interaktionslogik für TextureCreationControl.xaml
    /// </summary>
    public partial class TextureCreationControl : UserControl {
        /*
         * All texture packing algorithms where copied from
         * https://spritesheetpacker.codeplex.com/
         * Just the UI was written for the Editor by me
         */

        const int MAX_IMAGE_WIDTH = 4096;
        const int MAX_IMAGE_HEIGHT = 4096;

        public TextureCreationControl ( ) {
            InitializeComponent( );
        }

        public void Build (string textureName) {
            Bitmap image;
            Dictionary<string, Rectangle> dictionary;
            ImagePacker.PackImage(listbox_textures.Items.Cast<TextureItem>( ).Select(item => item.FullFileName), false, false, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT, 1, true, out image, out dictionary);
            if (!Directory.Exists(App.Project.Home + @"\textures\"))
                Directory.CreateDirectory(App.Project.Home + @"\textures\");
            image.Save(App.Project.Home + @"\textures\" + textureName + ".png",ImageFormat.Png);
            // parse dictionary
            Dictionary<string, int[ ]> dictionaryToSerialize = dictionary.ToDictionary(key => Path.GetFileNameWithoutExtension( key.Key), value => new int[ ] { value.Value.X, value.Value.Y, value.Value.Width, value.Value.Height });
            string dictionarySerialized = JsonConvert.SerializeObject(dictionaryToSerialize);
            File.WriteAllText(App.Project.Home + @"\textures\" + textureName + ".json", dictionarySerialized);
            MessageBox.Show("finished");
        }
        
        private void button_discard_Click (object sender, RoutedEventArgs e) {
            listbox_textures.Items.Remove(((DockPanel)((Button)sender).Parent).DataContext);
        }

        private void listbox_textures_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void listbox_textures_Drop (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (MiscHelper.IsImageFile(file)) {
                        listbox_textures.Items.Add(new TextureItem(file));
                    }
                }
            }
        }

        private struct TextureItem {

            public TextureItem (string filename) {
                FullFileName = filename;
                Image = new BitmapImage( );
                Image.BeginInit( );
                Image.CacheOption = BitmapCacheOption.OnLoad;
                Image.CreateOptions = BitmapCreateOptions.None;
                Image.DecodePixelHeight = 30;
                Image.DecodePixelWidth = 30;
                Image.UriSource = new Uri(filename);
                Image.EndInit( );
                SpriteName = Path.GetFileNameWithoutExtension(filename);
            }

            public string FullFileName { get; }
            public BitmapImage Image { get; }
            public string SpriteName { get; }
        }

        #region stolen part

        public static class MiscHelper {

            // the valid extensions for images
            public static readonly string[ ] AllowedImageExtensions = new[ ] { "png", "jpg", "jpeg", "bmp", "gif" };

            // stolen from http://en.wikipedia.org/wiki/Power_of_two#Algorithm_to_find_the_next-highest_power_of_two
            public static int FindNextPowerOfTwo (int k) {
                k--;
                for (int i = 1; i < sizeof(int) * 8; i <<= 1)
                    k = k | k >> i;
                return k + 1;
            }

            // determines if a file is an image we accept
            public static bool IsImageFile (string file) {
                if (!File.Exists(file))
                    return false;

                // ToLower for string comparisons
                string fileLower = file.ToLower( );

                // see if the file ends with one of our valid extensions
                foreach (var ext in AllowedImageExtensions)
                    if (fileLower.EndsWith(ext))
                        return true;
                return false;
            }
        }

        private static class ImagePacker {
            /// <summary>
            /// Packs a collection of images into a single image.
            /// </summary>
            /// <param name="imageFiles">The list of file paths of the images to be combined.</param>
            /// <param name="requirePowerOfTwo">
            /// Whether or not the output image must have a power of two size.
            /// </param>
            /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
            /// <param name="maximumWidth">The maximum width of the output image.</param>
            /// <param name="maximumHeight">The maximum height of the output image.</param>
            /// <param name="imagePadding">
            /// The amount of blank space to insert in between individual images.
            /// </param>
            /// <param name="generateMap">Whether or not to generate the map dictionary.</param>
            /// <param name="outputImage">The resulting output image.</param>
            /// <param name="outputMap">
            /// The resulting output map of placement rectangles for the images.
            /// </param>
            /// <returns>0 if the packing was successful, error code otherwise.</returns>
            public static void PackImage (
                IEnumerable<string> imageFiles,
                bool requirePow2,
                bool requireSquare,
                int maximumWidth,
                int maximumHeight,
                int padding,
                bool generateMap,
                out Bitmap outputImage,
                out Dictionary<string, Rectangle> outputMap) {

                List<string> files = new List<string>(imageFiles);
                Dictionary<string, Rectangle> imagePlacement = new Dictionary<string, Rectangle>( );
                Dictionary<string, Size> imageSizes = new Dictionary<string, Size>( );

                int outputWidth = maximumWidth;
                int outputHeight = maximumHeight;

                outputImage = null;
                outputMap = null;

                // make sure our dictionaries are cleared before starting
                imageSizes.Clear( );
                imagePlacement.Clear( );

                // get the sizes of all the images
                foreach (var image in files) {
                    Bitmap bitmap = Bitmap.FromFile(image) as Bitmap;
                    if (bitmap == null)
                        throw new Exception("failed to load image");
                    imageSizes.Add(image, bitmap.Size);
                }

                // sort our files by file size so we place large sprites first
                files.Sort(
                    (f1, f2) => {
                        Size b1 = imageSizes[f1];
                        Size b2 = imageSizes[f2];

                        int c = -b1.Width.CompareTo(b2.Width);
                        if (c != 0)
                            return c;

                        c = -b1.Height.CompareTo(b2.Height);
                        if (c != 0)
                            return c;

                        return f1.CompareTo(f2);
                    });

                // try to pack the images
                if (!PackImageRectangles(imageSizes,imagePlacement, files, ref outputWidth, ref outputHeight, padding, requirePow2, requireSquare )) {
                    MessageBox.Show("something went wrong while building the sprites.\nYou may check the texturesizes to not exceed 4096 by 4096 pixels.");
                    return;
                }
                // make our output image
                outputImage = CreateOutputImage(outputWidth, outputHeight, files, imagePlacement );
                if (outputImage == null) {
                    MessageBox.Show("something went wrong while building the sprites.\nIt seems like your GPU is confused.");
                    return;
                }

                if (generateMap) {
                    // go through our image placements and replace the width/height found in there
                    // with each image's actual width/height (since the ones in imagePlacement will
                    // have padding)
                    string[ ] keys = new string[imagePlacement.Keys.Count];
                    imagePlacement.Keys.CopyTo(keys, 0);
                    foreach (var k in keys) {
                        // get the actual size
                        Size s = imageSizes[k];

                        // get the placement rectangle
                        Rectangle r = imagePlacement[k];

                        // set the proper size
                        r.Width = s.Width;
                        r.Height = s.Height;

                        // insert back into the dictionary
                        imagePlacement[k] = r;
                    }

                    // copy the placement dictionary to the output
                    outputMap = new Dictionary<string, Rectangle>( );
                    foreach (var pair in imagePlacement) {
                        outputMap.Add(pair.Key, pair.Value);
                    }
                }

                // clear our dictionaries just to free up some memory
                imageSizes.Clear( );
                imagePlacement.Clear( );

                return;
            }

            private static Bitmap CreateOutputImage ( int outputWidth, int outputHeight, List<string> files, Dictionary<string, Rectangle> imagePlacement) {
                try {
                    Bitmap outputImage = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb);

                    // draw all the images into the output image
                    foreach (var image in files) {
                        Rectangle location = imagePlacement[image];
                        Bitmap bitmap = Bitmap.FromFile(image) as Bitmap;
                        if (bitmap == null)
                            return null;

                        // copy pixels over to avoid antialiasing or any other side effects of
                        // drawing the subimages to the output image using Graphics
                        for (int x = 0; x < bitmap.Width; x++)
                            for (int y = 0; y < bitmap.Height; y++)
                                outputImage.SetPixel(location.X + x, location.Y + y, bitmap.GetPixel(x, y));
                    }

                    return outputImage;
                } catch {
                    return null;
                }
            }

            // This method does some trickery type stuff where we perform the TestPackingImages
            // method over and over, trying to reduce the image size until we have found the smallest
            // possible image we can fit.
            private static bool PackImageRectangles (Dictionary<string, Size> imageSizes, Dictionary<string, Rectangle> imagePlacement, List<string> files, ref int outputWidth, ref int outputHeight, int padding , bool requirePow2, bool requireSquare) {
                // create a dictionary for our test image placements
                Dictionary<string, Rectangle> testImagePlacement = new Dictionary<string, Rectangle>( );

                // get the size of our smallest image
                int smallestWidth = int.MaxValue;
                int smallestHeight = int.MaxValue;
                foreach (var size in imageSizes) {
                    smallestWidth = Math.Min(smallestWidth, size.Value.Width);
                    smallestHeight = Math.Min(smallestHeight, size.Value.Height);
                }

                // we need a couple values for testing
                int testWidth = outputWidth;
                int testHeight = outputHeight;

                bool shrinkVertical = false;

                // just keep looping...
                while (true) {
                    // make sure our test dictionary is empty
                    testImagePlacement.Clear( );

                    // try to pack the images into our current test size
                    if (!TestPackingImages(testWidth, testHeight, testImagePlacement,files, imageSizes, padding )) {
                        // if that failed...

                        // if we have no images in imagePlacement, i.e. we've never succeeded at
                        // PackImages, show an error and return false since there is no way to fit
                        // the images into our maximum size texture
                        if (imagePlacement.Count == 0)
                            return false;

                        // otherwise return true to use our last good results
                        if (shrinkVertical)
                            return true;

                        shrinkVertical = true;
                        testWidth += smallestWidth + padding + padding;
                        testHeight += smallestHeight + padding + padding;
                        continue;
                    }

                    // clear the imagePlacement dictionary and add our test results in
                    imagePlacement.Clear( );
                    foreach (var pair in testImagePlacement)
                        imagePlacement.Add(pair.Key, pair.Value);

                    // figure out the smallest bitmap that will hold all the images
                    testWidth = testHeight = 0;
                    foreach (var pair in imagePlacement) {
                        testWidth = Math.Max(testWidth, pair.Value.Right);
                        testHeight = Math.Max(testHeight, pair.Value.Bottom);
                    }

                    // subtract the extra padding on the right and bottom
                    if (!shrinkVertical)
                        testWidth -= padding;
                    testHeight -= padding;

                    // if we require a power of two texture, find the next power of two that can fit
                    // this image
                    if (requirePow2) {
                        testWidth = MiscHelper.FindNextPowerOfTwo(testWidth);
                        testHeight = MiscHelper.FindNextPowerOfTwo(testHeight);
                    }

                    // if we require a square texture, set the width and height to the larger of the two
                    if (requireSquare) {
                        int max = Math.Max(testWidth, testHeight);
                        testWidth = testHeight = max;
                    }

                    // if the test results are the same as our last output results, we've reached an
                    // optimal size, so we can just be done
                    if (testWidth == outputWidth && testHeight == outputHeight) {
                        if (shrinkVertical)
                            return true;

                        shrinkVertical = true;
                    }

                    // save the test results as our last known good results
                    outputWidth = testWidth;
                    outputHeight = testHeight;

                    // subtract the smallest image size out for the next test iteration
                    if (!shrinkVertical)
                        testWidth -= smallestWidth;
                    testHeight -= smallestHeight;
                }
            }

            private static bool TestPackingImages (int testWidth, int testHeight, Dictionary<string, Rectangle> testImagePlacement, List<string> files, Dictionary<string, Size> imageSizes, int padding) {
                // create the rectangle packer
                ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);

                foreach (var image in files) {
                    // get the bitmap for this file
                    Size size = imageSizes[image];

                    // pack the image
                    Point origin;
                    if (!rectanglePacker.TryPack(size.Width + padding, size.Height + padding, out origin)) {
                        return false;
                    }

                    // add the destination rectangle to our dictionary
                    testImagePlacement.Add(image, new Rectangle(origin.X, origin.Y, size.Width + padding, size.Height + padding));
                }

                return true;
            }
        }

        /// <summary>
        /// Rectangle packer using an algorithm by Javier Arevalo
        /// </summary>
        /// <remarks>
        /// <para>
        /// Original code by Javier Arevalo (jare at iguanademos dot com). Rewritten to C# / .NET by
        /// Markus Ewald (cygon at nuclex dot org). The following comments were written by the
        /// original author when he published his algorithm.
        /// </para>
        /// <para>
        /// You have a bunch of rectangular pieces. You need to arrange them in a rectangular surface
        /// so that they don't overlap, keeping the total area of the rectangle as small as possible.
        /// This is fairly common when arranging characters in a bitmapped font, lightmaps for a 3D
        /// engine, and I guess other situations as well.
        /// </para>
        /// <para>
        /// The idea of this algorithm is that, as we add rectangles, we can pre-select "interesting"
        /// places where we can try to add the next rectangles. For optimal results, the rectangles
        /// should be added in order. I initially tried using area as a sorting criteria, but it
        /// didn't work well with very tall or very flat rectangles. I then tried using the longest
        /// dimension as a selector, and it worked much better. So much for intuition...
        /// </para>
        /// <para>
        /// These "interesting" places are just to the right and just below the currently added
        /// rectangle. The first rectangle, obviously, goes at the top left, the next one would go
        /// either to the right or below this one, and so on. It is a weird way to do it, but it
        /// seems to work very nicely.
        /// </para>
        /// <para>
        /// The way we search here is fairly brute-force, the fact being that for most offline
        /// purposes the performance seems more than adequate. I have generated a japanese font with
        /// around 8500 characters and all the time was spent generating the bitmaps.
        /// </para>
        /// <para>
        /// Also, for all we care, we could grow the parent rectangle in a different way than power
        /// of two. It just happens that power of 2 is very convenient for graphics hardware textures.
        /// </para>
        /// <para>
        /// I'd be interested in hearing of other approaches to this problem. Make sure to post them
        /// on http://www.flipcode.com
        /// </para>
        /// </remarks>
        internal class ArevaloRectanglePacker : RectanglePacker {

            #region class AnchorRankComparer

            /// <summary>
            /// Compares the 'rank' of anchoring points
            /// </summary>
            /// <remarks>
            /// Anchoring points are potential locations for the placement of new rectangles. Each
            /// time a rectangle is inserted, an anchor point is generated on its upper right end and
            /// another one at its lower left end. The anchor points are kept in a list that is
            /// ordered by their closeness to the upper left corner of the packing area (their
            /// 'rank') so the packer favors positions that are closer to the upper left for new rectangles.
            /// </remarks>
            private class AnchorRankComparer : IComparer<Point> {

                /// <summary>
                /// Provides a default instance for the anchor rank comparer
                /// </summary>
                public static readonly AnchorRankComparer Default = new AnchorRankComparer( );

                #region IComparer<Point> Members

                /// <summary>
                /// Compares the rank of two anchors against each other
                /// </summary>
                /// <param name="left">Left anchor point that will be compared</param>
                /// <param name="right">Right anchor point that will be compared</param>
                /// <returns>The relation of the two anchor point's ranks to each other</returns>
                public int Compare (Point left, Point right) {
                    //return Math.Min(left.X, left.Y) - Math.Min(right.X, right.Y);
                    return (left.X + left.Y) - (right.X + right.Y);
                }

                #endregion IComparer<Point> Members
            }

            #endregion class AnchorRankComparer

            /// <summary>
            /// Anchoring points where new rectangles can potentially be placed
            /// </summary>
            private readonly List<Point> anchors = new List<Point> { new Point(0, 0) };

            /// <summary>
            /// Rectangles contained in the packing area
            /// </summary>
            private readonly List<Rectangle> packedRectangles = new List<Rectangle>( );

            /// <summary>
            /// Current height of the packing area
            /// </summary>
            private int actualPackingAreaHeight = 1;

            /// <summary>
            /// Current width of the packing area
            /// </summary>
            private int actualPackingAreaWidth = 1;

            /// <summary>
            /// Initializes a new rectangle packer
            /// </summary>
            /// <param name="packingAreaWidth">Maximum width of the packing area</param>
            /// <param name="packingAreaHeight">Maximum height of the packing area</param>
            public ArevaloRectanglePacker (int packingAreaWidth, int packingAreaHeight)
                : base(packingAreaWidth, packingAreaHeight) {
            }

            /// <summary>
            /// Tries to allocate space for a rectangle in the packing area
            /// </summary>
            /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
            /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
            /// <param name="placement">Output parameter receiving the rectangle's placement</param>
            /// <returns>True if space for the rectangle could be allocated</returns>
            public override bool TryPack (int rectangleWidth, int rectangleHeight, out Point placement) {
                // Try to find an anchor where the rectangle fits in, enlarging the packing area and
                // repeating the search recursively until it fits or the maximum allowed size is exceeded.
                int anchorIndex = SelectAnchorRecursive(rectangleWidth, rectangleHeight, actualPackingAreaWidth, actualPackingAreaHeight);

                // No anchor could be found at which the rectangle did fit in
                if (anchorIndex == -1) {
                    placement = new Point( );
                    return false;
                }

                placement = anchors[anchorIndex];

                // Move the rectangle either to the left or to the top until it collides with a
                // neightbouring rectangle. This is done to combat the effect of lining up rectangles
                // with gaps to the left or top of them because the anchor that would allow placement
                // there has been blocked by another rectangle
                OptimizePlacement(ref placement, rectangleWidth, rectangleHeight);

                // Remove the used anchor and add new anchors at the upper right and lower left
                // positions of the new rectangle The anchor is only removed if the placement
                // optimization didn't move the rectangle so far that the anchor isn't blocked anymore
                bool blocksAnchor =
                    ((placement.X + rectangleWidth) > anchors[anchorIndex].X) &&
                    ((placement.Y + rectangleHeight) > anchors[anchorIndex].Y);

                if (blocksAnchor)
                    anchors.RemoveAt(anchorIndex);

                // Add new anchors at the upper right and lower left coordinates of the rectangle
                InsertAnchor(new Point(placement.X + rectangleWidth, placement.Y));
                InsertAnchor(new Point(placement.X, placement.Y + rectangleHeight));

                // Finally, we can add the rectangle to our packed rectangles list
                packedRectangles.Add(new Rectangle(placement.X, placement.Y, rectangleWidth, rectangleHeight));

                return true;
            }

            /// <summary>
            /// Locates the first free anchor at which the rectangle fits
            /// </summary>
            /// <param name="rectangleWidth">Width of the rectangle to be placed</param>
            /// <param name="rectangleHeight">Height of the rectangle to be placed</param>
            /// <param name="testedPackingAreaWidth">Total width of the packing area</param>
            /// <param name="testedPackingAreaHeight">Total height of the packing area</param>
            /// <returns>The index of the first free anchor or -1 if none is found</returns>
            private int FindFirstFreeAnchor (int rectangleWidth, int rectangleHeight, int testedPackingAreaWidth, int testedPackingAreaHeight) {
                var potentialLocation = new Rectangle(0, 0, rectangleWidth, rectangleHeight);

                // Walk over all anchors (which are ordered by their distance to the upper left
                // corner of the packing area) until one is discovered that can house the new rectangle.
                for (int index = 0; index < anchors.Count; ++index) {
                    potentialLocation.X = anchors[index].X;
                    potentialLocation.Y = anchors[index].Y;

                    // See if the rectangle would fit in at this anchor point
                    if (IsFree(ref potentialLocation, testedPackingAreaWidth, testedPackingAreaHeight))
                        return index;
                }

                // No anchor points were found where the rectangle would fit in
                return -1;
            }

            /// <summary>
            /// Inserts a new anchor point into the anchor list
            /// </summary>
            /// <param name="anchor">Anchor point that will be inserted</param>
            /// <remarks>
            /// This method tries to keep the anchor list ordered by ranking the anchors depending on
            /// the distance from the top left corner in the packing area.
            /// </remarks>
            private void InsertAnchor (Point anchor) {
                // Find out where to insert the new anchor based on its rank (which is calculated
                // based on the anchor's distance to the top left corner of the packing area).
                //
                // From MSDN on BinarySearch(): "If the List does not contain the specified value,
                // the method returns a negative integer. You can apply the bitwise complement
                // operation (~) to this negative integer to get the index of the first element that
                // is larger than the search value."
                int insertIndex = anchors.BinarySearch(anchor, AnchorRankComparer.Default);
                if (insertIndex < 0)
                    insertIndex = ~insertIndex;

                // Insert the anchor at the index matching its rank
                anchors.Insert(insertIndex, anchor);
            }

            /// <summary>
            /// Determines whether the rectangle can be placed in the packing area at its current location.
            /// </summary>
            /// <param name="rectangle">Rectangle whose position to check</param>
            /// <param name="testedPackingAreaWidth">Total width of the packing area</param>
            /// <param name="testedPackingAreaHeight">Total height of the packing area</param>
            /// <returns>True if the rectangle can be placed at its current position</returns>
            private bool IsFree (ref Rectangle rectangle, int testedPackingAreaWidth, int testedPackingAreaHeight) {
                // If the rectangle is partially or completely outside of the packing area, it can't
                // be placed at its current location
                bool leavesPackingArea = (rectangle.X < 0) || (rectangle.Y < 0) || (rectangle.Right > testedPackingAreaWidth) || (rectangle.Bottom > testedPackingAreaHeight);

                if (leavesPackingArea)
                    return false;

                // Brute-force search whether the rectangle touches any of the other rectangles
                // already in the packing area
                for (int index = 0; index < packedRectangles.Count; ++index) {
                    if (packedRectangles[index].IntersectsWith(rectangle))
                        return false;
                }

                // Success! The rectangle is inside the packing area and doesn't overlap with any
                // other rectangles that have already been packed.
                return true;
            }

            /// <summary>
            /// Optimizes the rectangle's placement by moving it either left or up to fill any gaps
            /// resulting from rectangles blocking the anchors of the most optimal placements.
            /// </summary>
            /// <param name="placement">Placement to be optimized</param>
            /// <param name="rectangleWidth">Width of the rectangle to be optimized</param>
            /// <param name="rectangleHeight">Height of the rectangle to be optimized</param>
            private void OptimizePlacement (ref Point placement, int rectangleWidth, int rectangleHeight) {
                var rectangle = new Rectangle(placement.X, placement.Y, rectangleWidth, rectangleHeight);

                // Try to move the rectangle to the left as far as possible
                int leftMost = placement.X;
                while (IsFree(ref rectangle, PackingAreaWidth, PackingAreaHeight)) {
                    leftMost = rectangle.X;
                    --rectangle.X;
                }

                // Reset rectangle to original position
                rectangle.X = placement.X;

                // Try to move the rectangle upwards as far as possible
                int topMost = placement.Y;
                while (IsFree(ref rectangle, PackingAreaWidth, PackingAreaHeight)) {
                    topMost = rectangle.Y;
                    --rectangle.Y;
                }

                // Use the dimension in which the rectangle could be moved farther
                if ((placement.X - leftMost) > (placement.Y - topMost))
                    placement.X = leftMost;
                else
                    placement.Y = topMost;
            }

            /// <summary>
            /// Searches for a free anchor and recursively enlarges the packing area if none can be found.
            /// </summary>
            /// <param name="rectangleWidth">Width of the rectangle to be placed</param>
            /// <param name="rectangleHeight">Height of the rectangle to be placed</param>
            /// <param name="testedPackingAreaWidth">Width of the tested packing area</param>
            /// <param name="testedPackingAreaHeight">Height of the tested packing area</param>
            /// <returns>
            /// Index of the anchor the rectangle is to be placed at or -1 if the rectangle does not
            /// fit in the packing area anymore.
            /// </returns>
            private int SelectAnchorRecursive (int rectangleWidth, int rectangleHeight, int testedPackingAreaWidth, int testedPackingAreaHeight) {
                // Try to locate an anchor point where the rectangle fits in
                int freeAnchorIndex = FindFirstFreeAnchor(rectangleWidth, rectangleHeight, testedPackingAreaWidth, testedPackingAreaHeight);

                // If a the rectangle fits without resizing packing area (any further in case of a
                // recursive call), take over the new packing area size and return the anchor at
                // which the rectangle can be placed.
                if (freeAnchorIndex != -1) {
                    actualPackingAreaWidth = testedPackingAreaWidth;
                    actualPackingAreaHeight = testedPackingAreaHeight;

                    return freeAnchorIndex;
                }

                // If we reach this point, the rectangle did not fit in the current packing area and
                // our only choice is to try and enlarge the packing area.

                // For readability, determine whether the packing area can be enlarged any further in
                // its width and in its height
                bool canEnlargeWidth = (testedPackingAreaWidth < PackingAreaWidth);
                bool canEnlargeHeight = (testedPackingAreaHeight < PackingAreaHeight);
                bool shouldEnlargeHeight = (!canEnlargeWidth) || (testedPackingAreaHeight < testedPackingAreaWidth);

                // Try to enlarge the smaller of the two dimensions first (unless the smaller
                // dimension is already at its maximum size). 'shouldEnlargeHeight' is true when the
                // height was the smaller dimension or when the width is maxed out.
                if (canEnlargeHeight && shouldEnlargeHeight) {
                    // Try to double the height of the packing area
                    return SelectAnchorRecursive(rectangleWidth, rectangleHeight, testedPackingAreaWidth, Math.Min(testedPackingAreaHeight * 2, PackingAreaHeight));
                }
                if (canEnlargeWidth) {
                    // Try to double the width of the packing area
                    return SelectAnchorRecursive(rectangleWidth, rectangleHeight, Math.Min(testedPackingAreaWidth * 2, PackingAreaWidth), testedPackingAreaHeight);
                }

                // Both dimensions are at their maximum sizes and the rectangle still didn't fit. We
                // give up!
                return -1;
            }
        }

        [Serializable]
        internal class OutOfSpaceException : Exception {

            /// <summary>
            /// Initializes the exception with an error message
            /// </summary>
            /// <param name="message">Error message describing the cause of the exception</param>
            public OutOfSpaceException (string message)
                : base(message) {
            }
        }

        /// <summary>
        /// Base class for rectangle packing algorithms
        /// </summary>
        /// <remarks>
        /// <para>
        /// By uniting all rectangle packers under this common base class, you can easily switch
        /// between different algorithms to find the most efficient or performant one for a given job.
        /// </para>
        /// <para>An almost exhaustive list of packing algorithms can be found here: http://www.csc.liv.ac.uk/~epa/surveyhtml.html</para>
        /// </remarks>
        internal abstract class RectanglePacker {

            /// <summary>
            /// Initializes a new rectangle packer
            /// </summary>
            /// <param name="packingAreaWidth">Width of the packing area</param>
            /// <param name="packingAreaHeight">Height of the packing area</param>
            protected RectanglePacker (int packingAreaWidth, int packingAreaHeight) {
                PackingAreaWidth = packingAreaWidth;
                PackingAreaHeight = packingAreaHeight;
            }

            /// <summary>
            /// Maximum height the packing area is allowed to have
            /// </summary>
            protected int PackingAreaHeight { get; private set; }

            /// <summary>
            /// Maximum width the packing area is allowed to have
            /// </summary>
            protected int PackingAreaWidth { get; private set; }

            /// <summary>
            /// Allocates space for a rectangle in the packing area
            /// </summary>
            /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
            /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
            /// <returns>The location at which the rectangle has been placed</returns>
            public virtual Point Pack (int rectangleWidth, int rectangleHeight) {
                Point point;

                if (!TryPack(rectangleWidth, rectangleHeight, out point))
                    throw new OutOfSpaceException("Rectangle does not fit in packing area");

                return point;
            }

            /// <summary>
            /// Tries to allocate space for a rectangle in the packing area
            /// </summary>
            /// <param name="rectangleWidth">Width of the rectangle to allocate</param>
            /// <param name="rectangleHeight">Height of the rectangle to allocate</param>
            /// <param name="placement">Output parameter receiving the rectangle's placement</param>
            /// <returns>True if space for the rectangle could be allocated</returns>
            public abstract bool TryPack (int rectangleWidth, int rectangleHeight, out Point placement);
        }

        #endregion stolen part
    }
}