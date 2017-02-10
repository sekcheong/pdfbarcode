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
using System.IO;
using iTextSharp.testutils;
using Microsoft.XmlDiffPatch;
using NUnit.Framework;

namespace itextsharp.tests.iTextSharp.testutils {
    class CompareToolTest {

        private const string TEST_RESOURCES_PATH = @"..\..\resources\testutils\CompareToolTest\";
        private const string OUT_FOLDER = @"CompareToolTest\";

        [SetUp]
        public void SetUp() {
           new DirectoryInfo(OUT_FOLDER).Create();
        }

        [Test]
        public void CompareToolErrorReportTest01() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            compareTool.SetXmlReportName("report01");
            String outPdf = TEST_RESOURCES_PATH + "simple_pdf.pdf";
            String cmpPdf = TEST_RESOURCES_PATH + "cmp_simple_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, OUT_FOLDER, "difference");
            Assert.NotNull("CompareTool must return differences found between the files", result);
            Assert.IsTrue(CompareXmls(TEST_RESOURCES_PATH + "cmp_report01.xml", OUT_FOLDER + "report01.xml"), "CompareTool report differs from the reference one");
            Console.WriteLine(result);
        }

        [Test]
        public void CompareToolErrorReportTest02() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            compareTool.SetXmlReportName("report02");
            String outPdf = TEST_RESOURCES_PATH + "tagged_pdf.pdf";
            String cmpPdf = TEST_RESOURCES_PATH + "cmp_tagged_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, OUT_FOLDER, "difference");
            Assert.NotNull("CompareTool must return differences found between the files",result);
            Assert.IsTrue(CompareXmls(TEST_RESOURCES_PATH + "cmp_report02.xml", OUT_FOLDER + "report02.xml"), "CompareTool report differs from the reference one");
            Console.WriteLine(result);
        }

        [Test]
        public void CompareToolErrorReportTest03() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            compareTool.SetXmlReportName("report03");
            String outPdf = TEST_RESOURCES_PATH + "screenAnnotation.pdf";
            String cmpPdf = TEST_RESOURCES_PATH + "cmp_screenAnnotation.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, OUT_FOLDER, "difference");
            Assert.NotNull("CompareTool must return differences found between the files", result);
            Assert.IsTrue(CompareXmls(TEST_RESOURCES_PATH + "cmp_report03.xml", OUT_FOLDER + "report03.xml"), "CompareTool report differs from the reference one");
            Console.WriteLine(result);
        }
        

        virtual protected bool CompareXmls(String xml1, String xml2) {
            XmlDiff xmldiff = new XmlDiff(XmlDiffOptions.None);
            return xmldiff.Compare(xml1, xml2, false);
        }
    }
}
