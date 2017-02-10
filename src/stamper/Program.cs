using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;

namespace stamper
{
	class Program
	{
		static void manipulatePdf(string src, string dest, string imageFile)
		{
			PdfReader reader = new PdfReader(src);

			PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create, FileAccess.Write));
			//GetInstance(System.Drawing.Image image, BaseColor color, bool forceBW) {
			
			System.Drawing.Image img = System.Drawing.Image.FromFile(imageFile);
			iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img, iTextSharp.text.BaseColor.WHITE, false);
			
			PdfImage stream = new PdfImage(image, "", null);

			stream.Put(new PdfName("ITXT_SpecialId"), new PdfName("123456789"));

			PdfIndirectObject reference = stamper.Writer.AddToBody(stream);
			image.DirectReference = reference.IndirectReference;			
			image.ScaleAbsolute(133, 100);
			iTextSharp.text.Rectangle pagesize = reader.GetPageSizeWithRotation(1);
			image.SetAbsolutePosition(0, pagesize.Height - 100);
			//image.ScaleAbsolute(new iTextSharp.text.Rectangle(
			PdfContentByte over = stamper.GetOverContent(1);
			over.AddImage(image);
			stamper.Close();
			reader.Close();
		}

		static void Main(string[] args)
		{
			manipulatePdf("source.pdf", "out.pdf", "cat.jpg");
		}
	}
}
