using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System;

namespace FSDP.Utilities
{
    public class ImageUtility
    {
        /// <summary>
        /// Saves provided image as two separate files: full-sized and thumbnail versions.
        /// </summary>
        /// <param name="savePath">File path on this machine for where to save the new files</param>
        /// <param name="fileName">Name of the base file</param>
        /// <param name="image">Image to be resized</param>
        /// <param name="maxImgSize">Largest size (width or height) to use for full-sized image</param>
        /// <param name="maxThumbSize">Largest size (width or height) to use for smaller, thumbnail image</param>
        public static void ResizeImage(string savePath, string fileName, Image image, int maxImgSize, int maxThumbSize)
        {
            int[] newImageSizes = GetNewSize(image.Width, image.Height, maxImgSize);
            Bitmap newImage = DoResizeImage(newImageSizes[0], newImageSizes[1], image);
            newImage.Save(savePath + fileName);
            int[] newThumbSizes = GetNewSize(newImage.Width, newImage.Height, maxThumbSize);
            Bitmap newThumb = DoResizeImage(newThumbSizes[0], newThumbSizes[1], image);
            newThumb.Save(savePath + "t_" + fileName);
            newImage.Dispose(); newThumb.Dispose(); image.Dispose();
        }

        /// <summary>
        /// Figure out new image size based on input parameters.
        /// </summary>
        /// <param name="imgWidth">Current image width</param>
        /// <param name="imgHeight">Current image height</param>
        /// <param name="maxImgSize">Desired maximum size (width OR height)</param>
        /// <returns></returns>
        public static int[] GetNewSize(int imgWidth, int imgHeight, int maxImgSize)
        {
            float ratioX = (float)maxImgSize / (float)imgWidth;
            float ratioY = (float)maxImgSize / (float)imgHeight;
            float ratio = Math.Min(ratioX, ratioY);
            int[] newImgSizes = new int[2];
            newImgSizes[0] = (int)(imgWidth * ratio);
            newImgSizes[1] = (int)(imgHeight * ratio);
            return newImgSizes;
        }

        /// <summary>
        /// Perform image resize.
        /// </summary>
        /// <param name="imgWidth">Desired width</param>
        /// <param name="imgHeight">Desired height</param>
        /// <param name="image">Image to be resized</param>
        /// <returns></returns>
        public static Bitmap DoResizeImage(int imgWidth, int imgHeight, Image image)
        {
            Bitmap newImage = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
            newImage.MakeTransparent();
            newImage.SetResolution(72, 72);
            // Do the resize
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, imgWidth, imgHeight);
            }
            return newImage;
        }

        /// <summary>
        /// Deletes designated file (and its thumbnail, if appropriate). Skips "default image" file.
        /// </summary>
        /// <param name="path">File path on this machine for where to delete designated file(s)</param>
        /// <param name="fileName">Name of the base file to be deleted</param>
        public static void Delete(string path, string fileName)
        {
            if (fileName.ToLower() == "noimage.png")
            {
                return;
            }

            FileInfo baseFile = new FileInfo(path + fileName);
            FileInfo thumbImg = new FileInfo(path + "t_" + fileName);

            if (baseFile.Exists)
            {
                baseFile.Delete();
            }

            if (thumbImg.Exists)
            {
                thumbImg.Delete();
            }
        }
    }
}
