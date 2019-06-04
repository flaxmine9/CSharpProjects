using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Parse_VK.Bot
{
    public class MyPicture
    {
        Point point { get; set; }
        Rectangle rectangle { get; set; }
        Bitmap bitmap { get; set; }
        ITakesScreenshot ssdriver { get; set; }
        ChromeDriver chrome { get; set; }
        string path { get; set; }

        public MyPicture(ChromeDriver chromeT, string pathT)
        {
            point = new Point();
            path = pathT;          
            chrome = chromeT;
            ssdriver = chrome as ITakesScreenshot;
        }

        public void DownloadPictures(IWebElement web_element, string id, int coordinate_y)
        {          
            List<IWebElement> locations = (web_element.FindElements(By.CssSelector("div[class='page_post_sized_thumbs  clear_fix']"))).ToList();
            if (locations.Any())
            {
                ScreenShotPage(id);
                int x = locations.First().Location.X;
                int y = locations.First().Location.Y - coordinate_y;
                if (x != 0 && y != 0)
                {
                    rectangle = new Rectangle(x, y, locations.First().Size.Width, locations.First().Size.Height - 10);
                    bitmap = new Bitmap(path + @"\" + id + ".png");
                    CropImage(bitmap, rectangle, id);
                }
            }
        }

        private void CropImage(Bitmap source, Rectangle section, string namefile)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            bmp.Save(@"E:\CutPage\" + namefile + ".png", ImageFormat.Png);
        }


        private void ScreenShotPage(string namefile)
        {
            Screenshot screenshot = ssdriver.GetScreenshot();
            Screenshot tempImage = screenshot;

            tempImage.SaveAsFile(path + @"\" + namefile + ".png", ScreenshotImageFormat.Png);
        }
    }
}
