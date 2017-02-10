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

using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline;
using NUnit.Framework;

namespace itextsharp.xmlworker.tests.iTextSharp.tool.xml.html {
    /**
 * @author Balder
 *
 */

    internal class HorAndVertScalingTest {
        private static List<IElement> elementList;
        private const string RESOURCES = @"..\..\resources\";

        private class CustomElementHandler : IElementHandler {
            virtual public void Add(IWritable w) {
                elementList.AddRange(((WritableElement) w).Elements());
            }
        }

        [SetUp]
        virtual public void SetUp() {
            LoggerFactory.GetInstance().SetLogger(new SysoLogger(3));
            TextReader bis = File.OpenText(RESOURCES + "/snippets/xfa-hor-vert_snippet.html");
            XMLWorkerHelper helper = XMLWorkerHelper.GetInstance();
            elementList = new List<IElement>();
            helper.ParseXHtml(new CustomElementHandler(), bis);
        }

        [TearDown]
        virtual public void TearDown() {
            elementList = null;
        }

        [Test]
        virtual public void ResolveNumberOfElements() {
            Assert.AreEqual(4, elementList.Count);
        }

        [Test]
        virtual public void ResolveFontSize() {
            Assert.AreEqual(12, elementList[0].Chunks[0].Font.Size, 0);
            Assert.AreEqual(16, elementList[1].Chunks[0].Font.Size, 0);
            Assert.AreEqual(16*1.5, elementList[1].Chunks[2].Font.Size, 0);
            Assert.AreEqual(15, elementList[2].Chunks[0].Font.Size, 0);
            Assert.AreEqual(7.5, elementList[2].Chunks[2].Font.Size, 0);
            Assert.AreEqual(6.375, elementList[2].Chunks[4].Font.Size, 0);
        }

        [Test]
        virtual public void ResolveScaling() {
            Assert.AreEqual(1, elementList[1].Chunks[0].HorizontalScaling, 0);
            Assert.AreEqual(1/1.5f, elementList[1].Chunks[2].HorizontalScaling, 1e-7);
            Assert.AreEqual(1, elementList[2].Chunks[0].HorizontalScaling, 0);
            Assert.AreEqual(1/0.5f, elementList[2].Chunks[2].HorizontalScaling, 0);
            Assert.AreEqual(1/0.5f, elementList[2].Chunks[4].HorizontalScaling, 0);
            Assert.AreEqual(1, elementList[3].Chunks[0].HorizontalScaling, 0);
            Assert.AreEqual(1.5, elementList[3].Chunks[2].HorizontalScaling, 0);
        }
    }
}
