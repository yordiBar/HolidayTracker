//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

//namespace HolidayTracker.Models.DTO
//{
//    public class CompanyImageDTO
//    {
//        public string LogoImage { get; set; }
//        public string Name { get; set; }
//    }

//    public static CompanyImageDTO GetCompanyImage(string basepath, int height, int width)
//    {
//        CompanyImageDTO value = new CompanyImageDTO();
//        value.LogoImage = basepath + "images/ht_logo.png";

//        // get company image from _context
//        Company company = _context.company > where(id => current user id)//get company info from current users company Id 
//            if (company != null)
//        {
//            byte[] companylogo = company.CompanyLogoFile;
//            if (companylogo != null)
//            {
//                value.LogoImage = ImageLibrary.ImageArrayToSrc(companylogo, width, height);
//            }
//            value.Name = company.Name;
//        }
//        return value;
//    }

//    public class ImageLibrary
//    {
//        public static string ImageArrayToSrc(byte[] Img, int newWidth, int newHeight)
//        {
//            return ResizePNGImage(Img, newWidth, newHeight);
//        }

//        private static string ResizePNGImage(byte[] Img, int newWidth, int newHeight)
//        {
//            try
//            {
//                Image i = Image.FromStream(new MemoryStream(Img));

//                Bitmap newImage = new Bitmap(newWidth, newHeight);
//                using (Graphics gr = Graphics.FromImage(newImage))
//                {
//                    gr.SmoothingMode = SmoothingMode.HighQuality;
//                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
//                    gr.DrawImage(i, new Rectangle(0, 0, newWidth, newHeight));
//                }

//                using (MemoryStream ms = new MemoryStream())
//                {
//                    try
//                    {
//                        ms.Position = 0;
//                        Bitmap bm = new Bitmap(newImage);
//                        bm.Save(ms, ImageFormat.Png);
//                    }
//                    catch (Exception ex)
//                    {
//                        // AddEventLog("failed to write png to stream trying Gif" + ExceptionLibrary.ExceptionToString(ex), EventLogEntryType.Error);
//                        ms.Position = 0;
//                        try
//                        {
//                            newImage.Save(ms, ImageFormat.Gif);
//                        }
//                        catch (Exception ex1)
//                        {
//                            //AddEventLog("failed to write Gif to stream trying jpeg" + ExceptionLibrary.ExceptionToString(ex1), EventLogEntryType.Error);
//                            ms.Position = 0;
//                            try
//                            {
//                                newImage.Save(ms, ImageFormat.Jpeg);
//                            }
//                            catch (Exception ex2)
//                            {
//                                //AddEventLog("failed to write jpeg to stream trying bmp" + ExceptionLibrary.ExceptionToString(ex2), EventLogEntryType.Error);
//                                ms.Position = 0;

//                                newImage.Save(ms, ImageFormat.Bmp);

//                            }
//                        }
//                    }
//                    byte[] check = ms.ToArray();
//                    return "data:image/png;base64," + Convert.ToBase64String(check);
//                }
//            }
//            catch (Exception ex)
//            {
//                //AddEventLog(ExceptionLibrary.ExceptionToString(ex), EventLogEntryType.Error);
//            }
//            return "";
//        }
//    }
//}
