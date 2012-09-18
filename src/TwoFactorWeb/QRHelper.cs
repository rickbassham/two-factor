using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Controls;

namespace TwoFactorWeb
{
    public static class QRHelper
    {
        /// <summary>
        /// Generates an img tag with a data uri encoded image of the QR code from the content given.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IHtmlString QRCode(this HtmlHelper html, string content)
        {
            QrEncoder enc = new QrEncoder(ErrorCorrectionLevel.H);
            var code = enc.Encode(content);

            Renderer r = new Renderer(5, Brushes.Black, Brushes.White);

            using (MemoryStream ms = new MemoryStream())
            {
                r.WriteToStream(code.Matrix, ms, ImageFormat.Png);

                byte[] image = ms.ToArray();

                return html.Raw(string.Format(@"<img src=""data:image/png;base64,{0}"" alt=""{1}"" />", Convert.ToBase64String(image), content));
            }
        }
    }
}