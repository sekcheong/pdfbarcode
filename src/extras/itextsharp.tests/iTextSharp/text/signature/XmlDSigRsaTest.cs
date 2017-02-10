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
using System.Security.Cryptography;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace itextsharp.tests.resources.text.signature
{
    [TestFixture]
    public class XmlDSigRsaTest : XmlDSigTest {

        public const String KeyStore = @"..\..\resources\text\pdf\signature\ds-rsa\key";
        public const String Src = @"..\..\resources\text\pdf\signature\xfa.pdf";
        public const String CmpDir = @"..\..\resources\text\pdf\signature\ds-rsa\";
        public const String DestDir = @"signatures\ds-rsa\";

        RSA publicKey;
        AsymmetricKeyParameter privateKey;

        [SetUp]
        virtual public void LoadKey() {
            string import = "";
            using(StreamReader streamReader = new StreamReader(KeyStore))
                import = streamReader.ReadToEnd();

            publicKey = new RSACryptoServiceProvider();
            publicKey.FromXmlString(import);

            AsymmetricCipherKeyPair cipherKeyPair = DotNetUtilities.GetRsaKeyPair(publicKey.ExportParameters(true));
            privateKey = cipherKeyPair.Private;
            publicKey.ImportParameters(publicKey.ExportParameters(false));
            
            Directory.CreateDirectory(DestDir);
        }


        [Test]
        virtual public void XmlDSigRSAWithPublicKey() {

            String filename = "xfa.signed.pk.pdf";
            String output = DestDir + filename;
            
            SignWithPublicKey(Src, output, privateKey, publicKey, DigestAlgorithms.SHA1);

            String cmp = SaveXmlFromResult(output);

            Assert.IsTrue(VerifySignature(cmp), "XmlDSig Verification");

            Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
        }

        [Test]
        virtual public void XmlDSigRSAWithKeyInfo() {

            String filename = "xfa.signed.ki.pdf";
            String output = DestDir + filename;

            SignWithKeyInfo(Src, output, privateKey, publicKey, DigestAlgorithms.SHA1);

            String cmp = SaveXmlFromResult(output);

            Assert.IsTrue(VerifySignature(cmp), "XmlDSig Verification");

            Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
        }

        [Test]
        virtual public void XmlDSigRSAWithPublicKeyPackage() {

            String filename = "xfa.signed.pk.package.pdf";
            String output = DestDir + filename;
            SignPackageWithPublicKey(Src, output, XfaXpathConstructor.XdpPackage.Template, privateKey,
                                     publicKey, DigestAlgorithms.SHA1);

            String cmp = SaveXmlFromResult(output);

            Assert.IsTrue(VerifyPackageSignature(cmp), "XmlDSig Verification");

            Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
        }

        [Test]
        virtual public void XmlDSigRSAWithKeyInfoPackage() {

            String filename = "xfa.signed.ki.package.pdf";
            String output = DestDir + filename;

            SignPackageWithKeyInfo(Src, output, XfaXpathConstructor.XdpPackage.Template, privateKey,
                                   publicKey, DigestAlgorithms.SHA1);

            String cmp = SaveXmlFromResult(output);

            Assert.IsTrue(VerifyPackageSignature(cmp), "XmlDSig Verification");

            Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
        }
    }
}
