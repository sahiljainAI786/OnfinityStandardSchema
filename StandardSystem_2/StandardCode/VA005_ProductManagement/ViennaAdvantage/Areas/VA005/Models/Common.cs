using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VA005.Models
{
    public class Common
    {
        public static void ConvertByteArrayToThumbnail(byte[] imageBytes, string imageName)
        {

            try
            {
                //********** Save Original Image ********************
                CreateThumbnail(0, 0, imageBytes, imageName);

                ////*********Create Thumbnail of 320/240 *******************
                //CreateThumbnail(320, 240, imageBytes, imageName);

                ////*********Create Thumbnail of 140/120 *******************
                //CreateThumbnail(140, 120, imageBytes, imageName);

                //*********Create Thumbnail of 46/46 *******************
                CreateThumbnail(46, 46, imageBytes, imageName);

                //*********Create Thumbnail of 32/32 *******************
                //CreateThumbnail(32, 32, imageBytes, imageName);

                ////*********Create Thumbnail of 16/16 *******************
                CreateThumbnail(100, 100, imageBytes, imageName);
            }
            catch
            {
            }
            finally
            {
            }


        }
        private static void CreateThumbnail(int width, int height, byte[] byteArray, string imageName)
        {
            int dimWidth = width;
            int dimHeight = height;
            MemoryStream ms = null;
            try
            {
                // System.Drawing.Image imThumbnailImage;
                Bitmap imThumbnailImage;
                // System.Drawing.Image OriginalImage;
                ms = new MemoryStream();
                // Stream / Write Image to Memory Stream from the Byte Array.
                ms.Write(byteArray, 0, byteArray.Length);
                Bitmap OriginalImage = (Bitmap)Image.FromStream(ms);
                //OriginalImage = System.Drawing.Image.FromStream(ms);
                if (height == 0 && width == 0)
                {
                    OriginalImage.Save(Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles/ProductImages"), imageName));
                    return;
                }
                if (!Directory.Exists((Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles/ProductImages"), "Thumb" + width.ToString() + "x" + height.ToString()))))
                {
                    Directory.CreateDirectory(Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles/ProductImages"), "Thumb" + width.ToString() + "x" + height.ToString()));       //Create Thumbnail Folder if doesnot exists
                }
                // Shrink the Original Image to a thumbnail size.
                int percenetage = 0;
                string filepath = Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles/ProductImages"), "Thumb" + width.ToString() + "x" + height.ToString() + "/" + imageName);
                if (!(OriginalImage.Height < height && OriginalImage.Width < width))
                {
                    if (OriginalImage.Height > OriginalImage.Width)
                    {
                        percenetage = GetPercentage(OriginalImage.Width, OriginalImage.Height);
                        // height = width*100 / percenetage;

                        width = (height * percenetage) / 100;
                        if (width > dimWidth)
                        {
                            width = dimWidth;
                            height = (width * 100) / percenetage;
                        }
                        // height =height+( width*GetPercentage(OriginalImage.Width, OriginalImage.Height))/100;
                        // width = (width * GetPercentage(OriginalImage.Width, OriginalImage.Height)) / 100;
                    }
                    else if (OriginalImage.Height == OriginalImage.Width)
                    {
                        width = height;
                    }
                    else
                    {
                        percenetage = GetPercentage(OriginalImage.Height, OriginalImage.Width);
                        //  width = height*100 / percenetage;

                        height = (width * percenetage) / 100;
                        if (height > dimHeight)
                        {
                            height = dimHeight;
                            width = (dimHeight * 100) / percenetage;
                        }
                        //width =width+(height*GetPercentage(OriginalImage.Height, OriginalImage.Width))/100;
                        //height = (height * GetPercentage(OriginalImage.Height, OriginalImage.Width)) / 100;
                    }
                    // imThumbnailImage = OriginalImage.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                    imThumbnailImage = new Bitmap(OriginalImage, new Size(width, height));
                    imThumbnailImage.Save(filepath);
                }
                else
                {
                    OriginalImage.Save(filepath);
                }


            }
            catch
            {
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
            //return myMS.ToArray();
        }
        private static int GetPercentage(int value, int total)
        {
            return (value * 100) / total;
        }

        public static string DeleteImages(string[] images)
        {
            if (images.Length > 0)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    string imgFormat = images[i].Substring(images[i].LastIndexOf('.'));
                    string filePath = Path.Combine(HostingEnvironment.MapPath("~/TempFiles/ProductImages/Thumb100x100"), images[i].ToString());
                    string filePath2 = Path.Combine(HostingEnvironment.MapPath("~/TempFiles/ProductImages/Thumb46x46"), images[i].ToString());
                    string filePath3 = Path.Combine(HostingEnvironment.MapPath("~/TempFiles/ProductImages"), images[i].ToString());
                    FileInfo file = new FileInfo(filePath);
                    FileInfo file2 = new FileInfo(filePath2);
                    FileInfo file3 = new FileInfo(filePath3);
                    if (file.Exists && file2.Exists && file3.Exists)
                    {
                        file.Delete();
                        file2.Delete();
                        file3.Delete();
                    }
                }
            }
            return "";
        }
        //public static int SaveUserImage(Ctx ctx, byte[] buffer, string imageName, bool isSaveInDB, int productID)
        public static string SaveUserImage(Ctx ctx, byte[] buffer, string imageName, bool isSaveInDB, int productID)
        {
            string imageDataURL = null;
            MProduct user = new MProduct(ctx, productID, null);
            int imageID = Util.GetValueOfInt(user.GetAD_Image_ID());



            //DirectoryInfo dir = new DirectoryInfo(HostingEnvironment.MapPath("~/TempFiles/ProductImages/Thumb100x100"));
            MImage mimg = new MImage(ctx, imageID, null);
            mimg.ByteArray = buffer;
            mimg.ImageFormat = imageName.Substring(imageName.LastIndexOf('.'));
            mimg.SetName(imageName);
            if (isSaveInDB)
            {
                mimg.SetBinaryData(buffer);
                //mimg.SetImageURL(string.Empty);
            }
            //mimg.SetImageURL("Images/Thumb46x46");//Image Saved in File System so instead of byteArray image Url will be set            
            mimg.SetImageURL(mimg.ImageFormat);
            if (!mimg.Save())
            {
                return "";
            }
            user.SetAD_Image_ID(mimg.GetAD_Image_ID());
            if (!user.Save())
            {
                return "";
            }
            else
            {
                string filePath = Path.Combine(HostingEnvironment.MapPath("~/Images/Thumb46x46"), mimg.GetAD_Image_ID() + mimg.ImageFormat);

                if (File.Exists(filePath))
                {
                    byte[] imageByteData = System.IO.File.ReadAllBytes(filePath);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                }
            }
            return imageDataURL;
            //return mimg.GetAD_Image_ID();
        }
    }
}