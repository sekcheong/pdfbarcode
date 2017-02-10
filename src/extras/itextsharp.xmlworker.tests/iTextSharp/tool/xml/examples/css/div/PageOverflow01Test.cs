/*
 * $Id$
 *
 * This file is part of the iText (R) project.
 * Copyright (c) 1998-2016 iText Group NV
 * Authors: Bruno Lowagie, Paulo Soares, et al.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License version 3
 * as published by the Free Software Foundation with the addition of the
 * following permission added to Section 15 as permitted in Section 7(a):
 * FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
 * ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
 * OF THIRD PARTY RIGHTS
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see http://www.gnu.org/licenses or write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License,
 * a covered work must retain the producer line in every PDF that is created
 * or manipulated using iText.
 *
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the iText software without
 * disclosing the source code of your own applications.
 * These activities include: offering paid services to customers as an ASP,
 * serving PDFs on the fly in a web application, shipping iText with a closed
 * source product.
 *
 * For more information, please contact iText Software Corp. at this
 * address: sales@itextpdf.com
 */

using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace itextsharp.xmlworker.tests.iTextSharp.tool.xml.examples.css.div {
    public class PageOverflow01Test : SampleTest {
        protected override string GetTestName() {
            return "pageOverflow01";
        }

        protected override void MakePdf(string outPdf) {
            Document doc = new Document(PageSize.A4.Rotate());
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(outPdf, FileMode.Create));
            doc.Open();
            TransformHtml2Pdf(doc, pdfWriter, new SampleTestImageProvider(), new XMLWorkerFontProvider(RESOURCES + @"tool\xml\examples\fonts"),
                new FileStream(RESOURCES + @"tool\xml\examples\sampleTest.css", FileMode.Open, FileAccess.Read, FileShare.Read));
            doc.Close();
        }

        protected override void TransformHtml2Pdf(Document doc, PdfWriter pdfWriter, IImageProvider imageProvider,
            IFontProvider fontProvider, Stream cssFile) {
            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);

            HtmlPipelineContext hpc;

            if (fontProvider != null)
                hpc = new HtmlPipelineContext(new CssAppliersImpl(fontProvider));
            else
                hpc = new HtmlPipelineContext(null);

            hpc.SetImageProvider(imageProvider);
            hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(Tags.GetHtmlTagProcessorFactory());
            HtmlPipeline htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, pdfWriter));
            IPipeline pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
            XMLWorker worker = new XMLWorker(pipeline, true);
            XMLParser xmlParse = new XMLParser(true, worker, Encoding.UTF8);
            xmlParse.Parse(new FileStream(inputHtml, FileMode.Open), Encoding.UTF8);
        }
    }
}
