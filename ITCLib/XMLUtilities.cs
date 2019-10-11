using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ITCLib
{
    public static class XMLUtilities
    {
        public static TableCellBorders BlackSingleCellBorder()
        {
            return new TableCellBorders(
                 new TopBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new BottomBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new LeftBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new RightBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new InsideHorizontalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 },
                 new InsideVerticalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1
                 });
        }

        public static TableBorders GreySingleTableBorder()
        {
            return new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "a1a1a1"
                });

        }

        public static TableBorders BlackSingleTableBorder()
        {
            return new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 1,
                    Color = "000000"
                });

        }

        public static TableBorders NoBorder()
        {
            return new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.None),
                    Size = 1,
                    Color = "000000"
                });

        }

       public static Paragraph PageBreak()
        {
            return new Paragraph(new Run(new Break { Type = BreakValues.Page }));
        }

        public static Shading RoseShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "ffa8d4" };
        }

        public static Shading SkyBlueShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "6de2fc" };
        }

        public static Paragraph NewParagraph(string paragraphText, JustificationValues jc = JustificationValues.Center, string fontSize = "24", string fontName = "Arial")
        {
            Paragraph p = new Paragraph();

            ParagraphProperties pPr = new ParagraphProperties();
            pPr.Append(new Justification() { Val = jc });
            pPr.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });
            p.Append(pPr);

            RunProperties rPr = new RunProperties();
            rPr.Append(new RunFonts() { Ascii = fontName });
            rPr.Append(new FontSize() { Val = fontSize });

            p.Append(new Run(rPr, new Text(paragraphText)));
            return p;

        }

        // TODO add method for bold, center heading row cells

        public static SectionProperties LandscapeCenteredSectionProps()
        {
            SectionProperties sectionProperties1 = new SectionProperties();

            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape, Code = (UInt16Value)1U };
            PageMargin pageMargin1 = new PageMargin() { Top = 403, Right = (UInt32Value)499U, Bottom = 998, Left = (UInt32Value)499U, Header = (UInt32Value)0U, Footer = (UInt32Value)397U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "708" };
            VerticalTextAlignmentOnPage verticalTextAlignmentOnPage1 = new VerticalTextAlignmentOnPage() { Val = VerticalJustificationValues.Center };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };


            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(verticalTextAlignmentOnPage1);
            sectionProperties1.Append(docGrid1);
            return sectionProperties1;

        }

        public static SectionProperties LandscapeSectionProps()
        {
            SectionProperties sectionProperties1 = new SectionProperties();

            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape, Code = (UInt16Value)1U };
            PageMargin pageMargin1 = new PageMargin() { Top = 403, Right = (UInt32Value)499U, Bottom = 998, Left = (UInt32Value)499U, Header = (UInt32Value)0U, Footer = (UInt32Value)397U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "708" };
            VerticalTextAlignmentOnPage verticalTextAlignmentOnPage1 = new VerticalTextAlignmentOnPage() { Val = VerticalJustificationValues.Top };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };


            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(verticalTextAlignmentOnPage1);
            sectionProperties1.Append(docGrid1);
            return sectionProperties1;

        }

        public static Table NewTable(int columns)
        {
            Table t = new Table();
            TableGrid grid = new TableGrid();
            for (int i = 0; i < columns; i++)
                grid.Append(new GridColumn());

            t.Append(grid);
            return t;
        }

        public static Paragraph XMLToC()
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "TOC1" };

            Tabs tabs1 = new Tabs();
            TabStop tabStop1 = new TabStop() { Val = TabStopValues.Right, Leader = TabStopLeaderCharValues.Dot, Position = 14832 };

            tabs1.Append(tabStop1);

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            NoProof noProof1 = new NoProof();

            paragraphMarkRunProperties1.Append(noProof1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(tabs1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
            NoProof noProof2 = new NoProof();
            FontSize fontSize1 = new FontSize() { Val = "24" };

            runProperties1.Append(runFonts1);
            runProperties1.Append(noProof2);
            runProperties1.Append(fontSize1);
            FieldChar fieldChar1 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run1.Append(runProperties1);
            run1.Append(fieldChar1);

            Run run2 = new Run();

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
            NoProof noProof3 = new NoProof();
            FontSize fontSize2 = new FontSize() { Val = "24" };

            runProperties2.Append(runFonts2);
            runProperties2.Append(noProof3);
            runProperties2.Append(fontSize2);
            FieldCode fieldCode1 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            fieldCode1.Text = " TOC \\o \"1-3\" \\h ";

            run2.Append(runProperties2);
            run2.Append(fieldCode1);

            Run run3 = new Run();

            RunProperties runProperties3 = new RunProperties();
            RunFonts runFonts3 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
            NoProof noProof4 = new NoProof();
            FontSize fontSize3 = new FontSize() { Val = "24" };

            runProperties3.Append(runFonts3);
            runProperties3.Append(noProof4);
            runProperties3.Append(fontSize3);
            FieldChar fieldChar2 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            run3.Append(runProperties3);
            run3.Append(fieldChar2);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            paragraph1.Append(run3);


            return paragraph1;
        }

        public static Paragraph XMLToCEnd()
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Center };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
            NoProof noProof1 = new NoProof();
            FontSize fontSize1 = new FontSize() { Val = "24" };

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(noProof1);
            paragraphMarkRunProperties1.Append(fontSize1);

            SectionProperties sectionProperties1 = new SectionProperties() { RsidR = "00D903A6", RsidSect = "005D2CEA" };

            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)15840U, Height = (UInt32Value)12240U, Orient = PageOrientationValues.Landscape, Code = (UInt16Value)1U };
            PageMargin pageMargin1 = new PageMargin() { Top = 403, Right = (UInt32Value)499U, Bottom = 998, Left = (UInt32Value)499U, Header = (UInt32Value)0U, Footer = (UInt32Value)397U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "708" };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };

            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);

            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraphProperties1.Append(sectionProperties1);

            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts2 = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
            NoProof noProof2 = new NoProof();
            FontSize fontSize2 = new FontSize() { Val = "24" };

            runProperties1.Append(runFonts2);
            runProperties1.Append(noProof2);
            runProperties1.Append(fontSize2);
            FieldChar fieldChar1 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run1.Append(runProperties1);
            run1.Append(fieldChar1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }

        public static Paragraph XMLToCParagraph()
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "TOC1" };

            Tabs tabs1 = new Tabs();
            TabStop tabStop1 = new TabStop() { Val = TabStopValues.Right, Leader = TabStopLeaderCharValues.Dot, Position = 14832 };

            tabs1.Append(tabStop1);

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            NoProof noProof1 = new NoProof();

            paragraphMarkRunProperties1.Append(noProof1);

            paragraphProperties1.Append(paragraphStyleId1);
            paragraphProperties1.Append(tabs1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);
            return paragraph1;
        }

        public static Hyperlink XMLToCEntry(string headingText)
        {
            Hyperlink hyperlink1 = new Hyperlink();

            Run run4 = new Run();

            RunProperties runProperties4 = new RunProperties();
            RunStyle runStyle1 = new RunStyle() { Val = "Hyperlink" };
            Bold bold1 = new Bold();
            NoProof noProof5 = new NoProof();

            runProperties4.Append(runStyle1);
            runProperties4.Append(bold1);
            runProperties4.Append(noProof5);
            Text text1 = new Text();
            text1.Text = headingText;

            run4.Append(runProperties4);
            run4.Append(text1);

            Run run5 = new Run();

            RunProperties runProperties5 = new RunProperties();
            NoProof noProof6 = new NoProof();

            runProperties5.Append(noProof6);
            TabChar tabChar1 = new TabChar();

            run5.Append(runProperties5);
            run5.Append(tabChar1);

            Run run6 = new Run();

            RunProperties runProperties6 = new RunProperties();
            NoProof noProof7 = new NoProof();

            runProperties6.Append(noProof7);
            FieldChar fieldChar3 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

            run6.Append(runProperties6);
            run6.Append(fieldChar3);

            //Run run7 = new Run();

            //RunProperties runProperties7 = new RunProperties();
            //NoProof noProof8 = new NoProof();

            //runProperties7.Append(noProof8);
            //FieldCode fieldCode2 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
            //fieldCode2.Text = " PAGEREF _Toc21341463 \\h ";

            //run7.Append(runProperties7);
            //run7.Append(fieldCode2);

            //Run run8 = new Run();

            //RunProperties runProperties8 = new RunProperties();
            //NoProof noProof9 = new NoProof();

            //runProperties8.Append(noProof9);

            //run8.Append(runProperties8);

            Run run9 = new Run();

            RunProperties runProperties9 = new RunProperties();
            NoProof noProof10 = new NoProof();

            runProperties9.Append(noProof10);
            FieldChar fieldChar4 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

            //run9.Append(runProperties9);
            //run9.Append(fieldChar4);

            //Run run10 = new Run();

            //RunProperties runProperties10 = new RunProperties();
            //NoProof noProof11 = new NoProof();

            //runProperties10.Append(noProof11);
            //Text text2 = new Text();
            //text2.Text = "3";

            //run10.Append(runProperties10);
            //run10.Append(text2);

            Run run11 = new Run();

            RunProperties runProperties11 = new RunProperties();
            NoProof noProof12 = new NoProof();

            runProperties11.Append(noProof12);
            FieldChar fieldChar5 = new FieldChar() { FieldCharType = FieldCharValues.End };

            run11.Append(runProperties11);
            run11.Append(fieldChar5);

            hyperlink1.Append(run4);
            hyperlink1.Append(run5);
            hyperlink1.Append(run6);
            //   hyperlink1.Append(run7);
            //   hyperlink1.Append(run8);
            hyperlink1.Append(run9);
            //   hyperlink1.Append(run10);
            hyperlink1.Append(run11);
            return hyperlink1;
        }

    }
}
