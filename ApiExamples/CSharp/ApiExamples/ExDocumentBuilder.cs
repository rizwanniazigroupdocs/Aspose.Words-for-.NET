﻿// Copyright (c) 2001-2020 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using NUnit.Framework;
using Cell = Aspose.Words.Tables.Cell;
using Color = System.Drawing.Color;
using Document = Aspose.Words.Document;
using Table = Aspose.Words.Tables.Table;
using System.Drawing;
#if NETCOREAPP2_1 || __MOBILE__
using SkiaSharp;
#endif

namespace ApiExamples
{
    [TestFixture]
    public class ExDocumentBuilder : ApiExampleBase
    {
        [Test]
        public void WriteAndFont()
        {
            //ExStart
            //ExFor:Font.Size
            //ExFor:Font.Bold
            //ExFor:Font.Name
            //ExFor:Font.Color
            //ExFor:Font.Underline
            //ExFor:DocumentBuilder.#ctor
            //ExSummary:Shows how to insert formatted text using DocumentBuilder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Specify font formatting, then add text.
            Aspose.Words.Font font = builder.Font;
            font.Size = 16;
            font.Bold = true;
            font.Color = Color.Blue;
            font.Name = "Courier New";
            font.Underline = Underline.Dash;

            builder.Write("Hello world!");
            //ExEnd

            doc = DocumentHelper.SaveOpen(builder.Document);
            Run firstRun = doc.FirstSection.Body.Paragraphs[0].Runs[0];

            Assert.AreEqual("Hello world!", firstRun.GetText().Trim());
            Assert.AreEqual(16.0d, firstRun.Font.Size);
            Assert.True(firstRun.Font.Bold);
            Assert.AreEqual("Courier New", firstRun.Font.Name);
            Assert.AreEqual(Color.Blue.ToArgb(), firstRun.Font.Color.ToArgb());
            Assert.AreEqual(Underline.Dash, firstRun.Font.Underline);
        }

        [Test]
        public void HeadersAndFooters()
        {
            //ExStart
            //ExFor:DocumentBuilder
            //ExFor:DocumentBuilder.#ctor(Document)
            //ExFor:DocumentBuilder.MoveToHeaderFooter
            //ExFor:DocumentBuilder.MoveToSection
            //ExFor:DocumentBuilder.InsertBreak
            //ExFor:DocumentBuilder.Writeln
            //ExFor:HeaderFooterType
            //ExFor:PageSetup.DifferentFirstPageHeaderFooter
            //ExFor:PageSetup.OddAndEvenPagesHeaderFooter
            //ExFor:BreakType
            //ExSummary:Shows how to create headers and footers in a document using DocumentBuilder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Specify that we want different headers and footers for first, even and odd pages.
            builder.PageSetup.DifferentFirstPageHeaderFooter = true;
            builder.PageSetup.OddAndEvenPagesHeaderFooter = true;

            // Create the headers, then add three pages to the document
            // in order to be able to display each header type.
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
            builder.Write("Header for the first page");
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderEven);
            builder.Write("Header for even pages");
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write("Header for all other pages");

            builder.MoveToSection(0);
            builder.Writeln("Page1");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page2");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page3");

            doc.Save(ArtifactsDir + "DocumentBuilder.HeadersAndFooters.docx");
            //ExEnd

            HeaderFooterCollection headersFooters = 
                new Document(ArtifactsDir + "DocumentBuilder.HeadersAndFooters.docx").FirstSection.HeadersFooters;

            Assert.AreEqual(3, headersFooters.Count);
            Assert.AreEqual("Header for the first page", headersFooters[HeaderFooterType.HeaderFirst].GetText().Trim());
            Assert.AreEqual("Header for even pages", headersFooters[HeaderFooterType.HeaderEven].GetText().Trim());
            Assert.AreEqual("Header for all other pages", headersFooters[HeaderFooterType.HeaderPrimary].GetText().Trim());

        }

        [Test]
        public void MergeFields()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertField(String)
            //ExFor:DocumentBuilder.MoveToMergeField(String, Boolean, Boolean)
            //ExSummary:Shows how to insert fields, and move the document builder's cursor to them.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertField(@"MERGEFIELD MyMergeField1 \* MERGEFORMAT");
            builder.InsertField(@"MERGEFIELD MyMergeField2 \* MERGEFORMAT");

            // Move the cursor to the first MERGEFIELD.
            builder.MoveToMergeField("MyMergeField1", true, false);
            
            // Note that the cursor is placed immediately after the first MERGEFIELD, and before the second.
            Assert.AreEqual(doc.Range.Fields[1].Start, builder.CurrentNode);
            Assert.AreEqual(doc.Range.Fields[0].End, builder.CurrentNode.PreviousSibling);

            // If we wish to edit the field's field code or contents using the builder,
            // its cursor would need to be inside a field.
            // To place it inside a field we would need to call the document builder's MoveTo method,
            // and pass the field's start or separator node as an argument.
            builder.Write(" Text between our merge fields. ");

            doc.Save(ArtifactsDir + "DocumentBuilder.MergeFields.docx");
            //ExEnd		

            doc = new Document(ArtifactsDir + "DocumentBuilder.MergeFields.docx");

            Assert.AreEqual("\u0013MERGEFIELD MyMergeField1 \\* MERGEFORMAT\u0014«MyMergeField1»\u0015" +
                            " Text between our merge fields. " +
                            "\u0013MERGEFIELD MyMergeField2 \\* MERGEFORMAT\u0014«MyMergeField2»\u0015", doc.GetText().Trim());
            Assert.AreEqual(2, doc.Range.Fields.Count);
            TestUtil.VerifyField(FieldType.FieldMergeField, @"MERGEFIELD MyMergeField1 \* MERGEFORMAT", 
                "«MyMergeField1»", doc.Range.Fields[0]);
            TestUtil.VerifyField(FieldType.FieldMergeField, @"MERGEFIELD MyMergeField2 \* MERGEFORMAT", 
                "«MyMergeField2»", doc.Range.Fields[1]);
        }

        [Test]
        public void InsertHorizontalRule()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertHorizontalRule
            //ExFor:ShapeBase.IsHorizontalRule
            //ExFor:Shape.HorizontalRuleFormat
            //ExFor:HorizontalRuleFormat
            //ExFor:HorizontalRuleFormat.Alignment
            //ExFor:HorizontalRuleFormat.WidthPercent
            //ExFor:HorizontalRuleFormat.Height
            //ExFor:HorizontalRuleFormat.Color
            //ExFor:HorizontalRuleFormat.NoShade
            //ExSummary:Shows how to insert a horizontal rule shape, and customize its formatting.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertHorizontalRule();

            HorizontalRuleFormat horizontalRuleFormat = shape.HorizontalRuleFormat;
            horizontalRuleFormat.Alignment = HorizontalRuleAlignment.Center;
            horizontalRuleFormat.WidthPercent = 70;
            horizontalRuleFormat.Height = 3;
            horizontalRuleFormat.Color = Color.Blue;
            horizontalRuleFormat.NoShade = true;

            Assert.True(shape.IsHorizontalRule);
            Assert.True(shape.HorizontalRuleFormat.NoShade);
            //ExEnd

            doc = DocumentHelper.SaveOpen(doc);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.AreEqual(HorizontalRuleAlignment.Center, shape.HorizontalRuleFormat.Alignment);
            Assert.AreEqual(70, shape.HorizontalRuleFormat.WidthPercent);
            Assert.AreEqual(3, shape.HorizontalRuleFormat.Height);
            Assert.AreEqual(Color.Blue.ToArgb(), shape.HorizontalRuleFormat.Color.ToArgb());
        }

        [Test(Description = "Checking the boundary conditions of WidthPercent and Height properties")]
        public void HorizontalRuleFormatExceptions()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Shape shape = builder.InsertHorizontalRule();

            HorizontalRuleFormat horizontalRuleFormat = shape.HorizontalRuleFormat;
            horizontalRuleFormat.WidthPercent = 1;
            horizontalRuleFormat.WidthPercent = 100;
            Assert.That(() => horizontalRuleFormat.WidthPercent = 0, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => horizontalRuleFormat.WidthPercent = 101, Throws.TypeOf<ArgumentOutOfRangeException>());
            
            horizontalRuleFormat.Height = 0;
            horizontalRuleFormat.Height = 1584;
            Assert.That(() => horizontalRuleFormat.Height = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => horizontalRuleFormat.Height = 1585, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InsertHyperlink()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertHyperlink
            //ExFor:Font.ClearFormatting
            //ExFor:Font.Color
            //ExFor:Font.Underline
            //ExFor:Underline
            //ExSummary:Shows how to insert a hyperlink field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("For more information, please visit the ");

            // Insert a hyperlink, and apply formatting to emphasize it.
            // The hyperlink will be a clickable piece of text which will take us to the location specified in the URL.
            builder.Font.Color = Color.Blue;
            builder.Font.Underline = Underline.Single;
            builder.InsertHyperlink("Aspose website", "http://www.aspose.com", false);
            builder.Font.ClearFormatting();
            builder.Writeln(".");

            // Ctrl + left clicking the link in the text in Microsoft Word will take us to the URL via a new web browser window.
            doc.Save(ArtifactsDir + "DocumentBuilder.InsertHyperlink.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertHyperlink.docx");

            FieldHyperlink hyperlink = (FieldHyperlink)doc.Range.Fields[0];
            TestUtil.VerifyWebResponseStatusCode(HttpStatusCode.OK, hyperlink.Address);

            Run fieldContents = (Run)hyperlink.Start.NextSibling;

            Assert.AreEqual(Color.Blue.ToArgb(), fieldContents.Font.Color.ToArgb());
            Assert.AreEqual(Underline.Single, fieldContents.Font.Underline);
            Assert.AreEqual("HYPERLINK \"http://www.aspose.com\"", fieldContents.GetText().Trim());
        }

        [Test]
        public void PushPopFont()
        {
            //ExStart
            //ExFor:DocumentBuilder.PushFont
            //ExFor:DocumentBuilder.PopFont
            //ExFor:DocumentBuilder.InsertHyperlink
            //ExSummary:Shows how to use a document builder's formatting stack.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set up font formatting, then write text that goes before the hyperlink.
            builder.Font.Name = "Arial";
            builder.Font.Size = 24;
            builder.Write("To visit Google, hold Ctrl and click ");

            // Preserve our current formatting configuration on the stack.
            builder.PushFont();

            // Alter the builder's current formatting by applying a new style.
            builder.Font.StyleIdentifier = StyleIdentifier.Hyperlink;
            builder.InsertHyperlink("here", "http://www.google.com", false);

            Assert.AreEqual(Color.Blue.ToArgb(), builder.Font.Color.ToArgb());
            Assert.AreEqual(Underline.Single, builder.Font.Underline);

            // Restore the font formatting that we saved earlier, and remove the element from the stack.
            builder.PopFont();

            Assert.AreEqual(Color.Empty.ToArgb(), builder.Font.Color.ToArgb());
            Assert.AreEqual(Underline.None, builder.Font.Underline);

            builder.Write(". We hope you enjoyed the example.");

            doc.Save(ArtifactsDir + "DocumentBuilder.PushPopFont.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.PushPopFont.docx");
            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;

            Assert.AreEqual(4, runs.Count);

            Assert.AreEqual("To visit Google, hold Ctrl and click", runs[0].GetText().Trim());
            Assert.AreEqual(". We hope you enjoyed the example.", runs[3].GetText().Trim());
            Assert.AreEqual(runs[0].Font.Color, runs[3].Font.Color);
            Assert.AreEqual(runs[0].Font.Underline, runs[3].Font.Underline);

            Assert.AreEqual("here", runs[2].GetText().Trim());
            Assert.AreEqual(Color.Blue.ToArgb(), runs[2].Font.Color.ToArgb());
            Assert.AreEqual(Underline.Single, runs[2].Font.Underline);
            Assert.AreNotEqual(runs[0].Font.Color, runs[2].Font.Color);
            Assert.AreNotEqual(runs[0].Font.Underline, runs[2].Font.Underline);

            TestUtil.VerifyWebResponseStatusCode(HttpStatusCode.OK, ((FieldHyperlink)doc.Range.Fields[0]).Address);
        }

#if NET462 || JAVA
        [Test]
        public void InsertWatermark()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToHeaderFooter
            //ExFor:PageSetup.PageWidth
            //ExFor:PageSetup.PageHeight
            //ExFor:DocumentBuilder.InsertImage(Image)
            //ExFor:WrapType
            //ExFor:RelativeHorizontalPosition
            //ExFor:RelativeVerticalPosition
            //ExSummary:Shows how to insert an image and use it as a watermark.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert the image into the header, so it will be visible on every page.
            Image image = Image.FromFile(ImageDir + "Transparent background logo.png");
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            Shape shape = builder.InsertImage(image);
            shape.WrapType = WrapType.None;
            shape.BehindText = true;

            // Place the image at the center of the page.
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            shape.Left = (builder.PageSetup.PageWidth - shape.Width) / 2;
            shape.Top = (builder.PageSetup.PageHeight - shape.Height) / 2;

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertWatermark.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertWatermark.docx");
            shape = (Shape)doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(400, 400, ImageType.Png, shape);
            Assert.AreEqual(WrapType.None, shape.WrapType);
            Assert.True(shape.BehindText);
            Assert.AreEqual(RelativeHorizontalPosition.Page, shape.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Page, shape.RelativeVerticalPosition);
            Assert.AreEqual((doc.FirstSection.PageSetup.PageWidth - shape.Width) / 2, shape.Left);
            Assert.AreEqual((doc.FirstSection.PageSetup.PageHeight - shape.Height) / 2, shape.Top);
        }

        [Test]
        public void InsertOleObject()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertOleObject(String, Boolean, Boolean, Image)
            //ExFor:DocumentBuilder.InsertOleObject(String, String, Boolean, Boolean, Image)
            //ExFor:DocumentBuilder.InsertOleObjectAsIcon(String, Boolean, String, String)
            //ExSummary:Shows how to insert an OLE object into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            
            // OLE objects are links to files in our local file system that can be opened by other installed applications.
            // Double clicking these shapes will launch the application, and then use it to open the linked object.
            // There are three ways of using the InsertOleObject method to insert these shapes and configure their appearance.
            // 1 -  Image taken from the local file system:
            Image representingImage = Image.FromFile(ImageDir + "Logo.jpg");
            builder.InsertOleObject(MyDir + "Spreadsheet.xlsx", false, false, representingImage);

            // 2 -  Icon based on the application that will open the object:
            builder.InsertOleObject(MyDir + "Spreadsheet.xlsx", "Excel.Sheet", false, true, null);

            // 3 -  Image icon that's 32 x 32 pixels or smaller from the local file system, with a custom caption:
            builder.InsertOleObjectAsIcon(MyDir + "Presentation.pptx", false, ImageDir + "Logo icon.ico",
                "Double click to view presentation!");

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertOleObject.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertOleObject.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape,0, true);
            
            Assert.AreEqual(ShapeType.OleObject, shape.ShapeType);
            Assert.AreEqual("Excel.Sheet.12", shape.OleFormat.ProgId);
            Assert.AreEqual(".xlsx", shape.OleFormat.SuggestedExtension);

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);

            Assert.AreEqual(ShapeType.OleObject, shape.ShapeType);
            Assert.AreEqual("Package", shape.OleFormat.ProgId);
            Assert.AreEqual(".xlsx", shape.OleFormat.SuggestedExtension);

            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);

            Assert.AreEqual(ShapeType.OleObject, shape.ShapeType);
            Assert.AreEqual("PowerPoint.Show.12", shape.OleFormat.ProgId);
            Assert.AreEqual(".pptx", shape.OleFormat.SuggestedExtension);
        }
#elif NETCOREAPP2_1 || __MOBILE__
        [Test]
        public void InsertWatermarkNetStandard2()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToHeaderFooter
            //ExFor:PageSetup.PageWidth
            //ExFor:PageSetup.PageHeight
            //ExFor:DocumentBuilder.InsertImage(Image)
            //ExFor:WrapType
            //ExFor:RelativeHorizontalPosition
            //ExFor:RelativeVerticalPosition
            //ExSummary:Shows how to insert a watermark image into a document using DocumentBuilder (.NetStandard 2.0).
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // The best place for the watermark image is in the header or footer so it is shown on every page
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);

            using (SKBitmap image = SKBitmap.Decode(ImageDir + "Transparent background logo.png"))
            {
                builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
                Shape shape = builder.InsertImage(image);
                shape.WrapType = WrapType.None;
                shape.BehindText = true;

                // Place the image at the center of the page.
                shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                shape.Left = (builder.PageSetup.PageWidth - shape.Width) / 2;
                shape.Top = (builder.PageSetup.PageHeight - shape.Height) / 2;
            }

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertWatermarkNetStandard2.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertWatermarkNetStandard2.docx");
            Shape outShape = (Shape)doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(400, 400, ImageType.Png, outShape);
            Assert.AreEqual(WrapType.None, outShape.WrapType);
            Assert.True(outShape.BehindText);
            Assert.AreEqual(RelativeHorizontalPosition.Page, outShape.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Page, outShape.RelativeVerticalPosition);
            Assert.AreEqual((doc.FirstSection.PageSetup.PageWidth - outShape.Width) / 2, outShape.Left);
            Assert.AreEqual((doc.FirstSection.PageSetup.PageHeight - outShape.Height) / 2, outShape.Top);
        }

        [Test]
        public void InsertOleObjectNetStandard2()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertOleObject(String, Boolean, Boolean, Image)
            //ExFor:DocumentBuilder.InsertOleObject(String, String, Boolean, Boolean, Image)
            //ExSummary:Shows how to insert an OLE object into a document (.NetStandard 2.0).
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            using (SKBitmap representingImage = SKBitmap.Decode(ImageDir + "Logo.jpg"))
            {
                // OLE objects are links to files in our local file system that can be opened by other installed applications.
                // Double clicking these shapes will launch the application, and then use it to open the linked object.
                // There are two ways of using the InsertOleObject method
                // to create a shape with an image taken from a SKBitmap object.
                // 1 -  Pass the local file system filename of the linked object:
                builder.InsertOleObject(MyDir + "Spreadsheet.xlsx", false, false, representingImage);

                // 2 -  Pass the local file system filename of the linked object,
                // and specify the class key for an application that opens the object:
                builder.InsertOleObject(MyDir + "Spreadsheet.xlsx", "Excel.Sheet", false, false,
                    representingImage);
            }

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertOleObjectNetStandard2.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertOleObjectNetStandard2.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape,0, true);
            
            Assert.AreEqual(ShapeType.OleObject, shape.ShapeType);
            Assert.AreEqual("Excel.Sheet.12", shape.OleFormat.ProgId);
            Assert.AreEqual(".xlsx", shape.OleFormat.SuggestedExtension);

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);

            Assert.AreEqual(ShapeType.OleObject, shape.ShapeType);
            Assert.AreEqual("Package", shape.OleFormat.ProgId);
            Assert.AreEqual(".xlsx", shape.OleFormat.SuggestedExtension);
        }
#endif

        [Test]
        public void InsertHtml()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertHtml(String)
            //ExSummary:Shows how to use a document builder to insert html content into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            const string html = "<p align='right'>Paragraph right</p>" + 
                                "<b>Implicit paragraph left</b>" +
                                "<div align='center'>Div center</div>" + 
                                "<h1 align='left'>Heading 1 left.</h1>";

            builder.InsertHtml(html);

            // Inserting HTML code parses the formatting of each element into equivalent document text formatting.  
            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.AreEqual("Paragraph right", paragraphs[0].GetText().Trim());
            Assert.AreEqual(ParagraphAlignment.Right, paragraphs[0].ParagraphFormat.Alignment);

            Assert.AreEqual("Implicit paragraph left", paragraphs[1].GetText().Trim());
            Assert.AreEqual(ParagraphAlignment.Left, paragraphs[1].ParagraphFormat.Alignment);
            Assert.True(paragraphs[1].Runs[0].Font.Bold);

            Assert.AreEqual("Div center", paragraphs[2].GetText().Trim());
            Assert.AreEqual(ParagraphAlignment.Center, paragraphs[2].ParagraphFormat.Alignment);

            Assert.AreEqual("Heading 1 left.", paragraphs[3].GetText().Trim());
            Assert.AreEqual("Heading 1", paragraphs[3].ParagraphFormat.Style.Name);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertHtml.docx");
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InsertHtmlWithFormatting(bool useBuilderFormatting)
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertHtml(String, Boolean)
            //ExSummary:Shows how to apply a document builder's formatting while inserting HTML content. 
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set a text alignment for the builder, then insert an HTML paragraph
            // with a specified alignment, and one without.
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Distributed;
            builder.InsertHtml(
                "<p align='right'>Paragraph 1.</p>" +
                "<p>Paragraph 2.</p>", useBuilderFormatting);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            // The first paragraph has an alignment specified. When the HTML code is parsed by InsertHtml,
            // the paragraph alignment value found in the HTML code always supersedes the document builder's value.
            Assert.AreEqual("Paragraph 1.", paragraphs[0].GetText().Trim());
            Assert.AreEqual(ParagraphAlignment.Right, paragraphs[0].ParagraphFormat.Alignment);

            // The second paragraph has no alignment specified. It can have its alignment value filled in
            // by the builder's value depending on the flag we passed to the InsertHtml method.
            Assert.AreEqual("Paragraph 2.", paragraphs[1].GetText().Trim());
            if (useBuilderFormatting)
                Assert.AreEqual(ParagraphAlignment.Distributed, paragraphs[1].ParagraphFormat.Alignment);
            else
                Assert.AreEqual(ParagraphAlignment.Left, paragraphs[1].ParagraphFormat.Alignment);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertHtmlWithFormatting.docx");
            //ExEnd
        }

        [Test]
        public void MathML()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            const string mathMl =
                "<math xmlns=\"http://www.w3.org/1998/Math/MathML\"><mrow><msub><mi>a</mi><mrow><mn>1</mn></mrow></msub><mo>+</mo><msub><mi>b</mi><mrow><mn>1</mn></mrow></msub></mrow></math>";

            builder.InsertHtml(mathMl);

            doc.Save(ArtifactsDir + "DocumentBuilder.MathML.docx");
            doc.Save(ArtifactsDir + "DocumentBuilder.MathML.pdf");

            Assert.IsTrue(DocumentHelper.CompareDocs(GoldsDir + "DocumentBuilder.MathML Gold.docx", ArtifactsDir + "DocumentBuilder.MathML.docx"));
        }

        [Test]
        public void InsertTextAndBookmark()
        {
            //ExStart
            //ExFor:DocumentBuilder.StartBookmark
            //ExFor:DocumentBuilder.EndBookmark
            //ExSummary:Shows how create a bookmark.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // A valid bookmark needs to have document body text enclosed by
            // BookmarkStart and BookmarkEnd nodes created with a matching bookmark name.
            builder.StartBookmark("MyBookmark");
            builder.Writeln("Hello world!");
            builder.EndBookmark("MyBookmark");
            
            Assert.AreEqual(1, doc.Range.Bookmarks.Count);
            Assert.AreEqual("MyBookmark", doc.Range.Bookmarks[0].Name);
            Assert.AreEqual("Hello world!", doc.Range.Bookmarks[0].Text.Trim());
            //ExEnd
        }

        [Test]
        public void CreateForm()
        {
            //ExStart
            //ExFor:TextFormFieldType
            //ExFor:DocumentBuilder.InsertTextInput
            //ExFor:DocumentBuilder.InsertComboBox
            //ExSummary:Shows how to create form fields.
            DocumentBuilder builder = new DocumentBuilder();

            // Form fields are objects in the document that the user can interact with by being prompted to enter values.
            // They can be created using a document builder, below are two of the possible ways of doing so.
            // 1 -  Basic text input:
            builder.InsertTextInput("My text input", TextFormFieldType.Regular, 
                "", "Enter your name here", 30);
            
            // 2 -  Combo box with prompt text, and a range of possible values:
            string[] items =
            {
                "-- Select your favorite footwear --", "Sneakers", "Oxfords", "Flip-flops", "Other"
            };

            builder.InsertParagraph();
            builder.InsertComboBox("My combo box", items, 0);

            builder.Document.Save(ArtifactsDir + "DocumentBuilder.CreateForm.docx");
            //ExEnd

            Document doc = new Document(ArtifactsDir + "DocumentBuilder.CreateForm.docx");
            FormField formField = doc.Range.FormFields[0];

            Assert.AreEqual("My text input", formField.Name);
            Assert.AreEqual(TextFormFieldType.Regular, formField.TextInputType);
            Assert.AreEqual("Enter your name here", formField.Result);

            formField = doc.Range.FormFields[1];

            Assert.AreEqual("My combo box", formField.Name);
            Assert.AreEqual(TextFormFieldType.Regular, formField.TextInputType);
            Assert.AreEqual("-- Select your favorite footwear --", formField.Result);
            Assert.AreEqual(0, formField.DropDownSelectedIndex);
            Assert.AreEqual(new[]
            {
                "-- Select your favorite footwear --", "Sneakers", "Oxfords", "Flip-flops", "Other"
            }, formField.DropDownItems.ToArray());
        }

        [Test]
        public void InsertCheckBox()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertCheckBox(string, bool, bool, int)
            //ExFor:DocumentBuilder.InsertCheckBox(String, bool, int)
            //ExSummary:Shows how to insert checkboxes into the document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert check boxes of varying sizes and default checked statuses.
            builder.Write("Unchecked check box of a default size: ");
            builder.InsertCheckBox(string.Empty, false, false, 0);
            builder.InsertParagraph();

            builder.Write("Large checked check box: ");
            builder.InsertCheckBox("CheckBox_Default", true, true, 50);
            builder.InsertParagraph();

            // Form fields have a name length limit of 20 characters.
            builder.Write("Very large checked check box: ");
            builder.InsertCheckBox("CheckBox_OnlyCheckedValue", true, 100);

            Assert.AreEqual("CheckBox_OnlyChecked", doc.Range.FormFields[2].Name);

            // We can interact with these check boxes in Microsoft Word by double clicking them.
            doc.Save(ArtifactsDir + "DocumentBuilder.InsertCheckBox.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertCheckBox.docx");

            FormFieldCollection formFields = doc.Range.FormFields;

            Assert.AreEqual(string.Empty, formFields[0].Name);
            Assert.AreEqual(false, formFields[0].Checked);
            Assert.AreEqual(false, formFields[0].Default);
            Assert.AreEqual(10, formFields[0].CheckBoxSize);

            Assert.AreEqual("CheckBox_Default", formFields[1].Name);
            Assert.AreEqual(true, formFields[1].Checked);
            Assert.AreEqual(true, formFields[1].Default);
            Assert.AreEqual(50, formFields[1].CheckBoxSize);

            Assert.AreEqual("CheckBox_OnlyChecked", formFields[2].Name);
            Assert.AreEqual(true, formFields[2].Checked);
            Assert.AreEqual(true, formFields[2].Default);
            Assert.AreEqual(100, formFields[2].CheckBoxSize);
        }

        [Test]
        public void InsertCheckBoxEmptyName()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Checking that the checkbox insertion with an empty name working correctly
            builder.InsertCheckBox("", true, false, 1);
            builder.InsertCheckBox(string.Empty, false, 1);
        }

        [Test]
        public void WorkingWithNodes()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveTo(Node)
            //ExFor:DocumentBuilder.MoveToBookmark(String)
            //ExFor:DocumentBuilder.CurrentParagraph
            //ExFor:DocumentBuilder.CurrentNode
            //ExFor:DocumentBuilder.MoveToDocumentStart
            //ExFor:DocumentBuilder.MoveToDocumentEnd
            //ExFor:DocumentBuilder.IsAtEndOfParagraph
            //ExFor:DocumentBuilder.IsAtStartOfParagraph
            //ExSummary:Shows how to move a document builder's cursor to different nodes in a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create a valid bookmark, which is an entity which consists of nodes
            // enclosed by a bookmark start node, and a bookmark end node. 
            builder.StartBookmark("MyBookmark");
            builder.Write("Bookmark contents.");
            builder.EndBookmark("MyBookmark");

            NodeCollection firstParagraphNodes = doc.FirstSection.Body.FirstParagraph.ChildNodes;

            Assert.AreEqual(NodeType.BookmarkStart, firstParagraphNodes[0].NodeType);
            Assert.AreEqual(NodeType.Run, firstParagraphNodes[1].NodeType);
            Assert.AreEqual("Bookmark contents.", firstParagraphNodes[1].GetText().Trim());
            Assert.AreEqual(NodeType.BookmarkEnd, firstParagraphNodes[2].NodeType);

            // The document builder's cursor is always ahead of the node that we last added with it.
            // If the builder's cursor is at the end of the document, then its current node will be null.   
            // The previous node is the bookmark end node that we last added.
            // Adding new nodes with the builder will append them to the last node.
            Assert.Null(builder.CurrentNode);

            // If we wish to edit a different part of the document with the builder,
            // we will need to bring its cursor to the node we wish to edit.
            builder.MoveToBookmark("MyBookmark");

            // Moving it to a bookmark will move it to the first node within the bookmark start and end nodes,
            // which is the enclosed run.
            Assert.AreEqual(firstParagraphNodes[1], builder.CurrentNode);

            // We can also move the cursor to an individual node like this. 
            builder.MoveTo(doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false)[0]);

            Assert.AreEqual(NodeType.BookmarkStart, builder.CurrentNode.NodeType);
            Assert.AreEqual(doc.FirstSection.Body.FirstParagraph, builder.CurrentParagraph);
            Assert.IsTrue(builder.IsAtStartOfParagraph);

            // A shorter way of moving the very start/end of a document can be done using specific MoveTo methods.
            builder.MoveToDocumentEnd();

            Assert.IsTrue(builder.IsAtEndOfParagraph);

            builder.MoveToDocumentStart();

            Assert.IsTrue(builder.IsAtStartOfParagraph);
            //ExEnd
        }

        [Test]
        public void FillMergeFields()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToMergeField(String)
            //ExFor:DocumentBuilder.Bold
            //ExFor:DocumentBuilder.Italic
            //ExSummary:Shows how to fill MERGEFIELDs with data with a document builder instead of a mail merge.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert some MERGEFIELDS, which accept data from columns of the same name
            // in a data source during a mail merge, and then fill them in manually.
            builder.InsertField(" MERGEFIELD Chairman ");
            builder.InsertField(" MERGEFIELD ChiefFinancialOfficer ");
            builder.InsertField(" MERGEFIELD ChiefTechnologyOfficer ");

            builder.MoveToMergeField("Chairman");
            builder.Bold = true;
            builder.Writeln("John Doe");

            builder.MoveToMergeField("ChiefFinancialOfficer");
            builder.Italic = true;
            builder.Writeln("Jane Doe");

            builder.MoveToMergeField("ChiefTechnologyOfficer");
            builder.Italic = true;
            builder.Writeln("John Bloggs");

            doc.Save(ArtifactsDir + "DocumentBuilder.FillMergeFields.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.FillMergeFields.docx");
            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.True(paragraphs[0].Runs[0].Font.Bold);
            Assert.AreEqual("John Doe", paragraphs[0].Runs[0].GetText().Trim());

            Assert.True(paragraphs[1].Runs[0].Font.Italic);
            Assert.AreEqual("Jane Doe", paragraphs[1].Runs[0].GetText().Trim());

            Assert.True(paragraphs[2].Runs[0].Font.Italic);
            Assert.AreEqual("John Bloggs", paragraphs[2].Runs[0].GetText().Trim());

        }

        [Test]
        public void InsertToc()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertTableOfContents
            //ExFor:Document.UpdateFields
            //ExFor:DocumentBuilder.#ctor(Document)
            //ExFor:ParagraphFormat.StyleIdentifier
            //ExFor:DocumentBuilder.InsertBreak
            //ExFor:BreakType
            //ExSummary:Shows how to insert a Table of contents (TOC) into a document using heading styles as entries.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a table of contents for the first page of the document.
            // Configure the table to pick up paragraphs with headings of levels 1 to 3.
            // Also, set its entries to be hyperlinks that will take us
            // to the location of the heading when left-clicked in Microsoft Word.
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            builder.InsertBreak(BreakType.PageBreak);

            // Populate the table of contents by adding paragraphs with heading styles.
            // Each such heading will create an entry in the table, as long as its heading level is between 1 and 3.
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln("Heading 1");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;
            builder.Writeln("Heading 1.1");
            builder.Writeln("Heading 1.2");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln("Heading 2");
            builder.Writeln("Heading 3");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;
            builder.Writeln("Heading 3.1");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading3;
            builder.Writeln("Heading 3.1.1");
            builder.Writeln("Heading 3.1.2");
            builder.Writeln("Heading 3.1.3");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading4;
            builder.Writeln("Heading 3.1.3.1");
            builder.Writeln("Heading 3.1.3.2");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;
            builder.Writeln("Heading 3.2");
            builder.Writeln("Heading 3.3");

            // A table of contents is a field of a type that needs to be updated in order to show an up-to-date result.
            doc.UpdateFields();
            doc.Save(ArtifactsDir + "DocumentBuilder.InsertToc.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertToc.docx");
            FieldToc tableOfContents = (FieldToc)doc.Range.Fields[0];

            Assert.AreEqual("1-3", tableOfContents.HeadingLevelRange);
            Assert.IsTrue(tableOfContents.InsertHyperlinks);
            Assert.IsTrue(tableOfContents.HideInWebLayout);
            Assert.IsTrue(tableOfContents.UseParagraphOutlineLevel);
        }

        [Test]
        public void InsertTable()
        {
            //ExStart
            //ExFor:DocumentBuilder
            //ExFor:DocumentBuilder.Write
            //ExFor:DocumentBuilder.StartTable
            //ExFor:DocumentBuilder.InsertCell
            //ExFor:DocumentBuilder.EndRow
            //ExFor:DocumentBuilder.EndTable
            //ExFor:DocumentBuilder.CellFormat
            //ExFor:DocumentBuilder.RowFormat
            //ExFor:CellFormat
            //ExFor:CellFormat.FitText
            //ExFor:CellFormat.Width
            //ExFor:CellFormat.VerticalAlignment
            //ExFor:CellFormat.Shading
            //ExFor:CellFormat.Orientation
            //ExFor:CellFormat.WrapText
            //ExFor:RowFormat
            //ExFor:RowFormat.Borders
            //ExFor:RowFormat.ClearFormatting
            //ExFor:Shading.ClearFormatting
            //ExSummary:Shows how to build a table with custom borders.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();

            // Setting table formatting options for a document builder
            // will apply them to every row and cell that we add with it.
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            builder.CellFormat.ClearFormatting();
            builder.CellFormat.Width = 150;
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.CellFormat.Shading.BackgroundPatternColor = Color.GreenYellow;
            builder.CellFormat.WrapText = false;
            builder.CellFormat.FitText = true;

            builder.RowFormat.ClearFormatting();
            builder.RowFormat.HeightRule = HeightRule.Exactly;
            builder.RowFormat.Height = 50;
            builder.RowFormat.Borders.LineStyle = LineStyle.Engrave3D;
            builder.RowFormat.Borders.Color = Color.Orange;

            builder.InsertCell();
            builder.Write("Row 1, Col 1");

            builder.InsertCell();
            builder.Write("Row 1, Col 2");
            builder.EndRow();

            // Changing the formatting will apply it to the current cell,
            // and any new cells that we create with the builder afterwards.
            // This will not affect the cells that we have added previously.
            builder.CellFormat.Shading.ClearFormatting();

            builder.InsertCell();
            builder.Write("Row 2, Col 1");

            builder.InsertCell();
            builder.Write("Row 2, Col 2");

            builder.EndRow();

            // Increase row height to fit vertical text.
            builder.InsertCell();
            builder.RowFormat.Height = 150;
            builder.CellFormat.Orientation = TextOrientation.Upward;
            builder.Write("Row 3, Col 1");

            builder.InsertCell();
            builder.CellFormat.Orientation = TextOrientation.Downward;
            builder.Write("Row 3, Col 2");

            builder.EndRow();
            builder.EndTable();

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTable.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTable.docx");
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual("Row 1, Col 1\a", table.Rows[0].Cells[0].GetText().Trim());
            Assert.AreEqual("Row 1, Col 2\a", table.Rows[0].Cells[1].GetText().Trim());
            Assert.AreEqual(HeightRule.Exactly, table.Rows[0].RowFormat.HeightRule);
            Assert.AreEqual(50.0d, table.Rows[0].RowFormat.Height);
            Assert.AreEqual(LineStyle.Engrave3D, table.Rows[0].RowFormat.Borders.LineStyle);
            Assert.AreEqual(Color.Orange.ToArgb(), table.Rows[0].RowFormat.Borders.Color.ToArgb());

            foreach (Cell c in table.Rows[0].Cells)
            {
                Assert.AreEqual(150, c.CellFormat.Width);
                Assert.AreEqual(CellVerticalAlignment.Center, c.CellFormat.VerticalAlignment);
                Assert.AreEqual(Color.GreenYellow.ToArgb(), c.CellFormat.Shading.BackgroundPatternColor.ToArgb());
                Assert.IsFalse(c.CellFormat.WrapText);
                Assert.IsTrue(c.CellFormat.FitText);

                Assert.AreEqual(ParagraphAlignment.Center, c.FirstParagraph.ParagraphFormat.Alignment);
            }

            Assert.AreEqual("Row 2, Col 1\a", table.Rows[1].Cells[0].GetText().Trim());
            Assert.AreEqual("Row 2, Col 2\a", table.Rows[1].Cells[1].GetText().Trim());


            foreach (Cell c in table.Rows[1].Cells)
            {
                Assert.AreEqual(150, c.CellFormat.Width);
                Assert.AreEqual(CellVerticalAlignment.Center, c.CellFormat.VerticalAlignment);
                Assert.AreEqual(Color.Empty.ToArgb(), c.CellFormat.Shading.BackgroundPatternColor.ToArgb());
                Assert.IsFalse(c.CellFormat.WrapText);
                Assert.IsTrue(c.CellFormat.FitText);

                Assert.AreEqual(ParagraphAlignment.Center, c.FirstParagraph.ParagraphFormat.Alignment);
            }

            Assert.AreEqual(150, table.Rows[2].RowFormat.Height);

            Assert.AreEqual("Row 3, Col 1\a", table.Rows[2].Cells[0].GetText().Trim());
            Assert.AreEqual(TextOrientation.Upward, table.Rows[2].Cells[0].CellFormat.Orientation);
            Assert.AreEqual(ParagraphAlignment.Center, table.Rows[2].Cells[0].FirstParagraph.ParagraphFormat.Alignment);

            Assert.AreEqual("Row 3, Col 2\a", table.Rows[2].Cells[1].GetText().Trim());
            Assert.AreEqual(TextOrientation.Downward, table.Rows[2].Cells[1].CellFormat.Orientation);
            Assert.AreEqual(ParagraphAlignment.Center, table.Rows[2].Cells[1].FirstParagraph.ParagraphFormat.Alignment);
        }

        [Test]
        public void InsertTableWithStyle()
        {
            //ExStart
            //ExFor:Table.StyleIdentifier
            //ExFor:Table.StyleOptions
            //ExFor:TableStyleOptions
            //ExFor:Table.AutoFit
            //ExFor:AutoFitBehavior
            //ExSummary:Shows how to build a new table while applying a style.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Table table = builder.StartTable();

            // We must insert at least one row before setting any table formatting.
            builder.InsertCell();

            // Set the table style used based on the style identifier.
            // Note that not all table styles are available when saving to .doc format.
            table.StyleIdentifier = StyleIdentifier.MediumShading1Accent1;

            // Partially apply the style to features of the table based on predicates, then build the table.
            table.StyleOptions =
                TableStyleOptions.FirstColumn | TableStyleOptions.RowBands | TableStyleOptions.FirstRow;
            table.AutoFit(AutoFitBehavior.AutoFitToContents);

            builder.Writeln("Item");
            builder.CellFormat.RightPadding = 40;
            builder.InsertCell();
            builder.Writeln("Quantity (kg)");
            builder.EndRow();

            builder.InsertCell();
            builder.Writeln("Apples");
            builder.InsertCell();
            builder.Writeln("20");
            builder.EndRow();

            builder.InsertCell();
            builder.Writeln("Bananas");
            builder.InsertCell();
            builder.Writeln("40");
            builder.EndRow();

            builder.InsertCell();
            builder.Writeln("Carrots");
            builder.InsertCell();
            builder.Writeln("50");
            builder.EndRow();

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTableWithStyle.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTableWithStyle.docx");

            doc.ExpandTableStylesToDirectFormatting();

            Assert.AreEqual("Medium Shading 1 Accent 1", table.Style.Name);
            Assert.AreEqual(TableStyleOptions.FirstColumn | TableStyleOptions.RowBands | TableStyleOptions.FirstRow,
                table.StyleOptions);
            Assert.AreEqual(189, table.FirstRow.FirstCell.CellFormat.Shading.BackgroundPatternColor.B);
            Assert.AreEqual(Color.White.ToArgb(), table.FirstRow.FirstCell.FirstParagraph.Runs[0].Font.Color.ToArgb());
            Assert.AreNotEqual(Color.LightBlue.ToArgb(),
                table.LastRow.FirstCell.CellFormat.Shading.BackgroundPatternColor.B);
            Assert.AreEqual(Color.Empty.ToArgb(), table.LastRow.FirstCell.FirstParagraph.Runs[0].Font.Color.ToArgb());
        }

        [Test]
        public void InsertTableSetHeadingRow()
        {
            //ExStart
            //ExFor:RowFormat.HeadingFormat
            //ExSummary:Shows how to build a table with rows that repeat on every page. 
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();

            // Any rows inserted while the HeadingFormat flag is set to true
            // will show up at the top of the table on every page that it spans.
            builder.RowFormat.HeadingFormat = true;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.CellFormat.Width = 100;
            builder.InsertCell();
            builder.Write("Heading row 1");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Heading row 2");
            builder.EndRow();

            builder.CellFormat.Width = 50;
            builder.ParagraphFormat.ClearFormatting();
            builder.RowFormat.HeadingFormat = false;

            // Add enough rows for the table to span two pages.
            for (int i = 0; i < 50; i++)
            {
                builder.InsertCell();
                builder.Write($"Row {table.Rows.Count}, column 1.");
                builder.InsertCell();
                builder.Write($"Row {table.Rows.Count}, column 2.");
                builder.EndRow();
            }

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTableSetHeadingRow.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTableSetHeadingRow.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            for (int i = 0; i < table.Rows.Count; i++)
                Assert.AreEqual(i < 2, table.Rows[i].RowFormat.HeadingFormat);
        }

        [Test]
        public void InsertTableWithPreferredWidth()
        {
            //ExStart
            //ExFor:Table.PreferredWidth
            //ExFor:PreferredWidth.FromPercent
            //ExFor:PreferredWidth
            //ExSummary:Shows how to set a table to auto fit to 50% of the width of the page.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Cell #1");
            builder.InsertCell();
            builder.Write("Cell #2");
            builder.InsertCell();
            builder.Write("Cell #3");

            table.PreferredWidth = PreferredWidth.FromPercent(50);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTableWithPreferredWidth.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTableWithPreferredWidth.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(PreferredWidthType.Percent, table.PreferredWidth.Type);
            Assert.AreEqual(50, table.PreferredWidth.Value);
        }

        [Test]
        public void InsertCellsWithPreferredWidths()
        {
            //ExStart
            //ExFor:CellFormat.PreferredWidth
            //ExFor:PreferredWidth
            //ExFor:PreferredWidth.Auto
            //ExFor:PreferredWidth.Equals(PreferredWidth)
            //ExFor:PreferredWidth.Equals(System.Object)
            //ExFor:PreferredWidth.FromPoints
            //ExFor:PreferredWidth.FromPercent
            //ExFor:PreferredWidth.GetHashCode
            //ExFor:PreferredWidth.ToString
            //ExSummary:Shows how to set a preferred width for table cells.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Table table = builder.StartTable();

            // There are two ways of applying the PreferredWidth class to table cells.
            // 1 -  Set an absolute preferred width based on points.
            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(40);
            builder.CellFormat.Shading.BackgroundPatternColor = Color.LightYellow;
            builder.Writeln($"Cell with a width of {builder.CellFormat.PreferredWidth}.");

            // 2 -  Set a relative preferred width based on percent of the table's width.
            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPercent(20);
            builder.CellFormat.Shading.BackgroundPatternColor = Color.LightBlue;
            builder.Writeln($"Cell with a width of {builder.CellFormat.PreferredWidth}.");

            builder.InsertCell();

            // A cell with no preferred width specified will take up the rest of the available space.
            builder.CellFormat.PreferredWidth = PreferredWidth.Auto;

            // Each configuration of the PreferredWidth attribute creates a new object.
            Assert.AreNotEqual(table.FirstRow.Cells[1].CellFormat.PreferredWidth.GetHashCode(),
                builder.CellFormat.PreferredWidth.GetHashCode());

            builder.CellFormat.Shading.BackgroundPatternColor = Color.LightGreen;
            builder.Writeln("Automatically sized cell.");
            
            doc.Save(ArtifactsDir + "DocumentBuilder.InsertCellsWithPreferredWidths.docx");
            //ExEnd

            Assert.AreEqual(100.0d, PreferredWidth.FromPercent(100).Value);
            Assert.AreEqual(100.0d, PreferredWidth.FromPoints(100).Value);

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertCellsWithPreferredWidths.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);
            
            Assert.AreEqual(PreferredWidthType.Points, table.FirstRow.Cells[0].CellFormat.PreferredWidth.Type);
            Assert.AreEqual(40.0d, table.FirstRow.Cells[0].CellFormat.PreferredWidth.Value);
            Assert.AreEqual("Cell with a width of 800.\r\a", table.FirstRow.Cells[0].GetText().Trim());

            Assert.AreEqual(PreferredWidthType.Percent, table.FirstRow.Cells[1].CellFormat.PreferredWidth.Type);
            Assert.AreEqual(20.0d, table.FirstRow.Cells[1].CellFormat.PreferredWidth.Value);
            Assert.AreEqual("Cell with a width of 20%.\r\a", table.FirstRow.Cells[1].GetText().Trim());

            Assert.AreEqual(PreferredWidthType.Auto, table.FirstRow.Cells[2].CellFormat.PreferredWidth.Type);
            Assert.AreEqual(0.0d, table.FirstRow.Cells[2].CellFormat.PreferredWidth.Value);
            Assert.AreEqual("Automatically sized cell.\r\a", table.FirstRow.Cells[2].GetText().Trim());
        }

        [Test]
        public void InsertTableFromHtml()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert the table from HTML. Note that AutoFitSettings does not apply to tables
            // inserted from HTML.
            builder.InsertHtml("<table>" + "<tr>" + "<td>Row 1, Cell 1</td>" + "<td>Row 1, Cell 2</td>" + "</tr>" +
                               "<tr>" + "<td>Row 2, Cell 2</td>" + "<td>Row 2, Cell 2</td>" + "</tr>" + "</table>");

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTableFromHtml.docx");

            // Verify the table was constructed properly
            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTableFromHtml.docx");

            Assert.AreEqual(1, doc.GetChildNodes(NodeType.Table, true).Count);
            Assert.AreEqual(2, doc.GetChildNodes(NodeType.Row, true).Count);
            Assert.AreEqual(4, doc.GetChildNodes(NodeType.Cell, true).Count);
        }

        [Test]
        public void InsertNestedTable()
        {
            //ExStart
            //ExFor:Cell.FirstParagraph
            //ExSummary:Shows how to create a nested table using a document builder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Build the outer table.
            Cell cell = builder.InsertCell();
            builder.Writeln("Outer Table Cell 1");
            builder.InsertCell();
            builder.Writeln("Outer Table Cell 2");
            builder.EndTable();

            // Move to the first cell of the outer table, the build another table inside the cell.
            builder.MoveTo(cell.FirstParagraph);
            builder.InsertCell();
            builder.Writeln("Inner Table Cell 1");
            builder.InsertCell();
            builder.Writeln("Inner Table Cell 2");
            builder.EndTable();

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertNestedTable.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertNestedTable.docx");

            Assert.AreEqual(2, doc.GetChildNodes(NodeType.Table, true).Count);
            Assert.AreEqual(4, doc.GetChildNodes(NodeType.Cell, true).Count);
            Assert.AreEqual(1, cell.Tables[0].Count);
            Assert.AreEqual(2, cell.Tables[0].FirstRow.Cells.Count);
        }

        [Test]
        public void CreateTable()
        {
            //ExStart
            //ExFor:DocumentBuilder
            //ExFor:DocumentBuilder.Write
            //ExFor:DocumentBuilder.InsertCell
            //ExSummary:Shows how to use a document builder to create a table.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Start the table, then populate the first row with two cells.
            builder.StartTable();
            builder.InsertCell();
            builder.Write("Row 1, Cell 1.");
            builder.InsertCell();
            builder.Write("Row 1, Cell 2.");

            // Call the builder's EndRow method to start a new row.
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Row 2, Cell 1.");
            builder.InsertCell();
            builder.Write("Row 2, Cell 2.");
            builder.EndTable();

            doc.Save(ArtifactsDir + "DocumentBuilder.CreateTable.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.CreateTable.docx");
            Table table = (Table) doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(4, table.GetChildNodes(NodeType.Cell, true).Count);

            Assert.AreEqual("Row 1, Cell 1.\a", table.Rows[0].Cells[0].GetText().Trim());
            Assert.AreEqual("Row 1, Cell 2.\a", table.Rows[0].Cells[1].GetText().Trim());
            Assert.AreEqual("Row 2, Cell 1.\a", table.Rows[1].Cells[0].GetText().Trim());
            Assert.AreEqual("Row 2, Cell 2.\a", table.Rows[1].Cells[1].GetText().Trim());
        }

        [Test]
        public void BuildFormattedTable()
        {
            //ExStart
            //ExFor:RowFormat.Height
            //ExFor:RowFormat.HeightRule
            //ExFor:Table.LeftIndent
            //ExFor:DocumentBuilder.ParagraphFormat
            //ExFor:DocumentBuilder.Font
            //ExSummary:Shows how to create a formatted table using DocumentBuilder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            table.LeftIndent = 20;

            // Set some formatting options for text and table appearance.
            builder.RowFormat.Height = 40;
            builder.RowFormat.HeightRule = HeightRule.AtLeast;
            builder.CellFormat.Shading.BackgroundPatternColor = Color.FromArgb(198, 217, 241);

            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Font.Size = 16;
            builder.Font.Name = "Arial";
            builder.Font.Bold = true;

            // Configuring the formatting options in a document builder will apply them
            // to the current cell/row its cursor is in,
            // as well as any new cells and rows created using that builder.
            builder.Write("Header Row,\n Cell 1");
            builder.InsertCell();
            builder.Write("Header Row,\n Cell 2");
            builder.InsertCell();
            builder.Write("Header Row,\n Cell 3");
            builder.EndRow();

            // Reconfigure the builder's formatting objects for new rows and cells that we are about to make.
            // The builder will not apply these to the first row that was already created,
            // so it will stand out as a header row.
            builder.CellFormat.Shading.BackgroundPatternColor = Color.White;
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.RowFormat.Height = 30;
            builder.RowFormat.HeightRule = HeightRule.Auto;
            builder.InsertCell();
            builder.Font.Size = 12;
            builder.Font.Bold = false;

            builder.Write("Row 1, Cell 1.");
            builder.InsertCell();
            builder.Write("Row 1, Cell 2.");
            builder.InsertCell();
            builder.Write("Row 1, Cell 3.");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Row 2, Cell 1.");
            builder.InsertCell();
            builder.Write("Row 2, Cell 2.");
            builder.InsertCell();
            builder.Write("Row 2, Cell 3.");
            builder.EndRow();
            builder.EndTable();

            doc.Save(ArtifactsDir + "DocumentBuilder.CreateFormattedTable.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.CreateFormattedTable.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(20.0d, table.LeftIndent);

            Assert.AreEqual(HeightRule.AtLeast, table.Rows[0].RowFormat.HeightRule);
            Assert.AreEqual(40.0d, table.Rows[0].RowFormat.Height);

            foreach (Cell c in doc.GetChildNodes(NodeType.Cell, true))
            {
                Assert.AreEqual(ParagraphAlignment.Center, c.FirstParagraph.ParagraphFormat.Alignment);

                foreach (Run r in c.FirstParagraph.Runs)
                {
                    Assert.AreEqual("Arial", r.Font.Name);

                    if (c.ParentRow == table.FirstRow)
                    {
                        Assert.AreEqual(16, r.Font.Size);
                        Assert.True(r.Font.Bold);
                    }
                    else
                    {
                        Assert.AreEqual(12, r.Font.Size);
                        Assert.False(r.Font.Bold);
                    }
                }
            }
        }

        [Test]
        public void TableBordersAndShading()
        {
            //ExStart
            //ExFor:Shading
            //ExFor:Table.SetBorders
            //ExFor:BorderCollection.Left
            //ExFor:BorderCollection.Right
            //ExFor:BorderCollection.Top
            //ExFor:BorderCollection.Bottom
            //ExSummary:Shows how to apply border and shading color while building a table.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Start a table, and set a default color/thickness for its borders.
            Table table = builder.StartTable();
            table.SetBorders(LineStyle.Single, 2.0, Color.Black);

            // Create a row with two cells with different background colors.
            builder.InsertCell();
            builder.CellFormat.Shading.BackgroundPatternColor = Color.LightSkyBlue;
            builder.Writeln("Row 1, Cell 1.");
            builder.InsertCell();
            builder.CellFormat.Shading.BackgroundPatternColor = Color.Orange;
            builder.Writeln("Row 1, Cell 2.");
            builder.EndRow();

            // Reset cell formatting to disable the background colors,
            // set a custom border thickness for all new cells created by the builder,
            // then build a second row.
            builder.CellFormat.ClearFormatting();
            builder.CellFormat.Borders.Left.LineWidth = 4.0;
            builder.CellFormat.Borders.Right.LineWidth = 4.0;
            builder.CellFormat.Borders.Top.LineWidth = 4.0;
            builder.CellFormat.Borders.Bottom.LineWidth = 4.0;

            builder.InsertCell();
            builder.Writeln("Row 2, Cell 1.");
            builder.InsertCell();
            builder.Writeln("Row 2, Cell 2.");

            doc.Save(ArtifactsDir + "DocumentBuilder.TableBordersAndShading.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.TableBordersAndShading.docx");
            table = (Table) doc.GetChild(NodeType.Table, 0, true);

            foreach (Cell c in table.FirstRow)
            {
                Assert.AreEqual(0.5d, c.CellFormat.Borders.Top.LineWidth);
                Assert.AreEqual(0.5d, c.CellFormat.Borders.Bottom.LineWidth);
                Assert.AreEqual(0.5d, c.CellFormat.Borders.Left.LineWidth);
                Assert.AreEqual(0.5d, c.CellFormat.Borders.Right.LineWidth);

                Assert.AreEqual(Color.Empty.ToArgb(), c.CellFormat.Borders.Left.Color.ToArgb());
                Assert.AreEqual(LineStyle.Single, c.CellFormat.Borders.Left.LineStyle);
            }

            Assert.AreEqual(Color.LightSkyBlue.ToArgb(),
                table.FirstRow.FirstCell.CellFormat.Shading.BackgroundPatternColor.ToArgb());
            Assert.AreEqual(Color.Orange.ToArgb(),
                table.FirstRow.Cells[1].CellFormat.Shading.BackgroundPatternColor.ToArgb());

            foreach (Cell c in table.LastRow)
            {
                Assert.AreEqual(4.0d, c.CellFormat.Borders.Top.LineWidth);
                Assert.AreEqual(4.0d, c.CellFormat.Borders.Bottom.LineWidth);
                Assert.AreEqual(4.0d, c.CellFormat.Borders.Left.LineWidth);
                Assert.AreEqual(4.0d, c.CellFormat.Borders.Right.LineWidth);

                Assert.AreEqual(Color.Empty.ToArgb(), c.CellFormat.Borders.Left.Color.ToArgb());
                Assert.AreEqual(LineStyle.Single, c.CellFormat.Borders.Left.LineStyle);
                Assert.AreEqual(Color.Empty.ToArgb(), c.CellFormat.Shading.BackgroundPatternColor.ToArgb());
            }
        }

        [Test]
        public void SetPreferredTypeConvertUtil()
        {
            //ExStart
            //ExFor:PreferredWidth.FromPoints
            //ExSummary:Shows how to use unit conversion tools while specifying a preferred width for a cell.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(ConvertUtil.InchToPoint(3));
            builder.InsertCell();

            Assert.AreEqual(216.0d, table.FirstRow.FirstCell.CellFormat.PreferredWidth.Value);
            //ExEnd
        }

        [Test]
        public void InsertHyperlinkToLocalBookmark()
        {
            //ExStart
            //ExFor:DocumentBuilder.StartBookmark
            //ExFor:DocumentBuilder.EndBookmark
            //ExFor:DocumentBuilder.InsertHyperlink
            //ExSummary:Shows how to insert a hyperlink which references a local bookmark.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartBookmark("Bookmark1");
            builder.Write("Bookmarked text. ");
            builder.EndBookmark("Bookmark1");
            builder.Writeln("Text outside of the bookmark.");

            // Insert a HYPERLINK field which links to the bookmark. We can pass field switches
            // to the InsertHyperlink method as part of the argument which contains the referenced bookmark's name.
            builder.Font.Color = Color.Blue;
            builder.Font.Underline = Underline.Single;
            builder.InsertHyperlink("Link to Bookmark1", @"Bookmark1"" \o ""Hyperlink Tip", true);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertHyperlinkToLocalBookmark.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertHyperlinkToLocalBookmark.docx");
            FieldHyperlink hyperlink = (FieldHyperlink)doc.Range.Fields[0];

            TestUtil.VerifyField(FieldType.FieldHyperlink, " HYPERLINK \\l \"Bookmark1\" \\o \"Hyperlink Tip\" ", "Link to Bookmark1", hyperlink);
            Assert.AreEqual("Bookmark1", hyperlink.SubAddress);
            Assert.AreEqual("Hyperlink Tip", hyperlink.ScreenTip);
            Assert.IsTrue(doc.Range.Bookmarks.Any(b => b.Name == "Bookmark1"));
        }

        [Test]
        public void CursorPosition()
        {
            // Write some text in a blank Document using a DocumentBuilder
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("Hello world!");

            // If the builder's cursor is at the end of the document, there will be no nodes in front of it so the current node will be null
            Assert.Null(builder.CurrentNode);

            // However, the current paragraph the cursor is in will be valid
            Assert.AreEqual("Hello world!", builder.CurrentParagraph.GetText().Trim());

            // Move to the beginning of the document and place the cursor at an existing node
            builder.MoveToDocumentStart();          
            Assert.AreEqual(NodeType.Run, builder.CurrentNode.NodeType);
        }

        [Test]
        public void MoveTo()
        {
            //ExStart
            //ExFor:Story.LastParagraph
            //ExFor:DocumentBuilder.MoveTo(Node)
            //ExSummary:Shows how to move a DocumentBuilder's cursor position to a specified node.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("Run 1. ");

            // The document builder has a cursor, which acts as the part of the document
            // where new nodes are appended when we use the builder's document construction methods.
            // This cursor functions in the same way as Microsoft Word's blinking cursor,
            // and it also always ends up immediately after any node that the builder just inserted. 
            // To append content to a different part of the document,
            // we can move the cursor to a different node with the MoveTo method.
            Assert.AreEqual(doc.FirstSection.Body.LastParagraph, builder.CurrentParagraph); //ExSkip
            builder.MoveTo(doc.FirstSection.Body.FirstParagraph.Runs[0]);
            Assert.AreEqual(doc.FirstSection.Body.FirstParagraph, builder.CurrentParagraph); //ExSkip

            // The cursor is now in front of the node that we moved it to.
            // Adding a second run will insert it in front of the first run.
            builder.Writeln("Run 2. ");

            Assert.AreEqual("Run 2. \rRun 1.", doc.GetText().Trim());

            // Move the cursor to the end of the document to continue appending text to the end as before.
            builder.MoveTo(doc.LastSection.Body.LastParagraph);
            builder.Writeln("Run 3. ");

            Assert.AreEqual("Run 2. \rRun 1. \rRun 3.", doc.GetText().Trim());
            Assert.AreEqual(doc.FirstSection.Body.LastParagraph, builder.CurrentParagraph); //ExSkip
            //ExEnd
        }

        [Test]
        public void MoveToDocumentStartEnd()
        {
            Document doc = new Document(MyDir + "Document.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentEnd();
            builder.Writeln("This is the end of the document.");

            builder.MoveToDocumentStart();
            builder.Writeln("This is the beginning of the document.");
        }

        [Test]
        public void MoveToSection()
        {
            // Create a blank document and append a section to it, giving it two sections
            Document doc = new Document();
            doc.AppendChild(new Section(doc));

            // Move a DocumentBuilder to the second section and add text
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToSection(1);
            builder.Writeln("Text added to the 2nd section.");
        }

        [Test]
        public void MoveToParagraph()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToParagraph
            //ExSummary:Shows how to move a cursor position to a specified paragraph.
            Document doc = new Document(MyDir + "Paragraphs.docx");
            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.AreEqual(22, paragraphs.Count);

            // Create document builder to edit the document. The builder's cursor,
            // which is the point new nodes will be inserted at when we call its document construction methods,
            // is currently at the beginning of the document.
            DocumentBuilder builder = new DocumentBuilder(doc);

            Assert.AreEqual(0, paragraphs.IndexOf(builder.CurrentParagraph));

            // Move that cursor to a different paragraph will place that cursor in front of that paragraph.
            builder.MoveToParagraph(2, 0);
            Assert.AreEqual(2, paragraphs.IndexOf(builder.CurrentParagraph)); //ExSkip

            // Any new content that we add will be inserted at that point.
            builder.Writeln("This is a new third paragraph. ");
            //ExEnd

            Assert.AreEqual(3, paragraphs.IndexOf(builder.CurrentParagraph));

            doc = DocumentHelper.SaveOpen(doc);

            Assert.AreEqual("This is a new third paragraph.", doc.FirstSection.Body.Paragraphs[2].GetText().Trim());
        }

        [Test]
        public void MoveToCell()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToCell
            //ExSummary:Shows how to move a document builder's cursor to a cell in a table.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create an empty 2x2 table.
            builder.StartTable();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndTable();

            // Because we have ended the table with the EndTable method,
            // the document builder's cursor is currently outside of the table.
            // This cursor has the same function as Microsoft Word's blinking text cursor.
            // It can also be moved to a different location in the document using the builder's MoveTo methods.
            // We can move the cursor back inside the table to a specific cell.
            builder.MoveToCell(0, 1, 1, 0);
            builder.Write("Column 2, cell 2.");

            doc.Save(ArtifactsDir + "DocumentBuilder.MoveToCell.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.MoveToCell.docx");

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual("Column 2, cell 2.\a", table.Rows[1].Cells[1].GetText().Trim());
        }

        [Test]
        public void MoveToBookmark()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToBookmark(String, Boolean, Boolean)
            //ExSummary:Shows how to move a document builder's node insertion point cursor to a bookmark.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // A valid bookmark consists of a BookmarkStart node, a BookmarkEnd node with a
            // matching bookmark name somewhere afterwards, and contents enclosed by those nodes.
            builder.StartBookmark("MyBookmark");
            builder.Write("Hello world! ");
            builder.EndBookmark("MyBookmark");

            // There are 4 ways of moving a document builder's cursor to a bookmark.
            // If we are between the BookmarkStart and BookmarkEnd nodes, the cursor will be inside the bookmark.
            // This means that any text added by the builder will become a part of the bookmark.
            // 1 -  Outside of the bookmark, in front of the BookmarkStart node:
            Assert.True(builder.MoveToBookmark("MyBookmark", true, false));
            builder.Write("1. ");

            Assert.AreEqual("Hello world! ", doc.Range.Bookmarks["MyBookmark"].Text);
            Assert.AreEqual("1. Hello world!", doc.GetText().Trim());

            // 2 -  Inside the bookmark, right after the BookmarkStart node:
            Assert.True(builder.MoveToBookmark("MyBookmark", true, true));
            builder.Write("2. ");

            Assert.AreEqual("2. Hello world! ", doc.Range.Bookmarks["MyBookmark"].Text);
            Assert.AreEqual("1. 2. Hello world!", doc.GetText().Trim());

            // 2 -  Inside the bookmark, right in front of the BookmarkEnd node:
            Assert.True(builder.MoveToBookmark("MyBookmark", false, false));
            builder.Write("3. ");

            Assert.AreEqual("2. Hello world! 3. ", doc.Range.Bookmarks["MyBookmark"].Text);
            Assert.AreEqual("1. 2. Hello world! 3.", doc.GetText().Trim());

            // 4 -  Outside of the bookmark, after the BookmarkEnd node:
            Assert.True(builder.MoveToBookmark("MyBookmark", false, true));
            builder.Write("4.");

            Assert.AreEqual("2. Hello world! 3. ", doc.Range.Bookmarks["MyBookmark"].Text);
            Assert.AreEqual("1. 2. Hello world! 3. 4.", doc.GetText().Trim());
            //ExEnd
        }

        [Test]
        public void BuildTable()
        {
            //ExStart
            //ExFor:Table
            //ExFor:DocumentBuilder.StartTable
            //ExFor:DocumentBuilder.EndRow
            //ExFor:DocumentBuilder.EndTable
            //ExFor:DocumentBuilder.CellFormat
            //ExFor:DocumentBuilder.RowFormat
            //ExFor:DocumentBuilder.Write(String)
            //ExFor:DocumentBuilder.Writeln(String)
            //ExFor:CellVerticalAlignment
            //ExFor:CellFormat.Orientation
            //ExFor:TextOrientation
            //ExFor:AutoFitBehavior
            //ExSummary:Shows how to build a formatted 2x2 table.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.Write("Row 1, cell 1.");
            builder.InsertCell();
            builder.Write("Row 1, cell 2.");
            builder.EndRow();

            // While building the table, the document builder will apply its current RowFormat/CellFormat attribute values
            // to the current row/cell that its cursor is in, as well as any new rows/cells as it creates them.
            Assert.AreEqual(CellVerticalAlignment.Center, table.Rows[0].Cells[0].CellFormat.VerticalAlignment);
            Assert.AreEqual(CellVerticalAlignment.Center, table.Rows[0].Cells[1].CellFormat.VerticalAlignment);

            builder.InsertCell();
            builder.RowFormat.Height = 100;
            builder.RowFormat.HeightRule = HeightRule.Exactly;
            builder.CellFormat.Orientation = TextOrientation.Upward;
            builder.Write("Row 2, cell 1.");
            builder.InsertCell();
            builder.CellFormat.Orientation = TextOrientation.Downward;
            builder.Write("Row 2, cell 2.");
            builder.EndRow();
            builder.EndTable();

            // Previously added rows and cells are not retroactively affected by changes to the builder's formatting.
            Assert.AreEqual(0, table.Rows[0].RowFormat.Height);
            Assert.AreEqual(HeightRule.Auto, table.Rows[0].RowFormat.HeightRule);
            Assert.AreEqual(100, table.Rows[1].RowFormat.Height);
            Assert.AreEqual(HeightRule.Exactly, table.Rows[1].RowFormat.HeightRule);
            Assert.AreEqual(TextOrientation.Upward, table.Rows[1].Cells[0].CellFormat.Orientation);
            Assert.AreEqual(TextOrientation.Downward, table.Rows[1].Cells[1].CellFormat.Orientation);

            doc.Save(ArtifactsDir + "DocumentBuilder.BuildTable.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.BuildTable.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(2, table.Rows.Count);
            Assert.AreEqual(2, table.Rows[0].Cells.Count);
            Assert.AreEqual(2, table.Rows[1].Cells.Count);

            Assert.AreEqual(0, table.Rows[0].RowFormat.Height);
            Assert.AreEqual(HeightRule.Auto, table.Rows[0].RowFormat.HeightRule);
            Assert.AreEqual(100, table.Rows[1].RowFormat.Height);
            Assert.AreEqual(HeightRule.Exactly, table.Rows[1].RowFormat.HeightRule);

            Assert.AreEqual("Row 1, cell 1.\a", table.Rows[0].Cells[0].GetText().Trim());
            Assert.AreEqual(CellVerticalAlignment.Center, table.Rows[0].Cells[0].CellFormat.VerticalAlignment);

            Assert.AreEqual("Row 1, cell 2.\a", table.Rows[0].Cells[1].GetText().Trim());

            Assert.AreEqual("Row 2, cell 1.\a", table.Rows[1].Cells[0].GetText().Trim());
            Assert.AreEqual(TextOrientation.Upward, table.Rows[1].Cells[0].CellFormat.Orientation);

            Assert.AreEqual("Row 2, cell 2.\a", table.Rows[1].Cells[1].GetText().Trim());
            Assert.AreEqual(TextOrientation.Downward, table.Rows[1].Cells[1].CellFormat.Orientation);
        }

        [Test]
        public void TableCellVerticalRotatedFarEastTextOrientation()
        {
            Document doc = new Document(MyDir + "Rotated cell text.docx");

            Table table = (Table) doc.GetChild(NodeType.Table, 0, true);
            Cell cell = table.FirstRow.FirstCell;

            Assert.AreEqual(TextOrientation.VerticalRotatedFarEast, cell.CellFormat.Orientation);

            doc = DocumentHelper.SaveOpen(doc);

            table = (Table) doc.GetChild(NodeType.Table, 0, true);
            cell = table.FirstRow.FirstCell;

            Assert.AreEqual(TextOrientation.VerticalRotatedFarEast, cell.CellFormat.Orientation);
        }

        [Test]
        public void InsertBreak()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("This is page 1.");
            builder.InsertBreak(BreakType.PageBreak);

            builder.Writeln("This is page 2.");
            builder.InsertBreak(BreakType.PageBreak);

            builder.Writeln("This is page 3.");
        }

        [Test]
        public void InsertInlineImage()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertImage(ImageDir + "Transparent background logo.png");
        }

        [Test]
        public void InsertFloatingImage()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertImage(String, RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, Double, Double, WrapType)
            //ExSummary:Shows how to insert an image.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // There are 2 ways of using a document builder to source an image, and then insert as a floating shape.
            // 1 -  From a file in the local file system:
            builder.InsertImage(ImageDir + "Transparent background logo.png", RelativeHorizontalPosition.Margin, 100,
                RelativeVerticalPosition.Margin, 0, 200, 200, WrapType.Square);

            // 2 -  From a URL:
            builder.InsertImage(AsposeLogoUrl, RelativeHorizontalPosition.Margin, 100,
                RelativeVerticalPosition.Margin, 250, 200, 200, WrapType.Square);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertFloatingImage.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertFloatingImage.docx");
            Shape image = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(400, 400, ImageType.Png, image);
            Assert.AreEqual(100.0d, image.Left);
            Assert.AreEqual(0.0d, image.Top);
            Assert.AreEqual(200.0d, image.Width);
            Assert.AreEqual(200.0d, image.Height);
            Assert.AreEqual(WrapType.Square, image.WrapType);
            Assert.AreEqual(RelativeHorizontalPosition.Margin, image.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Margin, image.RelativeVerticalPosition);

            image = (Shape)doc.GetChild(NodeType.Shape, 1, true);

            TestUtil.VerifyImageInShape(320, 320, ImageType.Png, image);
            Assert.AreEqual(100.0d, image.Left);
            Assert.AreEqual(250.0d, image.Top);
            Assert.AreEqual(200.0d, image.Width);
            Assert.AreEqual(200.0d, image.Height);
            Assert.AreEqual(WrapType.Square, image.WrapType);
            Assert.AreEqual(RelativeHorizontalPosition.Margin, image.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Margin, image.RelativeVerticalPosition);
        }

        [Test]
        public void InsertImageFromUrl()
        {
            // Insert an image from a URL
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertImage(AsposeLogoUrl);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertImageFromUrl.doc");

            // Verify that the image was inserted into the document
            Shape shape = (Shape) doc.GetChild(NodeType.Shape, 0, true);
            Assert.IsNotNull(shape);
            Assert.True(shape.HasImage);
        }

        [Test]
        public void InsertImageOriginalSize()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertImage(String, RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, Double, Double, WrapType)
            //ExSummary:Shows how to insert an image from the local file system into a document while preserving its dimensions.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // The InsertImage method creates a floating shape with the passed image in its image data.
            // We can specify the dimensions of the shape can be passing them to this method.
            Shape imageShape = builder.InsertImage(ImageDir + "Logo.jpg", RelativeHorizontalPosition.Margin, 0,
                RelativeVerticalPosition.Margin, 0, -1, -1, WrapType.Square);

            // Passing negative values as the intended dimensions will automatically define
            // the shape's dimensions based on the dimensions of its image.
            Assert.AreEqual(300.0d, imageShape.Width);
            Assert.AreEqual(300.0d, imageShape.Height);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertImageOriginalSize.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertImageOriginalSize.docx");
            imageShape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(400, 400, ImageType.Jpeg, imageShape);
            Assert.AreEqual(0.0d, imageShape.Left);
            Assert.AreEqual(0.0d, imageShape.Top);
            Assert.AreEqual(300.0d, imageShape.Width);
            Assert.AreEqual(300.0d, imageShape.Height);
            Assert.AreEqual(WrapType.Square, imageShape.WrapType);
            Assert.AreEqual(RelativeHorizontalPosition.Margin, imageShape.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Margin, imageShape.RelativeVerticalPosition);
        }

        [Test]
        public void InsertTextInput()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertTextInput
            //ExSummary:Shows how to insert a text input form field into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a form that prompts the user to enter text.
            builder.InsertTextInput("TextInput", TextFormFieldType.Regular, "", "Enter your text here", 0);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertTextInput.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertTextInput.docx");
            FormField formField = doc.Range.FormFields[0];

            Assert.True(formField.Enabled);
            Assert.AreEqual("TextInput", formField.Name);
            Assert.AreEqual(0, formField.MaxLength);
            Assert.AreEqual("Enter your text here", formField.Result);
            Assert.AreEqual(FieldType.FieldFormTextInput, formField.Type);
            Assert.AreEqual("", formField.TextInputFormat);
            Assert.AreEqual(TextFormFieldType.Regular, formField.TextInputType);
        }

        [Test]
        public void InsertComboBox()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertComboBox
            //ExSummary:Shows how to insert a combo box form field into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a form that prompts the user to pick one of the items from the menu.
            builder.Write("Pick a fruit: ");
            string[] items = { "Apple", "Banana", "Cherry" };
            builder.InsertComboBox("DropDown", items, 0);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertComboBox.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertComboBox.docx");
            FormField formField = doc.Range.FormFields[0];

            Assert.True(formField.Enabled);
            Assert.AreEqual("DropDown", formField.Name);
            Assert.AreEqual(0, formField.DropDownSelectedIndex);
            Assert.AreEqual(items, formField.DropDownItems);
            Assert.AreEqual(FieldType.FieldFormDropDown, formField.Type);
        }

        [Test]
        public void DocumentBuilderInsertToc()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a table of contents at the beginning of the document
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");

            // The newly inserted table of contents will be initially empty
            // It needs to be populated by updating the fields in the document
            doc.UpdateFields();
        }

        [Test]
        [Description("WORDSNET-16868")]
        public void SignatureLineProviderId()
        {
            //ExStart
            //ExFor:SignatureLine.ProviderId
            //ExFor:SignatureLineOptions.ShowDate
            //ExFor:SignatureLineOptions.Email
            //ExFor:SignatureLineOptions.DefaultInstructions
            //ExFor:SignatureLineOptions.Instructions
            //ExFor:SignatureLineOptions.AllowComments
            //ExFor:DocumentBuilder.InsertSignatureLine(SignatureLineOptions)
            //ExFor:SignOptions.ProviderId
            //ExSummary:Shows how to sign a document with a personal certificate and a signature line.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            SignatureLineOptions signatureLineOptions = new SignatureLineOptions
            {
                Signer = "vderyushev",
                SignerTitle = "QA",
                Email = "vderyushev@aspose.com",
                ShowDate = true,
                DefaultInstructions = false,
                Instructions = "Please sign here.",
                AllowComments = true
            };

            SignatureLine signatureLine = builder.InsertSignatureLine(signatureLineOptions).SignatureLine;
            signatureLine.ProviderId = Guid.Parse("CF5A7BB4-8F3C-4756-9DF6-BEF7F13259A2");
            
            doc.Save(ArtifactsDir + "DocumentBuilder.SignatureLineProviderId.docx");

            SignOptions signOptions = new SignOptions
            {
                SignatureLineId = signatureLine.Id,
                ProviderId = signatureLine.ProviderId,
                Comments = "Document was signed by vderyushev",
                SignTime = DateTime.Now
            };

            CertificateHolder certHolder = CertificateHolder.Create(MyDir + "morzal.pfx", "aw");

            DigitalSignatureUtil.Sign(ArtifactsDir + "DocumentBuilder.SignatureLineProviderId.docx", 
                ArtifactsDir + "DocumentBuilder.SignatureLineProviderId.Signed.docx", certHolder, signOptions);
            //ExEnd
            
            doc = new Document(ArtifactsDir + "DocumentBuilder.SignatureLineProviderId.Signed.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            signatureLine = shape.SignatureLine;

            Assert.AreEqual("vderyushev", signatureLine.Signer);
            Assert.AreEqual("QA", signatureLine.SignerTitle);
            Assert.AreEqual("vderyushev@aspose.com", signatureLine.Email);
            Assert.True(signatureLine.ShowDate);
            Assert.False(signatureLine.DefaultInstructions);
            Assert.AreEqual("Please sign here.", signatureLine.Instructions);
            Assert.True(signatureLine.AllowComments);
            Assert.True(signatureLine.IsSigned);
            Assert.True(signatureLine.IsValid);

            DigitalSignatureCollection signatures = DigitalSignatureUtil.LoadSignatures(
                ArtifactsDir + "DocumentBuilder.SignatureLineProviderId.Signed.docx");

            Assert.AreEqual(1, signatures.Count);
            Assert.True(signatures[0].IsValid);
            Assert.AreEqual("Document was signed by vderyushev", signatures[0].Comments);
            Assert.AreEqual(DateTime.Today, signatures[0].SignTime.Date);
            Assert.AreEqual("CN=Morzal.Me", signatures[0].IssuerName);
            Assert.AreEqual(DigitalSignatureType.XmlDsig, signatures[0].SignatureType);

        }

        [Test]
        public void SignatureLineInline()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertSignatureLine(SignatureLineOptions, RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, WrapType)
            //ExSummary:Shows how to insert an inline signature line into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            SignatureLineOptions options = new SignatureLineOptions
            {
                Signer = "John Doe",
                SignerTitle = "Manager",
                Email = "johndoe@aspose.com",
                ShowDate = true,
                DefaultInstructions = false,
                Instructions = "Please sign here.",
                AllowComments = true
            };

            builder.InsertSignatureLine(options, RelativeHorizontalPosition.RightMargin, 2.0,
                RelativeVerticalPosition.Page, 3.0, WrapType.Inline);

            // The signature line can be signed in Microsoft Word by double clicking it.
            doc.Save(ArtifactsDir + "DocumentBuilder.SignatureLineInline.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.SignatureLineInline.docx");

            Shape shape = (Shape) doc.GetChild(NodeType.Shape, 0, true);
            SignatureLine signatureLine = shape.SignatureLine;

            Assert.AreEqual("John Doe", signatureLine.Signer);
            Assert.AreEqual("Manager", signatureLine.SignerTitle);
            Assert.AreEqual("johndoe@aspose.com", signatureLine.Email);
            Assert.True(signatureLine.ShowDate);
            Assert.False(signatureLine.DefaultInstructions);
            Assert.AreEqual("Please sign here.", signatureLine.Instructions);
            Assert.True(signatureLine.AllowComments);
            Assert.False(signatureLine.IsSigned);
            Assert.False(signatureLine.IsValid);
        }

        [Test]
        public void SetFontFormatting()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set font formatting properties
            Aspose.Words.Font font = builder.Font;
            font.Bold = true;
            font.Color = Color.DarkBlue;
            font.Italic = true;
            font.Name = "Arial";
            font.Size = 24;
            font.Spacing = 5;
            font.Underline = Underline.Double;

            // Output formatted text
            builder.Writeln("I'm a very nice formatted String.");
        }

        [Test]
        public void SetParagraphFormatting()
        {
            //ExStart
            //ExFor:ParagraphFormat.RightIndent
            //ExFor:ParagraphFormat.LeftIndent
            //ExSummary:Shows how to configure paragraph formatting to create off-center text.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Center all text that the document builder writes, and set up indents.
            // The indent configuration below will create a body of text that will sit asymmetrically on the page.
            // The "center" that the text is aligned to will be the middle of the body of text, not the middle of the page.
            ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            paragraphFormat.Alignment = ParagraphAlignment.Center;
            paragraphFormat.LeftIndent = 100;
            paragraphFormat.RightIndent = 50;
            paragraphFormat.SpaceAfter = 25;

            builder.Writeln(
                "This paragraph demonstrates how left and right indentation affects word wrapping.");
            builder.Writeln(
                "The space between the above paragraph and this one depends on the DocumentBuilder's paragraph format.");

            doc.Save(ArtifactsDir + "DocumentBuilder.SetParagraphFormatting.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.SetParagraphFormatting.docx");

            foreach (Paragraph paragraph in doc.FirstSection.Body.Paragraphs)
            {
                Assert.AreEqual(ParagraphAlignment.Center, paragraph.ParagraphFormat.Alignment);
                Assert.AreEqual(100.0d, paragraph.ParagraphFormat.LeftIndent);
                Assert.AreEqual(50.0d, paragraph.ParagraphFormat.RightIndent);
                Assert.AreEqual(25.0d, paragraph.ParagraphFormat.SpaceAfter);
            }
        }

        [Test]
        public void SetCellFormatting()
        {
            //ExStart
            //ExFor:DocumentBuilder.CellFormat
            //ExFor:CellFormat.Width
            //ExFor:CellFormat.LeftPadding
            //ExFor:CellFormat.RightPadding
            //ExFor:CellFormat.TopPadding
            //ExFor:CellFormat.BottomPadding
            //ExFor:DocumentBuilder.StartTable
            //ExFor:DocumentBuilder.EndTable
            //ExSummary:Shows how to format cells with a document builder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Row 1, cell 1.");

            // Insert a second cell, and then configure cell text padding options. The settings will be applied to
            // the builder's current cell, as well as any new cells created by the builder.
            builder.InsertCell();

            CellFormat cellFormat = builder.CellFormat;
            cellFormat.Width = 250;
            cellFormat.LeftPadding = 30;
            cellFormat.RightPadding = 30;
            cellFormat.TopPadding = 30;
            cellFormat.BottomPadding = 30;

            builder.Write("Row 1, cell 2.");
            builder.EndRow();
            builder.EndTable();

            // The first cell was unaffected by the padding reconfiguration, and still holds the default values.
            Assert.AreEqual(0.0d, table.FirstRow.Cells[0].CellFormat.Width);
            Assert.AreEqual(5.4d, table.FirstRow.Cells[0].CellFormat.LeftPadding);
            Assert.AreEqual(5.4d, table.FirstRow.Cells[0].CellFormat.RightPadding);
            Assert.AreEqual(0.0d, table.FirstRow.Cells[0].CellFormat.TopPadding);
            Assert.AreEqual(0.0d, table.FirstRow.Cells[0].CellFormat.BottomPadding);

            Assert.AreEqual(250.0d, table.FirstRow.Cells[1].CellFormat.Width);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.LeftPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.RightPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.TopPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.BottomPadding);

            // The first cell will still grow in the output document to match the size of its neighboring cell.
            doc.Save(ArtifactsDir + "DocumentBuilder.SetCellFormatting.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.SetCellFormatting.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(159.3d, table.FirstRow.Cells[0].CellFormat.Width);
            Assert.AreEqual(5.4d, table.FirstRow.Cells[0].CellFormat.LeftPadding);
            Assert.AreEqual(5.4d, table.FirstRow.Cells[0].CellFormat.RightPadding);
            Assert.AreEqual(0.0d, table.FirstRow.Cells[0].CellFormat.TopPadding);
            Assert.AreEqual(0.0d, table.FirstRow.Cells[0].CellFormat.BottomPadding);

            Assert.AreEqual(310.0d, table.FirstRow.Cells[1].CellFormat.Width);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.LeftPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.RightPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.TopPadding);
            Assert.AreEqual(30.0d, table.FirstRow.Cells[1].CellFormat.BottomPadding);
        }

        [Test]
        public void SetRowFormatting()
        {
            //ExStart
            //ExFor:DocumentBuilder.RowFormat
            //ExFor:HeightRule
            //ExFor:RowFormat.Height
            //ExFor:RowFormat.HeightRule
            //ExSummary:Shows how to format rows with a document builder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Row 1, cell 1.");

            // Start a second row, and then configure its height. The settings will be applied to
            // the builder's current row, as well as any new rows created by the builder.
            builder.EndRow();

            RowFormat rowFormat = builder.RowFormat;
            rowFormat.Height = 100;
            rowFormat.HeightRule = HeightRule.Exactly;

            builder.InsertCell();
            builder.Write("Row 2, cell 1.");
            builder.EndTable();

            // The first row was unaffected by the padding reconfiguration, and still holds the default values.
            Assert.AreEqual(0.0d, table.Rows[0].RowFormat.Height);
            Assert.AreEqual(HeightRule.Auto, table.Rows[0].RowFormat.HeightRule);

            Assert.AreEqual(100.0d, table.Rows[1].RowFormat.Height);
            Assert.AreEqual(HeightRule.Exactly, table.Rows[1].RowFormat.HeightRule);

            doc.Save(ArtifactsDir + "DocumentBuilder.SetRowFormatting.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.SetRowFormatting.docx");
            table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.AreEqual(0.0d, table.Rows[0].RowFormat.Height);
            Assert.AreEqual(HeightRule.Auto, table.Rows[0].RowFormat.HeightRule);

            Assert.AreEqual(100.0d, table.Rows[1].RowFormat.Height);
            Assert.AreEqual(HeightRule.Exactly, table.Rows[1].RowFormat.HeightRule);
        }

        [Test]
        public void SetListFormatting()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.ApplyNumberDefault();

            builder.Writeln("Item 1");
            builder.Writeln("Item 2");

            builder.ListFormat.ListIndent();

            builder.Writeln("Item 2.1");
            builder.Writeln("Item 2.2");

            builder.ListFormat.ListIndent();

            builder.Writeln("Item 2.2.1");
            builder.Writeln("Item 2.2.2");

            builder.ListFormat.ListOutdent();

            builder.Writeln("Item 2.3");

            builder.ListFormat.ListOutdent();

            builder.Writeln("Item 3");

            builder.ListFormat.RemoveNumbers();
        }

        [Test]
        public void SetSectionFormatting()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set page properties
            builder.PageSetup.Orientation = Orientation.Landscape;
            builder.PageSetup.LeftMargin = 50;
            builder.PageSetup.PaperSize = PaperSize.Paper10x14;
        }

        [Test]
        public void InsertFootnote()
        {
            //ExStart
            //ExFor:FootnoteType
            //ExFor:DocumentBuilder.InsertFootnote(FootnoteType,String)
            //ExFor:DocumentBuilder.InsertFootnote(FootnoteType,String,String)
            //ExSummary:Shows how to reference text with a footnote and an endnote.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            
            // Insert some text and mark it with a footnote with the IsAuto attribute set to "true" by default,
            // so the marker seen in the body text will be auto-numbered at "1", and the footnote will appear at the bottom of the page
            builder.Write("This text will be referenced by a footnote.");
            builder.InsertFootnote(FootnoteType.Footnote, "Footnote comment regarding referenced text.");

            // Insert more text and mark it with an endnote with a custom reference mark,
            // which will be used in place of the number "2" and will set "IsAuto" to false
            builder.Write("This text will be referenced by an endnote.");
            builder.InsertFootnote(FootnoteType.Endnote, "Endnote comment regarding referenced text.", "CustomMark");

            // Footnotes always appear at the bottom of the page of their referenced text, so this page break will not affect the footnote
            // On the other hand, endnotes are always at the end of the document, so this page break will push the endnote down to the next page
            builder.InsertBreak(BreakType.PageBreak);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertFootnote.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertFootnote.docx");

            TestUtil.VerifyFootnote(FootnoteType.Footnote, true, string.Empty,
                "Footnote comment regarding referenced text.", (Footnote)doc.GetChild(NodeType.Footnote, 0, true));
            TestUtil.VerifyFootnote(FootnoteType.Endnote, false, "CustomMark",
                "CustomMark Endnote comment regarding referenced text.", (Footnote)doc.GetChild(NodeType.Footnote, 1, true));
        }

        [Test]
        public void DocumentBuilderApplyParagraphStyle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set paragraph style
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Title;

            builder.Write("Hello");
        }

        [Test]
        public void DocumentBuilderApplyBordersAndShading()
        {
            //ExStart
            //ExFor:BorderCollection.Item(BorderType)
            //ExFor:Shading
            //ExFor:TextureIndex
            //ExFor:ParagraphFormat.Shading
            //ExFor:Shading.Texture
            //ExFor:Shading.BackgroundPatternColor
            //ExFor:Shading.ForegroundPatternColor
            //ExSummary:Shows how to apply borders and shading to a paragraph.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set paragraph borders
            BorderCollection borders = builder.ParagraphFormat.Borders;
            borders.DistanceFromText = 20;
            borders[BorderType.Left].LineStyle = LineStyle.Double;
            borders[BorderType.Right].LineStyle = LineStyle.Double;
            borders[BorderType.Top].LineStyle = LineStyle.Double;
            borders[BorderType.Bottom].LineStyle = LineStyle.Double;

            // Set paragraph shading
            Shading shading = builder.ParagraphFormat.Shading;
            shading.Texture = TextureIndex.TextureDiagonalCross;
            shading.BackgroundPatternColor = Color.LightCoral;
            shading.ForegroundPatternColor = Color.LightSalmon;

            builder.Write("This paragraph is formatted with a double border and shading.");
            doc.Save(ArtifactsDir + "DocumentBuilder.DocumentBuilderApplyBordersAndShading.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.DocumentBuilderApplyBordersAndShading.docx");
            borders = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Borders;

            Assert.AreEqual(20.0d, borders.DistanceFromText);
            Assert.AreEqual(LineStyle.Double, borders[BorderType.Left].LineStyle);
            Assert.AreEqual(LineStyle.Double, borders[BorderType.Right].LineStyle);
            Assert.AreEqual(LineStyle.Double, borders[BorderType.Top].LineStyle);
            Assert.AreEqual(LineStyle.Double, borders[BorderType.Bottom].LineStyle);

            Assert.AreEqual(TextureIndex.TextureDiagonalCross, shading.Texture);
            Assert.AreEqual(Color.LightCoral.ToArgb(), shading.BackgroundPatternColor.ToArgb());
            Assert.AreEqual(Color.LightSalmon.ToArgb(), shading.ForegroundPatternColor.ToArgb());

        }

        [Test]
        public void DeleteRow()
        {
            //ExStart
            //ExFor:DocumentBuilder.DeleteRow
            //ExSummary:Shows how to delete a row from a table.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a table with 2 rows
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Cell 1");
            builder.InsertCell();
            builder.Write("Cell 2");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Cell 3");
            builder.InsertCell();
            builder.Write("Cell 4");
            builder.EndTable();

            Assert.AreEqual(2, table.Rows.Count);

            // Delete the first row of the first table in the document
            builder.DeleteRow(0, 0);

            Assert.AreEqual(1, table.Rows.Count);
            //ExEnd

            Assert.AreEqual("Cell 3\aCell 4\a\a", table.GetText().Trim());
        }

        [Test]
        [Ignore("Bug: does not insert headers and footers, all lists (bullets, numbering, multilevel) breaks")]
        public void InsertDocument()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertDocument(Document, ImportFormatMode)
            //ExFor:ImportFormatMode
            //ExSummary:Shows how to insert a document content into another document keep formatting of inserted document.
            Document doc = new Document(MyDir + "Document.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertBreak(BreakType.PageBreak);

            Document docToInsert = new Document(MyDir + "Formatted elements.docx");

            builder.InsertDocument(docToInsert, ImportFormatMode.KeepSourceFormatting);
            builder.Document.Save(ArtifactsDir + "DocumentBuilder.InsertDocument.docx");
            //ExEnd

            Assert.AreEqual(29, doc.Styles.Count);
            Assert.IsTrue(DocumentHelper.CompareDocs(ArtifactsDir + "DocumentBuilder.InsertDocument.docx", 
                GoldsDir + "DocumentBuilder.InsertDocument Gold.docx"));
        }

        [Test]
        public void KeepSourceNumbering()
        {
            //ExStart
            //ExFor:ImportFormatOptions.KeepSourceNumbering
            //ExFor:NodeImporter.#ctor(DocumentBase, DocumentBase, ImportFormatMode, ImportFormatOptions)
            //ExSummary:Shows how the numbering will be imported when it clashes in source and destination documents.
            // Open a document with a custom list numbering scheme and clone it
            // Since both have the same numbering format, the formats will clash if we import one document into the other
            Document srcDoc = new Document(MyDir + "Custom list numbering.docx");
            Document dstDoc = srcDoc.Clone();
            
            // Both documents have the same numbering in their lists, but if we set this flag to false and then import one document into the other
            // the numbering of the imported source document will continue from where it ends in the destination document
            ImportFormatOptions importFormatOptions = new ImportFormatOptions();
            importFormatOptions.KeepSourceNumbering = false;

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepDifferentStyles, importFormatOptions);
            foreach (Paragraph paragraph in srcDoc.FirstSection.Body.Paragraphs)
            {
                Node importedNode = importer.ImportNode(paragraph, true);
                dstDoc.FirstSection.Body.AppendChild(importedNode);
            }
            
            dstDoc.UpdateListLabels();
            dstDoc.Save(ArtifactsDir + "DocumentBuilder.KeepSourceNumbering.docx");
            //ExEnd
        }


        [Test]
        public void ResolveStyleBehaviorWhileAppendDocument()
        {
            //ExStart
            //ExFor:Document.AppendDocument(Document, ImportFormatMode, ImportFormatOptions)
            //ExSummary:Shows how to resolve styles behavior while append document.
            // Open a document with text in a custom style and clone it
            Document srcDoc = new Document(MyDir + "Custom list numbering.docx");
            Document dstDoc = srcDoc.Clone();

            // We now have two documents, each with an identical style named "CustomStyle" 
            // We can change the text color of one of the styles
            dstDoc.Styles["CustomStyle"].Font.Color = Color.DarkRed;

            ImportFormatOptions options = new ImportFormatOptions();
            // Specify that if numbering clashes in source and destination documents
            // then a numbering from the source document will be used
            options.KeepSourceNumbering = true;

            // If we join two documents which have different styles that share the same name,
            // we can resolve the style clash with an ImportFormatMode
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepDifferentStyles, options);
            dstDoc.UpdateListLabels();

            dstDoc.Save(ArtifactsDir + "DocumentBuilder.ResolveStyleBehaviorWhileAppendDocument.docx");
            //ExEnd
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IgnoreTextBoxes(bool isIgnoreTextBoxes)
        {
            //ExStart
            //ExFor:ImportFormatOptions.IgnoreTextBoxes
            //ExSummary:Shows how to manage formatting in the text boxes of the source destination during the import.
            // Create a document and add text
            Document dstDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            builder.Writeln("Hello world! Text box to follow.");

            // Create another document with a textbox, and insert some formatted text into it
            Document srcDoc = new Document();
            builder = new DocumentBuilder(srcDoc);

            Shape textBox = builder.InsertShape(ShapeType.TextBox, 300, 100);
            builder.MoveTo(textBox.FirstParagraph);
            builder.ParagraphFormat.Style.Font.Name = "Courier New";
            builder.ParagraphFormat.Style.Font.Size = 24.0d;
            builder.Write("Textbox contents");

            // When we import the document with the textbox as a node into the first document, by default the text inside the text box will keep its formatting
            // Setting the IgnoreTextBoxes flag will clear the formatting during importing of the node
            ImportFormatOptions importFormatOptions = new ImportFormatOptions();
            importFormatOptions.IgnoreTextBoxes = isIgnoreTextBoxes;

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting, importFormatOptions);

            foreach (Paragraph paragraph in srcDoc.FirstSection.Body.Paragraphs)
            {
                Node importedNode = importer.ImportNode(paragraph, true);
                dstDoc.FirstSection.Body.AppendChild(importedNode);
            }

            dstDoc.Save(ArtifactsDir + "DocumentBuilder.IgnoreTextBoxes.docx");
            //ExEnd

            dstDoc = new Document(ArtifactsDir + "DocumentBuilder.IgnoreTextBoxes.docx");
            textBox = (Shape)dstDoc.GetChild(NodeType.Shape, 0, true);

            Assert.AreEqual("Textbox contents", textBox.GetText().Trim());

            if (isIgnoreTextBoxes)
            {
                Assert.AreEqual(12.0d, textBox.FirstParagraph.Runs[0].Font.Size);
                Assert.AreEqual("Times New Roman", textBox.FirstParagraph.Runs[0].Font.Name);
            }
            else
            {
                Assert.AreEqual(24.0d, textBox.FirstParagraph.Runs[0].Font.Size);
                Assert.AreEqual("Courier New", textBox.FirstParagraph.Runs[0].Font.Name);
            }
        }

        [Test]
        public void MoveToField()
        {
            //ExStart
            //ExFor:DocumentBuilder.MoveToField
            //ExSummary:Shows how to move document builder's cursor to a specific field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a field using the DocumentBuilder and add a run of text after it
            Field field = builder.InsertField("MERGEFIELD field");
            builder.Write(" Text after the field.");

            // The builder's cursor is currently at end of the document
            Assert.Null(builder.CurrentNode);

            // We can move the builder to a field like this, placing the cursor at immediately after the field
            builder.MoveToField(field, true);

            // Note that the cursor is at a place past the FieldEnd node of the field, meaning that we are not actually inside the field
            // If we wish to move the DocumentBuilder to inside a field,
            // we will need to move it to a field's FieldStart or FieldSeparator node using the DocumentBuilder.MoveTo() method
            Assert.AreEqual(field.End, builder.CurrentNode.PreviousSibling);

            builder.Write(" Text immediately after the field.");
            //ExEnd

            doc = DocumentHelper.SaveOpen(doc);

            Assert.AreEqual("\u0013MERGEFIELD field\u0014«field»\u0015 Text immediately after the field. Text after the field.", doc.GetText().Trim());
        }

        [Test]
        public void InsertOleObjectException()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Assert.That(() => builder.InsertOleObject("", "checkbox", false, true, null),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InsertChartDouble()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertChart(ChartType, Double, Double)
            //ExSummary:Shows how to insert a chart into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertChart(ChartType.Pie, ConvertUtil.PixelToPoint(300), ConvertUtil.PixelToPoint(300));
            Assert.AreEqual(225.0d, ConvertUtil.PixelToPoint(300)); //ExSkip

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertedChartDouble.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertedChartDouble.docx");
            Shape chartShape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.AreEqual("Chart Title", chartShape.Chart.Title.Text);
            Assert.AreEqual(225.0d, chartShape.Width);
            Assert.AreEqual(225.0d, chartShape.Height);
        }

        [Test]
        public void InsertChartRelativePosition()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertChart(ChartType, RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, Double, Double, WrapType)
            //ExSummary:Shows how to insert a chart into a document and specify position and size.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertChart(ChartType.Pie, RelativeHorizontalPosition.Margin, 100, RelativeVerticalPosition.Margin,
                100, 200, 100, WrapType.Square);

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertedChartRelativePosition.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertedChartRelativePosition.docx");
            Shape chartShape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.AreEqual(100.0d, chartShape.Top);
            Assert.AreEqual(100.0d, chartShape.Left);
            Assert.AreEqual(200.0d, chartShape.Width);
            Assert.AreEqual(100.0d, chartShape.Height);
            Assert.AreEqual(WrapType.Square, chartShape.WrapType);
            Assert.AreEqual(RelativeHorizontalPosition.Margin, chartShape.RelativeHorizontalPosition);
            Assert.AreEqual(RelativeVerticalPosition.Margin, chartShape.RelativeVerticalPosition);
        }

        [Test]
        public void InsertField()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertField(String)
            //ExFor:Field
            //ExFor:Field.Update
            //ExFor:Field.Result
            //ExFor:Field.GetFieldCode
            //ExFor:Field.Type
            //ExFor:Field.Remove
            //ExFor:FieldType
            //ExSummary:Shows how to insert a field into a document by FieldCode.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a simple Date field into the document
            // When we insert a field through the DocumentBuilder class we can get the
            // special Field object which contains information about the field
            Field dateField = builder.InsertField(@"DATE \* MERGEFORMAT");

            // Update this particular field in the document so we can get the FieldResult
            dateField.Update();

            // Display some information from this field
            // The field result is where the last evaluated value is stored. This is what is displayed in the document
            // When field codes are not showing
            Assert.AreEqual(DateTime.Today, DateTime.Parse(dateField.Result));

            // Display the field code which defines the behavior of the field. This can be seen in Microsoft Word by pressing ALT+F9
            Assert.AreEqual(@"DATE \* MERGEFORMAT", dateField.GetFieldCode());

            // The field type defines what type of field in the Document this is. In this case the type is "FieldDate" 
            Assert.AreEqual(FieldType.FieldDate, dateField.Type);

            // We can also invoke the Remove method on the field to remove it from the document
            dateField.Remove();
            //ExEnd			

            Assert.AreEqual(0, doc.Range.Fields.Count);
        }

        [Test]
        public void InsertFieldByType()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertField(FieldType, Boolean)
            //ExSummary:Shows how to insert a field into a document using FieldType.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert an AUTHOR field using a DocumentBuilder
            doc.BuiltInDocumentProperties.Author = "John Doe";
            builder.Write("This document was written by ");
            builder.InsertField(FieldType.FieldAuthor, true);
            Assert.AreEqual(" AUTHOR ", doc.Range.Fields[0].GetFieldCode()); //ExSkip
            Assert.AreEqual("John Doe", doc.Range.Fields[0].Result); //ExSkip

            // Insert a PAGE field using a DocumentBuilder, but do not immediately update it
            builder.Write("\nThis is page ");
            builder.InsertField(FieldType.FieldPage, false);
            Assert.AreEqual(" PAGE ", doc.Range.Fields[1].GetFieldCode()); //ExSkip
            Assert.AreEqual("", doc.Range.Fields[1].Result); //ExSkip
            
            // Some fields types, such as ones that display document word/page counts may not keep track of their results in real time,
            // and will only display an accurate result during a field update
            // We can defer the updating of those fields until right before we need to see an accurate result
            // This method will manually update all the fields in a document
            doc.UpdateFields();

            Assert.AreEqual("1", doc.Range.Fields[1].Result);
            //ExEnd

            doc = DocumentHelper.SaveOpen(doc);

            Assert.AreEqual("This document was written by \u0013 AUTHOR \u0014John Doe\u0015" +
                            "\rThis is page \u0013 PAGE \u00141\u0015", doc.GetText().Trim());

            TestUtil.VerifyField(FieldType.FieldAuthor, " AUTHOR ", "John Doe", doc.Range.Fields[0]);
            TestUtil.VerifyField(FieldType.FieldPage, " PAGE ", "1", doc.Range.Fields[1]);
        }

        //ExStart
        //ExFor:IFieldResultFormatter
        //ExFor:IFieldResultFormatter.Format(Double, GeneralFormat)
        //ExFor:IFieldResultFormatter.Format(String, GeneralFormat)
        //ExFor:IFieldResultFormatter.FormatDateTime(DateTime, String, CalendarType)
        //ExFor:IFieldResultFormatter.FormatNumeric(Double, String)
        //ExFor:FieldOptions.ResultFormatter
        //ExFor:CalendarType
        //ExSummary:Shows how to control how the field result is formatted.
        [Test] //ExSkip
        public void FieldResultFormatting()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.FieldOptions.ResultFormatter = new FieldResultFormatter("${0}", "Date: {0}", "Item # {0}:");

            // Insert a field with a numeric format
            builder.InsertField(" = 2 + 3 \\# $###", null);

            // Insert a field with a date/time format
            builder.InsertField("DATE \\@ \"d MMMM yyyy\"", null);

            // Insert a field with a general format
            builder.InsertField("QUOTE \"2\" \\* Ordinal", null);

            // Formats will be applied and recorded by the formatter during the field update
            doc.UpdateFields();
            ((FieldResultFormatter)doc.FieldOptions.ResultFormatter).PrintInvocations();

            // Our formatter has also overridden the formats that were originally applied in the fields
            Assert.AreEqual("$5", doc.Range.Fields[0].Result);
            Assert.IsTrue(doc.Range.Fields[1].Result.StartsWith("Date: "));
            Assert.AreEqual("Item # 2:", doc.Range.Fields[2].Result);
        }

        /// <summary>
        /// Custom IFieldResult implementation that applies formats and tracks format invocations
        /// </summary>
        private class FieldResultFormatter : IFieldResultFormatter
        {
            public FieldResultFormatter(string numberFormat, string dateFormat, string generalFormat)
            {
                mNumberFormat = numberFormat;
                mDateFormat = dateFormat;
                mGeneralFormat = generalFormat;
            }

            public string FormatNumeric(double value, string format)
            {
                mNumberFormatInvocations.Add(new object[] { value, format });

                return string.IsNullOrEmpty(mNumberFormat) ? null : string.Format(mNumberFormat, value);
            }

            public string FormatDateTime(DateTime value, string format, CalendarType calendarType)
            {
                mDateFormatInvocations.Add(new object[] { value, format, calendarType });

                return string.IsNullOrEmpty(mDateFormat) ? null : string.Format(mDateFormat, value);
            }

            public string Format(string value, GeneralFormat format)
            {
                return Format((object)value, format);
            }

            public string Format(double value, GeneralFormat format)
            {
                return Format((object)value, format);
            }

            private string Format(object value, GeneralFormat format)
            {
                mGeneralFormatInvocations.Add(new object[] { value, format });

                return string.IsNullOrEmpty(mGeneralFormat) ? null : string.Format(mGeneralFormat, value);
            }

            public void PrintInvocations()
            {
                Console.WriteLine("Number format invocations ({0}):", mNumberFormatInvocations.Count);
                foreach (object[] s in mNumberFormatInvocations)
                {
                    Console.WriteLine("\tValue: " + s[0] + ", original format: " + s[1]);
                }

                Console.WriteLine("Date format invocations ({0}):", mDateFormatInvocations.Count);
                foreach (object[] s in mDateFormatInvocations)
                {
                    Console.WriteLine("\tValue: " + s[0] + ", original format: " + s[1] + ", calendar type: " + s[2]);
                }

                Console.WriteLine("General format invocations ({0}):", mGeneralFormatInvocations.Count);
                foreach (object[] s in mGeneralFormatInvocations)
                {
                    Console.WriteLine("\tValue: " + s[0] + ", original format: " + s[1]);
                }
            }

            private readonly string mNumberFormat;
            private readonly string mDateFormat;
            private readonly string mGeneralFormat;

            private readonly ArrayList mNumberFormatInvocations = new ArrayList();
            private readonly ArrayList mDateFormatInvocations = new ArrayList();
            private readonly ArrayList mGeneralFormatInvocations = new ArrayList();

        }
        //ExEnd

        [Test]
        public void InsertVideoWithUrl()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertOnlineVideo(String, Double, Double)
            //ExSummary:Shows how to insert online video into a document using video URL
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a video from Youtube
            builder.InsertOnlineVideo("https://youtu.be/t_1LYZ102RA", 360, 270);

            // Click on the shape in the output document to watch the video from Microsoft Word
            doc.Save(ArtifactsDir + "DocumentBuilder.InsertVideoWithUrl.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertVideoWithUrl.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(480, 360, ImageType.Jpeg, shape);
            TestUtil.VerifyWebResponseStatusCode(HttpStatusCode.OK, shape.HRef);

            Assert.AreEqual(360.0d, shape.Width);
            Assert.AreEqual(270.0d, shape.Height);
        }

        [Test]
        public void InsertUnderline()
        {
            //ExStart
            //ExFor:DocumentBuilder.Underline
            //ExSummary:Shows how to set and edit a document builder's underline.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set a new style for our underline
            builder.Underline = Underline.Dash;

            // Same object as DocumentBuilder.Font.Underline
            Assert.AreEqual(builder.Underline, builder.Font.Underline);
            Assert.AreEqual(Underline.Dash, builder.Font.Underline);

            // These properties will be applied to the underline as well
            builder.Font.Color = Color.Blue;
            builder.Font.Size = 32;

            builder.Writeln("Underlined text.");

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertUnderline.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertUnderline.docx");
            Run firstRun = doc.FirstSection.Body.FirstParagraph.Runs[0];

            Assert.AreEqual("Underlined text.", firstRun.GetText().Trim());
            Assert.AreEqual(Underline.Dash, firstRun.Font.Underline);
            Assert.AreEqual(Color.Blue.ToArgb(), firstRun.Font.Color.ToArgb());
            Assert.AreEqual(32.0d, firstRun.Font.Size);
        }

        [Test]
        public void CurrentStory()
        {
            //ExStart
            //ExFor:DocumentBuilder.CurrentStory
            //ExSummary:Shows how to work with a document builder's current story.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // A Story is a type of node that have child Paragraph nodes, such as a Body,
            // which would usually be a parent node to a DocumentBuilder's current paragraph
            Assert.AreEqual(builder.CurrentStory, doc.FirstSection.Body);
            Assert.AreEqual(builder.CurrentStory, builder.CurrentParagraph.ParentNode);
            Assert.AreEqual(StoryType.MainText, builder.CurrentStory.StoryType);

            builder.CurrentStory.AppendParagraph("Text added to current Story.");

            // A Story can contain tables too
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Row 1 cell 1");
            builder.InsertCell();
            builder.Write("Row 1 cell 2");
            builder.EndTable();

            // The table we just made is automatically placed in the story
            Assert.IsTrue(builder.CurrentStory.Tables.Contains(table));
            //ExEnd

            doc = DocumentHelper.SaveOpen(doc);
            Assert.AreEqual(1, doc.FirstSection.Body.Tables.Count);
            Assert.AreEqual("Row 1 cell 1\aRow 1 cell 2\a\a\rText added to current Story.", doc.FirstSection.Body.GetText().Trim());
        }

        [Test]
        public void InsertOlePowerpoint()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertOleObject(Stream, String, Boolean, Image)
            //ExSummary:Shows how to use document builder to embed Ole objects in a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a Microsoft Excel spreadsheet from the local file system into the document
            using (Stream spreadsheetStream = File.Open(MyDir + "Spreadsheet.xlsx", FileMode.Open))
            {
                // The spreadsheet can be activated by double clicking the panel in the document immediately under the text we will add
                // We did not set the area to double click as an icon nor did we change its appearance, so it looks like a simple panel
                builder.Writeln("Spreadsheet Ole object:");
                builder.InsertOleObject(spreadsheetStream, "OleObject.xlsx", false, null);

                // A Microsoft Powerpoint presentation is another type of object we can embed in our document
                using (Stream powerpointStream = File.Open(MyDir + "Presentation.pptx", FileMode.Open))
                {
                    // If we insert the Ole object as an icon, we are still provided with a default icon
                    // If that is not suitable, we can make the icon to look like any image
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imgBytes = webClient.DownloadData(AsposeLogoUrl);

                        #if NETCOREAPP2_1 || __MOBILE__
                        
                        SKBitmap bitmap = SKBitmap.Decode(imgBytes);
                        
                        // If we double click the image, the presentation will open
                        builder.InsertParagraph();
                        builder.Writeln("Powerpoint Ole object:");
                        builder.InsertOleObject(powerpointStream, "MyOleObject.pptx", true, bitmap);
                        
                        #elif NET462
                        
                        using (MemoryStream stream = new MemoryStream(imgBytes))
                        {
                            using (Image image = Image.FromStream(stream))
                            {
                                // If we double click the image, the presentation will open
                                builder.InsertParagraph();
                                builder.Writeln("Powerpoint Ole object:");
                                builder.InsertOleObject(powerpointStream, "OleObject.pptx", true, image);
                            }
                        }

                        #endif
                    }
                }
            }

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertOlePowerpoint.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertOlePowerpoint.docx");

            Assert.AreEqual(2, doc.GetChildNodes(NodeType.Shape, true).Count);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.AreEqual("", shape.OleFormat.IconCaption);
            Assert.False(shape.OleFormat.OleIcon);

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.AreEqual("Unknown", shape.OleFormat.IconCaption);
            Assert.True(shape.OleFormat.OleIcon);
        }

        [Test]
        public void InsertStyleSeparator()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertStyleSeparator
            //ExSummary:Shows how to separate styles from two different paragraphs used in one logical printed paragraph.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Append text in the "Heading 1" style
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("This text is in a Heading style. ");

            // Insert a style separator
            builder.InsertStyleSeparator();

            // The style separator appears in the form of a paragraph break that does not start a new line
            // So, while this looks like one continuous paragraph with two styles in the output document, 
            // in reality it is two paragraphs with different styles, but no line break between the first and second paragraph
            Assert.AreEqual(2, doc.FirstSection.Body.Paragraphs.Count);

            // Append text with another style
            Style paraStyle = builder.Document.Styles.Add(StyleType.Paragraph, "MyParaStyle");
            paraStyle.Font.Bold = false;
            paraStyle.Font.Size = 8;
            paraStyle.Font.Name = "Arial";

            // Set the style of the current paragraph to our custom style
            // This will apply to only the text after the style separator
            builder.ParagraphFormat.StyleName = paraStyle.Name;
            builder.Write("This text is in a custom style. ");

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertStyleSeparator.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertStyleSeparator.docx");

            Assert.AreEqual(2, doc.FirstSection.Body.Paragraphs.Count);
            Assert.AreEqual("This text is in a Heading style. \r This text is in a custom style.",
                doc.GetText().Trim());
        }

        [Test]
        public void WithoutStyleSeparator()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());

            Style paraStyle = builder.Document.Styles.Add(StyleType.Paragraph, "MyParaStyle");
            paraStyle.Font.Bold = false;
            paraStyle.Font.Size = 8;
            paraStyle.Font.Name = "Arial";

            // Append text with "Heading 1" style
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("This text is in a Heading style. ");

            // Append text with another style
            builder.ParagraphFormat.StyleName = paraStyle.Name;
            builder.Write("This text is in a custom style. ");

            builder.Document.Save(ArtifactsDir + "DocumentBuilder.WithoutStyleSeparator.docx");
        }

        [Test]
        public void SmartStyleBehavior()
        {
            //ExStart
            //ExFor:ImportFormatOptions
            //ExFor:ImportFormatOptions.SmartStyleBehavior
            //ExFor:DocumentBuilder.InsertDocument(Document, ImportFormatMode, ImportFormatOptions)
            //ExSummary:Shows how to resolve styles behavior while inserting documents.
            Document dstDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            Style myStyle = builder.Document.Styles.Add(StyleType.Paragraph, "MyStyle");
            myStyle.Font.Size = 14;
            myStyle.Font.Name = "Courier New";
            myStyle.Font.Color = Color.Blue;

            // Append text with custom style
            builder.ParagraphFormat.StyleName = myStyle.Name;
            builder.Writeln("Hello world!");

            // Clone the document, and edit the clone's "MyStyle" style so it is a different color than that of the original
            // If we append this document to the original, the different styles will clash since they are the same name, and we will need to resolve it
            Document srcDoc = dstDoc.Clone();
            srcDoc.Styles["MyStyle"].Font.Color = Color.Red;

            // When SmartStyleBehavior is enabled,
            // a source style will be expanded into a direct attribute inside a destination document,
            // if KeepSourceFormatting importing mode is used
            ImportFormatOptions options = new ImportFormatOptions();
            options.SmartStyleBehavior = true;

            builder.InsertDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);

            dstDoc.Save(ArtifactsDir + "DocumentBuilder.SmartStyleBehavior.docx");
            //ExEnd

            dstDoc = new Document(ArtifactsDir + "DocumentBuilder.SmartStyleBehavior.docx");

            Assert.AreEqual(Color.Blue.ToArgb(), dstDoc.Styles["MyStyle"].Font.Color.ToArgb());
            Assert.AreEqual("MyStyle", dstDoc.FirstSection.Body.Paragraphs[0].ParagraphFormat.Style.Name);

            Assert.AreEqual("Normal", dstDoc.FirstSection.Body.Paragraphs[1].ParagraphFormat.Style.Name);
            Assert.AreEqual(14, dstDoc.FirstSection.Body.Paragraphs[1].Runs[0].Font.Size);
            Assert.AreEqual("Courier New", dstDoc.FirstSection.Body.Paragraphs[1].Runs[0].Font.Name);
            Assert.AreEqual(Color.Red.ToArgb(), dstDoc.FirstSection.Body.Paragraphs[1].Runs[0].Font.Color.ToArgb());
        }

        #if NET462 || NETCOREAPP2_1 || JAVA
        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(1), Category("SkipTearDown")]
        public void MarkdownDocumentEmphases()
        {
            DocumentBuilder builder = new DocumentBuilder();
            
            // Bold and Italic are represented as Font.Bold and Font.Italic
            builder.Font.Italic = true;
            builder.Writeln("This text will be italic");
            
            // Use clear formatting if we don't want to combine styles between paragraphs
            builder.Font.ClearFormatting();
            
            builder.Font.Bold = true;
            builder.Writeln("This text will be bold");
            
            builder.Font.ClearFormatting();
            
            // You can also create bold and italic text
            builder.Font.Italic = true;
            builder.Write("You ");
            builder.Font.Bold = true;
            builder.Write("can");
            builder.Font.Bold = false;
            builder.Writeln(" combine them");

            builder.Font.ClearFormatting();

            builder.Font.StrikeThrough = true;
            builder.Writeln("This text will be strikethrough");
            
            // Markdown treats asterisks (*), underscores (_) and tilde (~) as indicators of emphasis
            builder.Document.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(2), Category("SkipTearDown")]
        public void MarkdownDocumentInlineCode()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);
            
            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");
            
            // Style with name that starts from word InlineCode, followed by optional dot (.) and number of backticks (`)
            // If number of backticks is missed, then one backtick will be used by default
            Style inlineCode1BackTicks = doc.Styles.Add(StyleType.Character, "InlineCode");
            builder.Font.Style = inlineCode1BackTicks;
            builder.Writeln("Text with InlineCode style with one backtick");
            
            // Use optional dot (.) and number of backticks (`)
            // There will be 3 backticks
            Style inlineCode3BackTicks = doc.Styles.Add(StyleType.Character, "InlineCode.3");
            builder.Font.Style = inlineCode3BackTicks;
            builder.Writeln("Text with InlineCode style with 3 backticks");

            builder.Document.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(3), Category("SkipTearDown")]
        [Description("WORDSNET-19850")]
        public void MarkdownDocumentHeadings()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");
            
            // By default, Heading styles in Word may have bold and italic formatting
            // If we do not want text to be emphasized, set these properties explicitly to false
            // Thus we can't use 'builder.Font.ClearFormatting()' because Bold/Italic will be set to true
            builder.Font.Bold = false;
            builder.Font.Italic = false;
            
            // Create for one heading for each level
            builder.ParagraphFormat.StyleName = "Heading 1";
            builder.Font.Italic = true;
            builder.Writeln("This is an italic H1 tag");

            // Reset our styles from the previous paragraph to not combine styles between paragraphs
            builder.Font.Bold = false;
            builder.Font.Italic = false;

            // Structure-enhanced text heading can be added through style inheritance
            Style setextHeading1 = doc.Styles.Add(StyleType.Paragraph, "SetextHeading1");
            builder.ParagraphFormat.Style = setextHeading1;
            doc.Styles["SetextHeading1"].BaseStyleName = "Heading 1";
            builder.Writeln("SetextHeading 1");
            
            builder.ParagraphFormat.StyleName = "Heading 2";
            builder.Writeln("This is an H2 tag");

            builder.Font.Bold = false;
            builder.Font.Italic = false;

            Style setextHeading2 = doc.Styles.Add(StyleType.Paragraph, "SetextHeading2");
            builder.ParagraphFormat.Style = setextHeading2;
            doc.Styles["SetextHeading2"].BaseStyleName = "Heading 2";
            builder.Writeln("SetextHeading 2");
            
            builder.ParagraphFormat.Style = doc.Styles["Heading 3"];
            builder.Writeln("This is an H3 tag");
            
            builder.Font.Bold = false;
            builder.Font.Italic = false;

            builder.ParagraphFormat.Style = doc.Styles["Heading 4"];
            builder.Font.Bold = true;
            builder.Writeln("This is an bold H4 tag");
            
            builder.Font.Bold = false;
            builder.Font.Italic = false;

            builder.ParagraphFormat.Style = doc.Styles["Heading 5"];
            builder.Font.Italic = true;
            builder.Font.Bold = true;
            builder.Writeln("This is an italic and bold H5 tag");
            
            builder.Font.Bold = false;
            builder.Font.Italic = false;

            builder.ParagraphFormat.Style = doc.Styles["Heading 6"];
            builder.Writeln("This is an H6 tag");
            
            doc.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(4), Category("SkipTearDown")]
        public void MarkdownDocumentBlockquotes()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");

            // By default, the document stores blockquote style for the first level
            builder.ParagraphFormat.StyleName = "Quote";
            builder.Writeln("Blockquote");
            
            // Create styles for nested levels through style inheritance
            Style quoteLevel2 = doc.Styles.Add(StyleType.Paragraph, "Quote1");
            builder.ParagraphFormat.Style = quoteLevel2;
            doc.Styles["Quote1"].BaseStyleName = "Quote";
            builder.Writeln("1. Nested blockquote");
            
            Style quoteLevel3 = doc.Styles.Add(StyleType.Paragraph, "Quote2");
            builder.ParagraphFormat.Style = quoteLevel3;
            doc.Styles["Quote2"].BaseStyleName = "Quote1";
            builder.Font.Italic = true;
            builder.Writeln("2. Nested italic blockquote");
            
            Style quoteLevel4 = doc.Styles.Add(StyleType.Paragraph, "Quote3");
            builder.ParagraphFormat.Style = quoteLevel4;
            doc.Styles["Quote3"].BaseStyleName = "Quote2";
            builder.Font.Italic = false;
            builder.Font.Bold = true;
            builder.Writeln("3. Nested bold blockquote");
            
            Style quoteLevel5 = doc.Styles.Add(StyleType.Paragraph, "Quote4");
            builder.ParagraphFormat.Style = quoteLevel5;
            doc.Styles["Quote4"].BaseStyleName = "Quote3";
            builder.Font.Bold = false;
            builder.Writeln("4. Nested blockquote");
            
            Style quoteLevel6 = doc.Styles.Add(StyleType.Paragraph, "Quote5");
            builder.ParagraphFormat.Style = quoteLevel6;
            doc.Styles["Quote5"].BaseStyleName = "Quote4";
            builder.Writeln("5. Nested blockquote");
            
            Style quoteLevel7 = doc.Styles.Add(StyleType.Paragraph, "Quote6");
            builder.ParagraphFormat.Style = quoteLevel7;
            doc.Styles["Quote6"].BaseStyleName = "Quote5";
            builder.Font.Italic = true;
            builder.Font.Bold = true;
            builder.Writeln("6. Nested italic bold blockquote");
            
            doc.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(5), Category("SkipTearDown")]
        public void MarkdownDocumentIndentedCode()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.Writeln("\n");
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");

            Style indentedCode = doc.Styles.Add(StyleType.Paragraph, "IndentedCode");
            builder.ParagraphFormat.Style = indentedCode;
            builder.Writeln("This is an indented code");
            
            doc.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(6), Category("SkipTearDown")]
        public void MarkdownDocumentFencedCode()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.Writeln("\n");
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");

            Style fencedCode = doc.Styles.Add(StyleType.Paragraph, "FencedCode");
            builder.ParagraphFormat.Style = fencedCode;
            builder.Writeln("This is a fenced code");

            Style fencedCodeWithInfo = doc.Styles.Add(StyleType.Paragraph, "FencedCode.C#");
            builder.ParagraphFormat.Style = fencedCodeWithInfo;
            builder.Writeln("This is a fenced code with info string");

            doc.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(7), Category("SkipTearDown")]
        public void MarkdownDocumentHorizontalRule()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");

            // Insert HorizontalRule that will be present in .md file as '-----'
            builder.InsertHorizontalRule();
 
            builder.Document.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file
        /// That's why we need order for them 
        /// </summary>
        [Test, Order(8), Category("SkipTearDown")]
        public void MarkdownDocumentBulletedList()
        {
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Prepare our created document for further work
            // And clear paragraph formatting not to use the previous styles
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln("\n");

            // Bulleted lists are represented using paragraph numbering
            builder.ListFormat.ApplyBulletDefault();
            // There can be 3 types of bulleted lists
            // The only diff in a numbering format of the very first level are ‘-’, ‘+’ or ‘*’ respectively
            builder.ListFormat.List.ListLevels[0].NumberFormat = "-";
            
            builder.Writeln("Item 1");
            builder.Writeln("Item 2");
            builder.ListFormat.ListIndent();
            builder.Writeln("Item 2a");
            builder.Writeln("Item 2b");
 
            builder.Document.Save(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
        }

        /// <summary>
        /// All markdown tests work with the same file.
        /// That's why we need order for them.
        /// </summary>
        [Test, Order(9)]
        [TestCase("Italic", "Normal", true, false, Category = "SkipTearDown")]
        [TestCase("Bold", "Normal", false, true, Category = "SkipTearDown")]
        [TestCase("ItalicBold", "Normal", true, true, Category = "SkipTearDown")]
        [TestCase("Text with InlineCode style with one backtick", "InlineCode", false, false, Category = "SkipTearDown")]
        [TestCase("Text with InlineCode style with 3 backticks", "InlineCode.3", false, false, Category = "SkipTearDown")]
        [TestCase("This is an italic H1 tag", "Heading 1", true, false, Category = "SkipTearDown")]
        [TestCase("SetextHeading 1", "SetextHeading1", false, false, Category = "SkipTearDown")]
        [TestCase("This is an H2 tag", "Heading 2", false, false, Category = "SkipTearDown")]
        [TestCase("SetextHeading 2", "SetextHeading2", false, false, Category = "SkipTearDown")]
        [TestCase("This is an H3 tag", "Heading 3", false, false, Category = "SkipTearDown")]
        [TestCase("This is an bold H4 tag", "Heading 4", false, true, Category = "SkipTearDown")]
        [TestCase("This is an italic and bold H5 tag", "Heading 5", true, true, Category = "SkipTearDown")]
        [TestCase("This is an H6 tag", "Heading 6", false, false, Category = "SkipTearDown")]
        [TestCase("Blockquote", "Quote", false, false, Category = "SkipTearDown")]
        [TestCase("1. Nested blockquote", "Quote1", false, false, Category = "SkipTearDown")]
        [TestCase("2. Nested italic blockquote", "Quote2", true, false, Category = "SkipTearDown")]
        [TestCase("3. Nested bold blockquote", "Quote3", false, true, Category = "SkipTearDown")]
        [TestCase("4. Nested blockquote", "Quote4", false, false, Category = "SkipTearDown")]
        [TestCase("5. Nested blockquote", "Quote5", false, false, Category = "SkipTearDown")]
        [TestCase("6. Nested italic bold blockquote", "Quote6", true, true, Category = "SkipTearDown")]
        [TestCase("This is an indented code", "IndentedCode", false, false, Category = "SkipTearDown")]
        [TestCase("This is a fenced code", "FencedCode", false, false, Category = "SkipTearDown")]
        [TestCase("This is a fenced code with info string", "FencedCode.C#", false, false, Category = "SkipTearDown")]
        [TestCase("Item 1", "Normal", false, false)]
        public void LoadMarkdownDocumentAndAssertContent(string text, string styleName, bool isItalic, bool isBold)
        {
            // Load created document from previous tests
            Document doc = new Document(ArtifactsDir + "DocumentBuilder.MarkdownDocument.md");
            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            
            foreach (Paragraph paragraph in paragraphs)
            {
                if (paragraph.Runs.Count != 0)
                {
                    // Check that all document text has the necessary styles
                    if (paragraph.Runs[0].Text == text && !text.Contains("InlineCode"))
                    {
                        Assert.AreEqual(styleName, paragraph.ParagraphFormat.Style.Name);
                        Assert.AreEqual(isItalic, paragraph.Runs[0].Font.Italic);
                        Assert.AreEqual(isBold, paragraph.Runs[0].Font.Bold);
                    }
                    else if (paragraph.Runs[0].Text == text && text.Contains("InlineCode"))
                    {
                        Assert.AreEqual(styleName, paragraph.Runs[0].Font.StyleName);
                    }
                }

                // Check that document also has a HorizontalRule present as a shape
                NodeCollection shapesCollection = doc.FirstSection.Body.GetChildNodes(NodeType.Shape, true);
                Shape horizontalRuleShape = (Shape) shapesCollection[0];
                
                Assert.IsTrue(shapesCollection.Count == 1);
                Assert.IsTrue(horizontalRuleShape.IsHorizontalRule);
            }
        }

        [Test]
        public void InsertOnlineVideo()
        {
            //ExStart
            //ExFor:DocumentBuilder.InsertOnlineVideo(String, String, Byte[], Double, Double)
            //ExFor:DocumentBuilder.InsertOnlineVideo(String, RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, Double, Double, WrapType)
            //ExFor:DocumentBuilder.InsertOnlineVideo(String, String, Byte[], RelativeHorizontalPosition, Double, RelativeVerticalPosition, Double, Double, Double, WrapType)
            //ExSummary:Shows how to insert online video into a document using html code.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Visible URL
            string vimeoVideoUrl = @"https://vimeo.com/52477838";

            // Embed Html code
            string vimeoEmbedCode =
                "<iframe src=\"https://player.vimeo.com/video/52477838\" width=\"640\" height=\"360\" frameborder=\"0\" title=\"Aspose\" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>";

            // This video will have an automatically generated thumbnail, and we are setting the size according to its 16:9 aspect ratio
            builder.Writeln("Video with an automatically generated thumbnail at the top left corner of the page:");
            builder.InsertOnlineVideo(vimeoVideoUrl, RelativeHorizontalPosition.LeftMargin, 0,
                RelativeVerticalPosition.TopMargin, 0, 320, 180, WrapType.Square);
            builder.InsertBreak(BreakType.PageBreak);

            // We can get an image to use as a custom thumbnail
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(AsposeLogoUrl);

                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    using (Image image = Image.FromStream(stream))
                    {
                        // This puts the video where we are with our document builder, with a custom thumbnail and size depending on the size of the image
                        builder.Writeln("Custom thumbnail at document builder's cursor:");
                        builder.InsertOnlineVideo(vimeoVideoUrl, vimeoEmbedCode, imageBytes, image.Width, image.Height);
                        builder.InsertBreak(BreakType.PageBreak);

                        // We can put the video at the bottom right edge of the page too, but we'll have to take the page margins into account 
                        double left = builder.PageSetup.RightMargin - image.Width;
                        double top = builder.PageSetup.BottomMargin - image.Height;

                        // Here we use a custom thumbnail and relative positioning to put it and the bottom right of the page
                        builder.Writeln("Bottom right of page with custom thumbnail:");

                        builder.InsertOnlineVideo(vimeoVideoUrl, vimeoEmbedCode, imageBytes,
                            RelativeHorizontalPosition.RightMargin, left, RelativeVerticalPosition.BottomMargin, top,
                            image.Width, image.Height, WrapType.Square);
                    }
                }
            }

            doc.Save(ArtifactsDir + "DocumentBuilder.InsertOnlineVideo.docx");
            //ExEnd

            doc = new Document(ArtifactsDir + "DocumentBuilder.InsertOnlineVideo.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            TestUtil.VerifyImageInShape(640, 360, ImageType.Jpeg, shape);

            Assert.AreEqual(320.0d, shape.Width);
            Assert.AreEqual(180.0d, shape.Height);
            Assert.AreEqual(0.0d, shape.Left);
            Assert.AreEqual(0.0d, shape.Top);
            Assert.AreEqual(WrapType.Square, shape.WrapType);
            Assert.AreEqual(RelativeVerticalPosition.TopMargin, shape.RelativeVerticalPosition);
            Assert.AreEqual(RelativeHorizontalPosition.LeftMargin, shape.RelativeHorizontalPosition);

            Assert.AreEqual("https://vimeo.com/52477838", shape.HRef);

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);

            TestUtil.VerifyImageInShape(320, 320, ImageType.Png, shape);
            Assert.AreEqual(320.0d, shape.Width);
            Assert.AreEqual(320.0d, shape.Height);
            Assert.AreEqual(0.0d, shape.Left);
            Assert.AreEqual(0.0d, shape.Top);
            Assert.AreEqual(WrapType.Inline, shape.WrapType);
            Assert.AreEqual(RelativeVerticalPosition.Paragraph, shape.RelativeVerticalPosition);
            Assert.AreEqual(RelativeHorizontalPosition.Column, shape.RelativeHorizontalPosition);

            Assert.AreEqual("https://vimeo.com/52477838", shape.HRef);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            TestUtil.VerifyWebResponseStatusCode(HttpStatusCode.OK, shape.HRef);
        }
#endif
    }
}