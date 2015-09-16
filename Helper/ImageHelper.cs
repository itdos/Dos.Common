using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Dos.Common
{
    /// <summary>
    /// 图片水印处理类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 生成缩略图的模式， WH-指定宽高缩放（可能变形） W-指定宽，高按比例  H-指定高，宽按比例 CUT-指定高宽裁减（不变形,推荐用这个）。
        /// </summary>
        public enum ThumbnailModeOption : byte
        {
            /// <summary>
            /// 指定宽高缩放（可能变形）
            /// </summary>
            WH,
            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            W,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            H,
            /// <summary>
            /// 指定高宽裁减（不变形,推荐用这个）
            /// </summary>
            CUT
        }

        /// <summary>
        /// 加图片水印的位置，TopLeft-左上角 TopCenter-上中间 TopRight-右上角 BottomLeft-左下角 BottomCenter-下中间 右下角-右下角 Middle-正中间。
        /// </summary>
        public enum WaterPositionOption : byte
        {
            /// <summary>
            /// 左上角
            /// </summary>
            LeftTop,
            /// <summary>
            /// 上中间
            /// </summary>
            CenterTop,
            /// <summary>
            /// 右上角
            /// </summary>
            RightTop,
            /// <summary>
            /// 左下角
            /// </summary>
            LeftBottom,
            /// <summary>
            /// 下中间
            /// </summary>
            CenterBottom,
            /// <summary>
            /// 右下角
            /// </summary>
            RightBottom,
            /// <summary>
            /// 正中间
            /// </summary>
            Middle
        }

        /// <summary>
        /// 获取图片格式。
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string fileName)
        {
            string extension = fileName.Substring(fileName.LastIndexOf(".")).Trim().ToLower();

            switch (extension)
            {
                case ".jpg":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ".jpeg":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ".gif":
                    return System.Drawing.Imaging.ImageFormat.Gif;
                case ".png":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case ".bmp":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                case ".ico":
                    return System.Drawing.Imaging.ImageFormat.Icon;
                default:
                    goto case ".jpg";
            }
        }

        /// <summary>
        /// 加水印图片并保存。
        /// </summary>
        /// <param name="originalImageStream">Stream</param>
        /// <param name="strFileName">源图路径（物理路径）</param>
        /// <param name="savePath">图片保存路径（物理路径）</param>
        /// <param name="waterPath">水印图路径（物理路径）</param>
        /// <param name="edge">水印图离原图边界的距离</param>
        /// <param name="position">加图片水印的位置</param>
        /// <returns>是否成功</returns>
        public static bool MakeWaterImage(Stream originalImageStream, string strFileName, string savePath, string waterPath, int edge, WaterPositionOption position)
        {
            bool success = false;

            int x = 0;
            int y = 0;
            Image waterImage = null;
            Image image = null;
            Bitmap bitmap = null;
            Graphics graphics = null;

            try
            {
                //加载原图
                image = Image.FromStream(originalImageStream);
                //加载水印图
                waterImage = Image.FromFile(waterPath);
                bitmap = new Bitmap(image);
                graphics = Graphics.FromImage(bitmap);

                int newEdge = edge;
                if (newEdge >= image.Width + waterImage.Width) newEdge = 10;

                switch (position)
                {
                    case WaterPositionOption.LeftTop:
                        x = newEdge;
                        y = newEdge;
                        break;
                    case WaterPositionOption.CenterTop:
                        x = (image.Width - waterImage.Width) / 2;
                        y = newEdge;
                        break;
                    case WaterPositionOption.RightTop:
                        x = image.Width - waterImage.Width - newEdge;
                        y = newEdge;
                        break;
                    case WaterPositionOption.LeftBottom:
                        x = newEdge;
                        y = image.Height - waterImage.Height - newEdge;
                        break;
                    case WaterPositionOption.CenterBottom:
                        x = (image.Width - waterImage.Width) / 2;
                        y = image.Height - waterImage.Height - newEdge;
                        break;
                    case WaterPositionOption.RightBottom:
                        x = image.Width - waterImage.Width - newEdge;
                        y = image.Height - waterImage.Height - newEdge;
                        break;
                    case WaterPositionOption.Middle:
                        x = (image.Width - waterImage.Width) / 2;
                        y = (image.Height - waterImage.Height) / 2;
                        break;
                    default:
                        goto case WaterPositionOption.RightBottom;
                }

                // 画水印图片
                graphics.DrawImage(waterImage, new Rectangle(x, y, waterImage.Width, waterImage.Height), 0, 0, waterImage.Width, waterImage.Height, GraphicsUnit.Pixel);

                // 关闭打开着的文件并保存（覆盖）新图片
                originalImageStream.Close();
                bitmap.Save(savePath, GetImageFormat(strFileName));

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw;
                //throw new Exception(ex.Message.Replace("'", " ").Replace("\n", " ").Replace("\\", "/"));
            }
            finally
            {
                if (graphics != null) graphics.Dispose();
                if (bitmap != null) bitmap.Dispose();
                if (image != null) image.Dispose();
                if (waterImage != null) waterImage.Dispose();
            }

            return success;
        }

        /// <summary>
        /// 生成缩略图并保存。
        /// </summary>
        /// <param name="originalImageStream">Stream</param>
        /// <param name="strFileName">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="maxWidth">缩略图最大宽度</param>
        /// <param name="maxheight">缩略图最大高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        /// <returns>是否成功</returns>
        public static bool MakeThumbnail(Stream originalImageStream, string strFileName, string thumbnailPath, int maxWidth, int maxheight, ThumbnailModeOption mode)
        {
            bool success = false;

            int x = 0;
            int y = 0;
            int toW = maxWidth;
            int toH = maxheight;
            Image image = null;
            Image bitmap = null;
            Graphics graphics = null;
            try
            {
                image = Image.FromStream(originalImageStream);
                int w = image.Width;
                int h = image.Height;

                switch (mode)
                {
                    case ThumbnailModeOption.WH:
                        break;
                    case ThumbnailModeOption.W:
                        if (w < maxWidth)
                        {
                            toW = w;
                            toH = h;
                        }
                        else
                        {
                            toH = h * maxWidth / w;
                        }
                        break;
                    case ThumbnailModeOption.H:
                        if (h < maxheight)
                        {
                            toW = w;
                            toH = h;
                        }
                        else
                        {
                            toW = w * maxheight / h;
                        }
                        break;
                    case ThumbnailModeOption.CUT:
                        if (((double)w / (double)h) > ((double)toW / (double)toH))
                        {
                            w = h * toW / toH;
                            y = 0;
                            x = (image.Width - w) / 2;
                        }
                        else
                        {
                            h = w * toH / toW;
                            x = 0;
                            y = (image.Height - h) / 2;
                        }
                        break;
                    default:
                        goto case ThumbnailModeOption.CUT;
                }

                bitmap = new Bitmap(toW, toH);
                graphics = Graphics.FromImage(bitmap);
                graphics.InterpolationMode = InterpolationMode.High;   //设置高质量,低速度呈现平滑程度
                graphics.SmoothingMode = SmoothingMode.HighQuality;    //清空画布并以透明背景色填充
                graphics.Clear(Color.Transparent);

                // 在指定位置并且按指定大小绘制原图片的指定部分
                graphics.DrawImage(image, new Rectangle(0, 0, toW, toH), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
                // 保存缩略图
                bitmap.Save(thumbnailPath, GetImageFormat(strFileName));
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw;
            }
            finally
            {
                if (graphics != null) graphics.Dispose();
                if (bitmap != null) bitmap.Dispose();
                if (image != null) image.Dispose();
            }
            return success;
        }
        /// <summary>
        /// 生成缩略图并打水印再保存。
        /// </summary>
        /// <param name="originalImageStream">Stream</param>
        /// <param name="strFileName">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="maxWidth">缩略图最大宽度</param>
        /// <param name="maxheight">缩略图最大高度</param>
        /// <param name="mode">生成缩略图的方式</param>
        /// <param name="waterPath">水印图路径（物理路径）</param>
        /// <param name="edge">水印图离原图边界的距离</param>
        /// <param name="position">加图片水印的位置</param>
        /// <returns>是否成功</returns>
        public static bool MakeThumbnailWater(Stream originalImageStream, string strFileName, string thumbnailPath, int maxWidth, int maxheight, ThumbnailModeOption mode, string waterPath, int edge, WaterPositionOption position)
        {
            bool success = false;
            try
            {
                // 生成缩略图
                MakeThumbnail(originalImageStream, strFileName, thumbnailPath, maxWidth, maxheight, mode);
                Stream stream = File.Open(thumbnailPath, FileMode.Open);
                //打水印
                MakeWaterImage(stream, strFileName, thumbnailPath, waterPath, edge, position);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw;
            }
            return success;
        }
    }
}