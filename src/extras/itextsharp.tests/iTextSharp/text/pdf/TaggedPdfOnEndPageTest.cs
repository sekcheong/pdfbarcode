﻿using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.testutils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;

namespace itextsharp.tests.iTextSharp.text.pdf {
    public class TaggedPdfOnEndPageTest {
        private const string TEST_RESOURCES_PATH = @"..\..\resources\text\pdf\TaggedPdfOnEndPageTest\";
        private const string OUTPUT_FOLDER = @"TaggedPdfOnEndPageTest\";

        [SetUp]
        public static void Init() {
            Directory.CreateDirectory(OUTPUT_FOLDER);
        }

        [Test]
        public void Test() {
            String file = "tagged_pdf_end_page.pdf";

            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(OUTPUT_FOLDER + file, FileMode.Create));
            writer.PdfVersion = PdfWriter.VERSION_1_7;
            writer.SetTagged();
            writer.CreateXmpMetadata();
            document.SetMargins(10, 10, 60, 10);

            PdfPTable headerTable = new PdfPTable(1);

            PdfPageHeader header = new CustomPdfPageHeader(writer, 10, headerTable);

            writer.PageEvent = header;

            document.Open();

            PdfPTable table = CreateContent();
            document.Add(table);

            document.Close();

            // compare
            CompareTool compareTool = new CompareTool().SetFloatRelativeError(1e-4f);
            String errorMessage = compareTool.CompareByContent(OUTPUT_FOLDER + file, TEST_RESOURCES_PATH + file,
                OUTPUT_FOLDER, "diff");
            if (errorMessage != null) {
                Assert.Fail(errorMessage);
            }
        }

        private PdfPTable CreateContent() {
            PdfPTable table = new PdfPTable(4);
            table.HeaderRows = 1;
            table.WidthPercentage = 100f;
            for (int i = 1; i <= 4; i++) {
                table.AddCell(new PdfPCell(new Phrase("#" + i)));
            }
            for (int i = 0; i < 200; i++) {
                fillRow(table);
            }
            return table;
        }

        private void fillRow(PdfPTable table) {
            for (int j = 0; j < 3; j++) {
                Phrase phrase = new Phrase("value");
                PdfPCell cell = new PdfPCell(phrase);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }
        }

        private class CustomPdfPageHeader : PdfPageHeader {
            public CustomPdfPageHeader(PdfWriter writer, float marginTop, PdfPTable headerTable)
                : base(writer, marginTop, headerTable) {
            }

            public override PdfPTable CreateTable(int pageNumber, Image total) {
                PdfPTable table = new PdfPTable(3);
                table.TotalWidth = 500;
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.AddCell(new Phrase("Header"));
                table.AddCell(new Phrase(String.Format("Page {0} of ", pageNumber)));
                PdfPCell pageTotal = new PdfPCell(total);
                pageTotal.Border = Rectangle.NO_BORDER;
                table.AddCell(pageTotal);
                return table;
            }
        }

        public abstract class PdfPageHeader : PdfPageEventHelper {
            private float marginTop;
            private List<PdfTemplate> templates = new List<PdfTemplate>();
            private PdfPTable headerTable;

            public PdfPageHeader(PdfWriter writer, float marginTop, PdfPTable headerTable) {
                this.marginTop = marginTop;
                this.headerTable = headerTable;
            }

            public override void OnStartPage(PdfWriter writer, Document document) {
                PdfContentByte canvas = writer.DirectContentUnder;

                Rectangle rect = document.PageSize;
                PdfTemplate template = canvas.CreateTemplate(20, 16);
                Image total = null;
                try {
                    total = Image.GetInstance(template);
                } catch (BadElementException e) {
                }
                total.SetAccessibleAttribute(PdfName.ALT, new PdfString("Total"));
                templates.Add(template);

                PdfPTable table = CreateTable(writer.PageNumber, total);
                if (table != null) {
                    canvas.OpenMCBlock(headerTable);
                    canvas.OpenMCBlock(headerTable.GetBody());
                    table.WriteSelectedRows(0, -1, document.LeftMargin,
                        rect.GetTop(marginTop), canvas);
                    canvas.CloseMCBlock(headerTable.GetBody());
                    canvas.CloseMCBlock(headerTable);
                }
            }

            public override void OnCloseDocument(PdfWriter writer, Document document) {
                Font font = new Font(Font.FontFamily.COURIER, 15);
                Phrase phrase = new Phrase(templates.Count.ToString(), font);
                foreach (PdfTemplate template in templates) {
                    ColumnText.ShowTextAligned(template, Element.ALIGN_LEFT, phrase, 2, 2, 0);
                }
            }

            public abstract PdfPTable CreateTable(int pageNumber, Image total);
        }
    }
}
