using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Test
{
    [TestClass()]
    public class TitleCardTests
    {
        TitleCard titleCard;
        Bitmap bitmap;
        List<string> textStrings;
        int width;
        int height;
        string initial;
        string keyString;
        List<string> fgFontList;
        List<string> bgFontList;
        int lineLength;

        [TestInitialize()]
        public void Initialize()
        {
            width = 200;
            height = 100;
            bitmap = new Bitmap(width, height);
            textStrings = new List<string>() { "Firstname Middlename Lastname", "A Multi-Word Title" };
            initial = "L";
            keyString = "Lastname";
            fgFontList = new List<string>() { "Calibri", "Arial" };
            bgFontList = new List<string>() { "Palatino", "Times New Roman" };
            lineLength = 40;

            titleCard = CreateTitleCard();
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            Assert.IsNotNull(titleCard.Graphics);
            Assert.AreEqual(initial, titleCard.DecorativeInitial);
            Assert.IsTrue(titleCard.StylingKey > 0);
            Assert.AreEqual(width, titleCard.Width);
            Assert.AreEqual(height, titleCard.Height);
            CollectionAssert.AreEqual(fgFontList, titleCard.ForegroundFont.ToList());
            CollectionAssert.AreEqual(bgFontList, titleCard.BackgroundFont.ToList());
            Assert.AreEqual(lineLength, titleCard.PreferredMaxCharsPerLine);
        }

        TitleCard CreateTitleCard()
        {
            return new TitleCard(
                Graphics.FromImage(bitmap),
                textStrings,
                initial,
                keyString,
                width,
                height,
                fgFontList,
                bgFontList,
                lineLength);
        }
    }
}