using System.Drawing;

namespace Core
{
    public static class GraphicsUtilities
    {
        private const float arbitraryFontSize = 10;

        public static bool IsFontInstalled(string fontFamilyName, FontStyle fontStyle)
        {
            using (Font fontTester = new Font(fontFamilyName, arbitraryFontSize, fontStyle))
            {
                return fontTester.Name == fontFamilyName;
            }
        }
    }
}
