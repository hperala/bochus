using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Core
{
    /// <summary>
    /// A generator for an image that contains a set of strings. The image can be written to a 
    /// graphics context.
    /// </summary>
    public class TitleCard
    {
        private const int stylingKeyLength = 8;

        // Palettes generated with paletton.com using adjancent colors mode
        private readonly Palette[] palettes = new Palette[]
            {
                new Palette(
                    Color.FromArgb(85,  0,  0),
                    Color.FromArgb(170, 57, 57),
                    Color.FromArgb(255, 170, 170),
                    Color.FromArgb(85, 39, 0),
                    Color.FromArgb(170, 108, 57),
                    Color.FromArgb(255, 209, 170),
                    Color.FromArgb(68, 0, 39),
                    Color.FromArgb(136, 45, 97),
                    Color.FromArgb(205, 136, 175)),
                new Palette(
                    Color.FromArgb(85, 70, 0),
                    Color.FromArgb(170, 151, 57),
                    Color.FromArgb(255, 240, 170),
                    Color.FromArgb(85, 85, 0),
                    Color.FromArgb(170, 170, 57),
                    Color.FromArgb(255, 255, 170),
                    Color.FromArgb(85, 57,  0),
                    Color.FromArgb(170, 132, 57),
                    Color.FromArgb(255, 227, 170)),
                new Palette(
                    Color.FromArgb(0, 68,  0),
                    Color.FromArgb(45, 136, 45),
                    Color.FromArgb(136, 204, 136),
                    Color.FromArgb(0, 51, 51),
                    Color.FromArgb(34, 102, 102),
                    Color.FromArgb(102, 153, 153),
                    Color.FromArgb(53, 79, 0),
                    Color.FromArgb(123, 159, 53),
                    Color.FromArgb(212, 238, 159)),
                new Palette(
                    Color.FromArgb(19, 7, 58),
                    Color.FromArgb(64, 48, 117),
                    Color.FromArgb(136, 124, 175),
                    Color.FromArgb(38, 3, 57),
                    Color.FromArgb(88, 42, 114),
                    Color.FromArgb(151, 117, 170),
                    Color.FromArgb(6, 21, 57),
                    Color.FromArgb(46, 66, 114),
                    Color.FromArgb(120, 135, 171))
            };

        /// <summary>
        /// Creates a new TitleCard and generates its layout. To draw the image, use 
        /// <code>DrawBackground</code> and <code>DrawForeground</code>.
        /// </summary>
        /// <param name="graphics">a graphics context for drawing. Typically created from a bitmap.
        /// Size should match the width and height parameters.</param>
        /// <param name="strings">one or more text strings to display on the card</param>
        /// <param name="decorativeInitial">a string (recommended length: one character) used for 
        /// decorative purposes</param>
        /// <param name="stylingKeyString">a seed used for pseudorandom selection of the card 
        /// style</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="foregroundFont">list of fonts (preferred first) to use for foreground 
        /// text</param>
        /// <param name="backgroundFont">list of fonts (preferred first) to use for background text
        /// (decorative initial)</param>
        /// <param name="preferredMaxCharsPerLine">preferred line length. Strings longer than this 
        /// may be wrapped.</param>
        public TitleCard(
            Graphics graphics,
            IEnumerable<string> strings,
            string decorativeInitial,
            string stylingKeyString,
            int width,
            int height,
            IEnumerable<string> foregroundFont = null,
            IEnumerable<string> backgroundFont = null,
            int preferredMaxCharsPerLine = 17)
        {
            if (strings.Count() == 0)
            {
                throw new ArgumentException();
            }
            Graphics = graphics;
            DecorativeInitial = decorativeInitial;
            StylingKey = StringUtilities.StringToKey(stylingKeyString, stylingKeyLength, 'a');
            Width = width;
            Height = height;
            if (foregroundFont == null)
            {
                ForegroundFont = new string[] { "Arial" };
            }
            else
            {
                ForegroundFont = foregroundFont;
            }
            if (backgroundFont == null)
            {
                BackgroundFont = new string[] { "Great Vibes", "Times New Roman" };
            }
            else
            {
                BackgroundFont = backgroundFont;
            }
            PreferredMaxCharsPerLine = preferredMaxCharsPerLine;

            Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            IEnumerable<string> lines = SplitLongStrings(strings);
            CreateStyles(lines);
            CreateLayout();
        }

        public Graphics Graphics { get; }
        public int Width { get; }
        public int Height { get; }
        public IEnumerable<string> ForegroundFont { get; }
        public IEnumerable<string> BackgroundFont { get; }
        public string DecorativeInitial { get; }
        public int StylingKey { get; }
        public List<LayoutSpec> Lines { get; private set; }
        public Color BackgroundPrimaryColor { get; private set; }
        public Color BackgroundSecondaryColor { get; private set; }
        public int PreferredMaxCharsPerLine { get; }

        /// <summary>
        /// Draws the background of the title card to this object's graphics context.
        /// </summary>
        public void DrawBackground()
        {
            Graphics.Clear(BackgroundPrimaryColor);

            string fontFamilyName = FirstAvailableFont(BackgroundFont);
            var family = new FontFamily(fontFamilyName);
            var style = FontStyle.Regular;
            var brush = new SolidBrush(BackgroundSecondaryColor);
            var spec = new LayoutSpec()
            {
                Text = DecorativeInitial,
                FontFamily = family,
                FontStyle = style,
                Color = brush.Color
            };
            var boundingBox = new SizeF(Width, Height);

            float fontSize = 0;
            SizeF stringSize = new SizeF();
            GetMaxFontSize(Graphics, spec, boundingBox, out fontSize, out stringSize);

            using (var font = new Font(family, fontSize, style))
            {
                var x = -0.6F * stringSize.Width;
                var y = 0.3F * stringSize.Height;
                Graphics.DrawString(DecorativeInitial, font, brush, x, y);
            }
        }

        /// <summary>
        /// Draws the foreground text of the title card to this object's graphics context.
        /// </summary>
        public void DrawForeground()
        {
            foreach (var line in Lines)
            {
                using (var font = new Font(line.FontFamily, line.EmSize, line.FontStyle))
                {
                    Graphics.DrawString(line.Text, font, new SolidBrush(line.Color), line.X, line.Y);
                }
            }
        }

        private IEnumerable<string> SplitLongStrings(IEnumerable<string> strings)
        {
            var lines = new List<string>();
            foreach (var inputString in strings)
            {
                lines.AddRange(StringUtilities.SplitString(
                    inputString,
                    PreferredMaxCharsPerLine,
                    ' '));
            }
            return lines;
        }

        private void CreateStyles(IEnumerable<string> lines)
        {
            var lightOnDark = StylingKey % 2 == 0;
            var paletteIndex = StylingKey % palettes.Length;
            var fgColorIndex = StylingKey % Palette.ColorsPerPalette;

            // It is safest to use shades of the same color as the (main) background and 
            // foreground, because some "light" shades in the palettes can be as dark as some 
            // "medium" shades
            var bgPrimaryColorIndex = fgColorIndex;

            var bgSecondaryColorIndex = (StylingKey + 1) % Palette.ColorsPerPalette;

            Palette palette = palettes[paletteIndex];
            Color color = lightOnDark ? palette.Light[fgColorIndex] : palette.Dark[fgColorIndex];

            string fontFamilyName = FirstAvailableFont(ForegroundFont);
            Lines = lines.Select(
                x => new LayoutSpec
                {
                    Text = x,
                    FontFamily = new FontFamily(fontFamilyName),
                    FontStyle = FontStyle.Bold,
                    Color = color,
                    LayoutStyle = LayoutStyle.MaxSize
                }).ToList();
            BackgroundPrimaryColor = palette.Medium[bgPrimaryColorIndex];
            BackgroundSecondaryColor = palette.Medium[bgSecondaryColorIndex];
        }

        private void CreateLayout()
        {
            int index = 0;
            foreach (var line in Lines)
            {
                var boundingBox = new SizeF(Width, Height / (float)Lines.Count);
                float fontSize = 0;
                SizeF stringSize = new SizeF();
                GetMaxFontSize(Graphics, line, boundingBox, out fontSize, out stringSize);

                float x = 0;
                float y = 0;
                GetPosition(stringSize, index, Lines.Count, out x, out y);

                line.EmSize = fontSize;
                line.X = x;
                line.Y = y;

                index++;
            }
        }

        private string FirstAvailableFont(IEnumerable<string> fontFamilyNames)
        {
            var familyName = fontFamilyNames.FirstOrDefault(
                x => GraphicsUtilities.IsFontInstalled(x, FontStyle.Regular));

            if (familyName == null)
            {
                throw new Exception(String.Format(
                    Properties.Resources.ErrFontNotFound, 
                    string.Join(", ", fontFamilyNames)));
            }
            return familyName;
        }

        private void GetMaxFontSize(Graphics graphics, LayoutSpec line, SizeF boundingBox, out float fontSize, out SizeF stringSize)
        {
            float testedFontSize = 5;
            SizeF testedStringSize = GetStringSize(graphics, line, testedFontSize);

            do
            {
                fontSize = testedFontSize;
                stringSize = testedStringSize;

                testedFontSize += 1;
                testedStringSize = GetStringSize(graphics, line, testedFontSize);
            }
            while (testedStringSize.Width < boundingBox.Width && testedStringSize.Height < boundingBox.Height);
        }

        private SizeF GetStringSize(Graphics graphics, LayoutSpec line, float fontEmSize)
        {
            using (var font = new Font(line.FontFamily, fontEmSize, line.FontStyle))
            {
                return graphics.MeasureString(line.Text, font);
            }
        }

        private void GetPosition(SizeF stringSize, int index, int count, out float x, out float y)
        {
            float lineHeight = Height / (float)count;
            float lineCenterY = index * lineHeight + lineHeight / 2;
            float lineTopY = lineCenterY - stringSize.Height / 2;

            x = 0;
            y = lineTopY;
        }
    }

    public enum LayoutStyle
    {
        MaxSize
    }

    public class LayoutSpec
    {
        public string Text { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public float EmSize { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Color Color { get; set; }
        public LayoutStyle LayoutStyle { get; set; }
    }

    public class Palette
    {
        public const int ColorsPerPalette = 3;

        public readonly Color[] Dark;
        public readonly Color[] Medium;
        public readonly Color[] Light;

        public Palette(
            Color firstDark,
            Color firstMedium,
            Color firstLight,
            Color secondDark,
            Color secondMedium,
            Color secondLight,
            Color thirdDark,
            Color thirdMedium,
            Color thirdLight)
        {
            Dark = new Color[ColorsPerPalette] { firstDark, secondDark, thirdDark };
            Medium = new Color[ColorsPerPalette] { firstMedium, secondMedium, thirdMedium };
            Light = new Color[ColorsPerPalette] { firstLight, secondLight, thirdLight };
        }
    }
}
