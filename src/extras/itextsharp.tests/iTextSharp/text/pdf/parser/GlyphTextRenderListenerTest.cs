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

using System;
using itextsharp.tests.iTextSharp.testutils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;

namespace itextsharp.tests.iTextSharp.text.pdf.parser {
    internal class GlyphTextRenderListenerTest {
        private const string TEST_RESOURCES_PATH = @"..\..\resources\text\pdf\parser\GlyphTextRenderListenerTest\";

        [Test]
        virtual public void Test1() {
            PdfReader pdfReader = TestResourceUtils.GetResourceAsPdfReader(TEST_RESOURCES_PATH, "test.pdf");
            PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);

            float x1, y1, x2, y2;

            x1 = 203;
            x2 = 224;
            y1 = 842 - 44;
            y2 = 842 - 93;
            String extractedText =
                parser.ProcessContent(1,
                    new GlyphTextRenderListener(new FilteredTextRenderListener(new LocationTextExtractionStrategy(),
                        new RegionTextRenderFilter(new Rectangle(x1, y1, x2, y2))))).GetResultantText();
            Assert.AreEqual("1234\nt5678", extractedText);
        }

        [Test]
        virtual public void Test2() {
            PdfReader pdfReader = TestResourceUtils.GetResourceAsPdfReader(TEST_RESOURCES_PATH, "Sample.pdf");

            PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);
            String extractedText =
                parser.ProcessContent(1,
                    new GlyphTextRenderListener(new FilteredTextRenderListener(new LocationTextExtractionStrategy(),
                        new RegionTextRenderFilter(new Rectangle(111, 855, 136, 867))))).GetResultantText();

            Assert.AreEqual("Your ", extractedText);
        }

        [Test]
        virtual public void TestWithMultiFilteredRenderListener() {
            PdfReader pdfReader = TestResourceUtils.GetResourceAsPdfReader(TEST_RESOURCES_PATH, "test.pdf");
            PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);

            float x1, y1, x2, y2;

            MultiFilteredRenderListener listener = new MultiFilteredRenderListener();
            x1 = 122;
            x2 = 144;
            y1 = 841.9f - 151;
            y2 = 841.9f - 163;
            ITextExtractionStrategy region1Listener = listener.AttachRenderListener(
                new LocationTextExtractionStrategy(), new RegionTextRenderFilter(new Rectangle(x1, y1, x2, y2)));

            x1 = 156;
            x2 = 169;
            y1 = 841.9f - 151;
            y2 = 841.9f - 163;
            ITextExtractionStrategy region2Listener = listener.AttachRenderListener(
                new LocationTextExtractionStrategy(), new RegionTextRenderFilter(new Rectangle(x1, y1, x2, y2)));

            parser.ProcessContent(1, new GlyphRenderListener(listener));
            Assert.AreEqual("Your", region1Listener.GetResultantText());
            Assert.AreEqual("dju", region2Listener.GetResultantText());
        }
    }
}
