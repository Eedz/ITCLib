using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace ITCLib
{
    public class ReportFormatting
    {
        
        public ReportFormatting()
        {

        }

        public void FormatTags (Word.Application appWord, Word.Document doc, bool highlight)
        {
            FormatStyle ( doc);
            InterpretFontTags( doc);
            InterpretRichText(doc);
            if ( highlight) { InterpretHighlightTags(appWord, doc); }
            InterpretFillTags(appWord, doc);
        }

        public void FormatStyle(Word.Document doc) {
            Word.Range rng = doc.Content;
            Word.Find f = rng.Find;

            // indents
            f.Replacement.ClearFormatting();
            f.Replacement.ParagraphFormat.IndentCharWidth(1);
            FindAndReplace(doc, "\\[indent\\](*)\\[/indent\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.ParagraphFormat.IndentCharWidth(2);
            FindAndReplace(doc, "\\[indent2\\](*)\\[/indent2\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.ParagraphFormat.IndentCharWidth(3);
            FindAndReplace(doc, "\\[indent3\\](*)\\[/indent3\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            FindAndReplace(doc, "\\[center\\](*)\\[/center\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorLightBlue;
            FindAndReplace(doc, "\\[lblue\\](*)\\[/lblue\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorGreen;
            FindAndReplace(doc, "\\[green\\](*)\\[/green\\]", f);

            

            //' RED TEXT
            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorRed
            //FindAndReplace "\[red\](*)\[/red\]", f

            //' GRAY TEXT
            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorGray35
            //FindAndReplace "\[gray\](*)\[/gray\]", f


            //' FONT SIZE
            //f.Replacement.ClearFormatting
            //f.Replacement.Font.Size = 8
            //FindAndReplace "\<Font Size=8\>(*)\</Font\>", f


            //' tracked changes formatting tags
            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorLightBlue;
            FindAndReplace(doc, "\\<Font Color=Blue\\>(*)\\</Font\\>", f);


            //' tracked changes formatting tags
            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorLightBlue
            //FindAndReplace "\<Font    Color=Blue\>(*)\</Font\>", f

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorRed;
            FindAndReplace(doc, "\\<Font Color=Red\\>(*)\\</Font\\>", f);


            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorRed
            //FindAndReplace "\<Font    Color=Red\>(*)\</Font\>", f


            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorblack
            //FindAndReplace "\<Font Color=Black\>(*)\</Font\>", f


            //f.Replacement.ClearFormatting
            //f.Replacement.Font.color = wdColorGray25
            //FindAndReplace "\<Font Color=\#a6a6a6\>(*)\</Font\>", f
        }

        public void InterpretRichText(Word.Document doc)
        {
            Word.Range rng = doc.Content;
            Word.Find f = rng.Find;
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "\r\n"; // System.Environment.NewLine;
            FindAndReplace(doc, "\\<br\\>", f, true);
            // punctuation
            f.Replacement.ClearFormatting();
            f.Replacement.Text = " ";
            FindAndReplace (doc, "&nbsp;", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = ">";
            FindAndReplace (doc, "&gt;", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "<";
            FindAndReplace (doc, "&lt;", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "&";
            FindAndReplace(doc, "&amp;", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "\"";
            FindAndReplace(doc, "&quot;", f, true);

            // RTF
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "\v";
            FindAndReplace(doc, "\\<br\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "\v";
            FindAndReplace(doc, "\\<BR\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Underline = Word.WdUnderline.wdUnderlineDash;
            FindAndReplace(doc, "\\<u\\>(*)\\</u\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace (doc, "\\<blockquote\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace (doc, "\\</blockquote\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\<div\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\</div\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\<li\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\</li\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\<ul\\>", f, true);

            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";
            FindAndReplace(doc, "\\</ul\\>", f, true);

        }

        public void InterpretFontTags(Word.Document doc) {
            Word.Range rng = doc.Content;
            Word.Find f = rng.Find;
            
            // Font options
            f.Replacement.ClearFormatting();
            f.Replacement.Font.Bold = 1;
            FindAndReplace(doc, "\\<strong\\>(*)\\</strong\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Italic = 1;
            FindAndReplace(doc, "\\<em\\>(*)\\</em\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
            FindAndReplace(doc, "\\<u\\>(*)\\</u\\>", f);

            // Font colors
            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorLightBlue;
            FindAndReplace(doc, "\\<lblue\\>(*)\\</lblue\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorRed;
            FindAndReplace(doc, "\\<red\\>(*)\\</red\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorGray35;
            FindAndReplace(doc, "\\<grey\\>(*)\\</grey\\>", f);

            // HTML Font 
            f.Replacement.ClearFormatting();
            f.Replacement.Font.Color = Word.WdColor.wdColorGray35;
            FindAndReplace(doc, "\\<Font Color=#a6a6a6\\>(*)\\</Font\\>", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.Size = 8;
            FindAndReplace(doc, "\\<Font Size=8\\>(*)\\</Font\\>", f);

            // tracked changes tags

        }
        public void InterpretHighlightTags(Word.Application appWord, Word.Document doc) {
            Word.Range rng = doc.Content;
            Word.Find f = rng.Find;
            Word.WdColorIndex old = appWord.Options.DefaultHighlightColorIndex;

            appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdYellow;
            f.Replacement.ClearFormatting();
            f.Replacement.Highlight = 1;
            FindAndReplace(doc, "\\[yellow\\](*)\\[/yellow\\]", f);
            FindAndReplace(doc, "\\<font style=\"BACKGROUND-COLOR:#FFFF00\"\\>(*)\\</font\\>", f);

            // highlight based on hex value


            f.MatchWildcards = true;
            f.MatchCase = false;
            f.Replacement.ClearFormatting();

            f.Execute(@"\<font style=""BACKGROUND-COLOR:\#??????""\>(*)\</font\>");
            while (f.Found) {
                if (f.Found) {
                    // apply highlight
                    string hex = rng.Text.Substring(31, 6);
                    rng.HighlightColorIndex = GetHighlightColor(hex);


                    // remove highlight tags
                    Word.Range tagRng = doc.Range(rng.Start, rng.Start + 39);
                    tagRng.Delete();
                    tagRng = doc.Range(rng.End - 7, rng.End);
                    tagRng.Delete();
                }

                rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                f.Execute(@"\<font style=""BACKGROUND-COLOR:\#??????""\>(*)\</font\>");
            }
   

            appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdBrightGreen;
            f.Replacement.ClearFormatting();
            f.Replacement.Highlight = 1;
            FindAndReplace(doc, "\\[brightgreen\\](*)\\[/brightgreen\\]", f);

            appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdGray25;
            f.Replacement.ClearFormatting();
            f.Replacement.Highlight = 1;
            FindAndReplace(doc, "\\[grey\\](*)\\[/grey\\]", f);

            appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdTurquoise;
            f.Replacement.ClearFormatting();
            f.Replacement.Highlight = 1;
            FindAndReplace(doc, "\\[t\\](*)\\[/t\\]", f);

            f.Replacement.ClearFormatting();
            f.Replacement.Font.StrikeThrough = 1;
            FindAndReplace(doc, "\\[s\\](*)\\[/s\\]", f);

            try
            {
                rng = doc.Sections[2].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                f = rng.Find;
                old = appWord.Options.DefaultHighlightColorIndex;

                appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdYellow;
                f.Replacement.ClearFormatting();
                f.Replacement.Highlight = 1;
                FindAndReplace(doc, "\\[yellow\\](*)\\[/yellow\\]", f);
                FindAndReplace(doc, "\\<font style=\"BACKGROUND-COLOR:#FFFF00\"\\>(*)\\</font\\>", f);

                appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdBrightGreen;
                f.Replacement.ClearFormatting();
                f.Replacement.Highlight = 1;
                FindAndReplace(doc, "\\[brightgreen\\](*)\\[/brightgreen\\]", f);

                appWord.Options.DefaultHighlightColorIndex = Word.WdColorIndex.wdTurquoise;
                f.Replacement.ClearFormatting();
                f.Replacement.Highlight = 1;
                FindAndReplace(doc, "\\[t\\](*)\\[/t\\]", f);
            }
            catch
            {
            }

            // reset options
            f.Replacement.Highlight = 0;
            appWord.Options.DefaultHighlightColorIndex = old;
        }

        public void InterpretFillTags(Word.Application appWord, Word.Document doc) {
            Word.Find f;
            Word.Range rng;
            
            rng = doc.Range();
            f = rng.Find;

            f.MatchWildcards = true;
            f.Replacement.Text = "\\1";
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";

            f.Execute("\\[pinkfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);


            while (f.Found) { 
                if (f.Found) {
                    rng.Select();
                    appWord.Selection.HomeKey(Word.WdUnits.wdLine, Word.WdMovementType.wdExtend);
                    appWord.Selection.EndKey(Word.WdUnits.wdRow, Word.WdMovementType.wdExtend);
                    appWord.Selection.Shading.ForegroundPatternColor = Word.WdColor.wdColorLavender; // 16767487
                    appWord.Selection.Shading.BackgroundPatternColor = Word.WdColor.wdColorLavender; // 16767487
                }
                
                f.Execute("\\[pinkfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);
            }

            rng = doc.Range();
            f = rng.Find;

            f.MatchWildcards = true;
            f.Replacement.Text = "\\1";
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";

            f.Execute("\\[bluefill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);

            while (f.Found) {
                if (f.Found) {
                    rng.Select();
                    string found = rng.Text;
                    appWord.Selection.HomeKey(Word.WdUnits.wdLine, Word.WdMovementType.wdExtend);
                    appWord.Selection.EndKey(Word.WdUnits.wdRow, Word.WdMovementType.wdExtend);
                    appWord.Selection.Shading.ForegroundPatternColor = Word.WdColor.wdColorPaleBlue; //16769485
                    appWord.Selection.Shading.BackgroundPatternColor = Word.WdColor.wdColorPaleBlue; //16769485
                }
              
                f.Execute("\\[bluefill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);
            }

            rng = doc.Range();
            f = rng.Find;

            f.MatchWildcards = true;
            f.Replacement.Text = "\\1";
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";

            f.Execute ("\\[greenfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);


            while (f.Found)
            {
                if (f.Found)
                {
                    rng.Select();
                    appWord.Selection.HomeKey(Word.WdUnits.wdLine, Word.WdMovementType.wdExtend);
                    appWord.Selection.EndKey(Word.WdUnits.wdRow, Word.WdMovementType.wdExtend);
                    appWord.Selection.Shading.ForegroundPatternColor = Word.WdColor.wdColorLightGreen; //5950882
                    appWord.Selection.Shading.BackgroundPatternColor = Word.WdColor.wdColorLightGreen; //5950882
                }
            
                f.Execute ("\\[greenfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);
            }

            rng = doc.Range();
            f = rng.Find;

            f.MatchWildcards = true;
            f.Replacement.Text = "\\1";
            f.Replacement.ClearFormatting();
            f.Replacement.Text = "";

            f.Execute("\\[greyfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);


            while (f.Found)
            {
                if (f.Found)
                {
                    rng.Select();
                    appWord.Selection.HomeKey(Word.WdUnits.wdLine, Word.WdMovementType.wdExtend);
                    appWord.Selection.EndKey(Word.WdUnits.wdRow, Word.WdMovementType.wdExtend);
                    appWord.Selection.Shading.ForegroundPatternColor = Word.WdColor.wdColorGray25; 
                    appWord.Selection.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray25; 
                }

                f.Execute("\\[greyfill\\]", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceOne);
            }

        }
        public void ConvertTC(Word.Document doc) { }
        public void FormatShading(Word.Document doc) { }

        public void FindAndReplace (Word.Document doc, string findText, Word.Find f, bool replaceText = false)
        {
            f.MatchWildcards = true;
            if (!replaceText) f.Replacement.Text = "\\1";
            bool done = false;

            while (!done){
                f.Execute(findText, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Word.WdReplace.wdReplaceAll);
                if (!f.Found) { done = true; }
            }
            
           
        }

        
        public void FormatHeadings(Word.Document doc, bool keepVarNames, bool keepQnums, bool subheads)
        {
            string txt;
            int varCol = -1, qnumCol = -1, altQnumCol = -1;

            // determine the Qnum, AltQnum and VarName columns
            for (int i = 1; i < doc.Tables[1].Rows[1].Cells.Count; i ++)
            {
                txt = doc.Tables[1].Cell(1, i).Range.Text;
                if (txt.StartsWith("Q#") || txt.StartsWith("Qnum")) qnumCol = i;
                if (txt.StartsWith("AltQ#")) altQnumCol = i;
                if (txt.StartsWith("VarName")) varCol = i;
            }

            for (int i = 1; i <= doc.Tables[1].Rows.Count; i++)
            {

                if (doc.Tables[1].Rows[i].Cells.Count == 1)
                    continue; 

                txt = doc.Tables[1].Cell(i, varCol).Range.Text;
                txt = Utilities.RemoveHighlightTags(txt);
                txt = txt.Replace("\a", "");
                txt = txt.Replace("\r", "");

                if (!txt.StartsWith("Z"))
                    continue;

                if (txt.StartsWith("Z"))
                {
                    // set heading style and properties
                    doc.Tables[1].Rows[i].Range.Paragraphs.set_Style(Word.WdBuiltinStyle.wdStyleHeading1);
                    doc.Tables[1].Rows[i].SetHeight(20, Word.WdRowHeightRule.wdRowHeightAuto);
                    doc.Tables[1].Rows[i].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    doc.Tables[1].Rows[i].Borders.OutsideColor = Word.WdColor.wdColorBlack;
                    doc.Tables[1].Rows[i].Borders.InsideColor = Word.WdColor.wdColorBlack;
                    doc.Tables[1].Rows[i].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    doc.Tables[1].Rows[i].Range.Font.Bold = 1;
                    doc.Tables[1].Rows[i].Range.Font.Size = 12;
                    doc.Tables[1].Rows[i].Range.Font.Color = Word.WdColor.wdColorBlack;

                    if (!keepVarNames)
                        doc.Tables[1].Cell(i, varCol).Range.Text = "";
                    
                    if (!keepQnums)
                    {
                        if (qnumCol != -1) doc.Tables[1].Cell(i, qnumCol).Range.Text = "";
                        if (altQnumCol!= -1) doc.Tables[1].Cell(i, altQnumCol).Range.Text = "";
                    }
                }


                if (txt.StartsWith("Z") && txt.EndsWith("s") && subheads)
                {
                    doc.Tables[1].Rows[i].Shading.ForegroundPatternColor = Word.WdColor.wdColorSkyBlue;
                }
                else if (txt.StartsWith("Z"))
                {
                    doc.Tables[1].Rows[i].Shading.ForegroundPatternColor = Word.WdColor.wdColorRose;
                }
            }
        }

        public Word.WdColorIndex GetHighlightColor(string hex)
        {
            switch (hex)
            {
                case "000000":
                    return Word.WdColorIndex.wdBlack;
                case "0000FF":
                    return Word.WdColorIndex.wdBlue;
                case "00FFFF":
                    return Word.WdColorIndex.wdTurquoise;
                case "00008B":
                    return Word.WdColorIndex.wdDarkBlue;
                case "8B0000":
                    return Word.WdColorIndex.wdDarkRed;
                case "808000":
                    return Word.WdColorIndex.wdDarkYellow;
                case "00FF00":
                    return Word.WdColorIndex.wdBrightGreen;
                case "D3D3D3":
                    return Word.WdColorIndex.wdGray25;
                case "FFFFFF":
                    return Word.WdColorIndex.wdNoHighlight;
                case "FF0000":
                    return Word.WdColorIndex.wdRed;
                case "FFFF00":
                    return Word.WdColorIndex.wdYellow;
                        default:
                    return Word.WdColorIndex.wdYellow;
            }
        }

        public override string ToString()
        {
            PropertyInfo[] _PropertyInfos = null;
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }
    }
}

