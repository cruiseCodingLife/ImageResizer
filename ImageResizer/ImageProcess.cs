using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcess
    {
        //-----------------------#同步Sync--------------------------------
        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }
        //-----------------------#同步Sync--------------------------------

        //-----------------------#非同步Async--------------------------------

        public void CleanAsync(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);
                List<Task> tasks = new List<Task>();
                foreach (var item in allImageFiles)
                {
                    Task taskDeleteImages = Task.Run(async () => await todoDeleteImages(item));
                    tasks.Add(taskDeleteImages);
                    
                }
                Task.WhenAll(tasks).Wait();
            }
        }
        public async Task todoDeleteImages(string item)
        {
            File.Delete(item);
        }
        //-----------------------#非同步Async--------------------------------

        //-----------------------#同步Sync--------------------------------
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale)
        {
            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                using (Image imgPhoto = Image.FromFile(filePath))
                {
                    string imgName = Path.GetFileNameWithoutExtension(filePath);

                    int sourceWidth = imgPhoto.Width;
                    int sourceHeight = imgPhoto.Height;

                    int destionatonWidth = (int)(sourceWidth * scale);
                    int destionatonHeight = (int)(sourceHeight * scale);

                    using (Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight))
                    {
                        string destFile = Path.Combine(destPath, imgName + ".jpg");
                        processedImage.Save(destFile, ImageFormat.Jpeg);
                    }

                }
            }
        }

        //-----------------------#同步Sync--------------------------------
        //-----------------------#非同步Async--------------------------------
        public void ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            List<Task> tasks = new List<Task>();
            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                Task taskResizeImages = Task.Run(async () => await todoResizeImages(filePath, destPath, scale));
                tasks.Add(taskResizeImages);
            }
            Task.WhenAll(tasks).Wait();
        }
        public async Task todoResizeImages(string filePath, string destPath, double scale)
        {
            using (Image imgPhoto = Image.FromFile(filePath))
            {
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                using (Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight))
                {
                    string destFile = Path.Combine(destPath, imgName + ".jpg");
                    processedImage.Save(destFile, ImageFormat.Jpeg);
                }
            }
        }
        //-----------------------#非同步Async--------------------------------

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        Bitmap processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {

            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(resizedbitmap))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);

                g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
                return resizedbitmap;
            }  
        }
    }
}
