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
    internal class MultiFilteredRenderListenerTest {
        private string TEST_RESOURCES_PATH = @"..\..\resources\text\pdf\parser\MultiFilteredRenderListenerTest\";

        [Test]
        virtual public void Test() {
            PdfReader pdfReader = TestResourceUtils.GetResourceAsPdfReader(TEST_RESOURCES_PATH, "test.pdf");

            String[] expectedText = new String[] {
                "PostScript Compatibility",
                "Because the PostScript language does not support the transparent imaging \n" +
                "model, PDF 1.4 consumer applications must have some means for converting the \n" +
                "appearance of a document that uses transparency to a purely opaque description \n" +
                "for printing on PostScript output devices. Similar techniques can also be used to \n" +
                "convert such documents to a form that can be correctly viewed by PDF 1.3 and \n" +
                "earlier consumers. ",
                "Otherwise, flatten the colors to some assumed device color space with pre-\n" +
                "determined calibration. In the generated PostScript output, paint the flattened \n" +
                "colors in a CIE-based color space having that calibration. "
            };

            Rectangle[] regions = new Rectangle[] {
                new Rectangle(90, 605, 220, 581),
                new Rectangle(80, 578, 450, 486), new Rectangle(103, 196, 460, 143)
            };

            RegionTextRenderFilter[] regionFilters = new RegionTextRenderFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++)
                regionFilters[i] = new RegionTextRenderFilter(regions[i]);


            MultiFilteredRenderListener listener = new MultiFilteredRenderListener();
            LocationTextExtractionStrategy[] extractionStrategies = new LocationTextExtractionStrategy[regions.Length];
            for (int i = 0; i < regions.Length; i++)
                extractionStrategies[i] =
                    (LocationTextExtractionStrategy)
                        listener.AttachRenderListener(new LocationTextExtractionStrategy(), regionFilters[i]);

            new PdfReaderContentParser(pdfReader).ProcessContent(1, listener);

            for (int i = 0; i < regions.Length; i++) {
                String actualText = extractionStrategies[i].GetResultantText();
                Assert.AreEqual(expectedText[i], actualText);
            }
        }

        [Test]
        virtual public void MultipleFiltersForOneRegionTest() {
            PdfReader pdfReader = TestResourceUtils.GetResourceAsPdfReader(TEST_RESOURCES_PATH, "test.pdf");

            Rectangle[] regions = new Rectangle[] {
                new Rectangle(0, 0, 500, 650),
                new Rectangle(0, 0, 400, 400), new Rectangle(200, 200, 500, 600), new Rectangle(100, 100, 450, 400)
            };

            RegionTextRenderFilter[] regionFilters = new RegionTextRenderFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++)
                regionFilters[i] = new RegionTextRenderFilter(regions[i]);

            MultiFilteredRenderListener listener = new MultiFilteredRenderListener();
            LocationTextExtractionStrategy extractionStrategy =
                (LocationTextExtractionStrategy)
                    listener.AttachRenderListener(new LocationTextExtractionStrategy(), regionFilters);
            new PdfReaderContentParser(pdfReader).ProcessContent(1, listener);
            String actualText = extractionStrategy.GetResultantText();

            String expectedText = PdfTextExtractor.GetTextFromPage(pdfReader, 1,
                new FilteredTextRenderListener(new LocationTextExtractionStrategy(), regionFilters));

            Assert.AreEqual(expectedText, actualText);
        }
    }
}
