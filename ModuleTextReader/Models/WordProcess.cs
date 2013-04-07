using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace TextReader
{
    public static class WordProcessor
    {
        private static XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private static XNamespace rels = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private static XNamespace a = "http://schemas.openxmlformats.org/drawingml/2006/main";
        private static XNamespace wp = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";

        private static XName w_r = w + "r";
        private static XName w_ins = w + "ins";
        private static XName w_link = w + "hyperlink";
        private static XName w_pPr = w + "pPr";
        private static XName w_p = w + "p";
        private static XName w_pStyle = w + "pStyle";
        private static XName w_body = w + "body";
        private static XName w_style = w + "style";
        private static XName w_type = w + "type";
        private static XName w_default = w + "default";
        private static XName w_styleId = w + "styleId";
        private static XName w_val = w + "val";
        private static XName w_rPr = w + "rPr";
        private static XName w_rStyle = w + "rStyle";
        private static XName w_drawing = w + "drawing";
        private static XName w_t = w + "t";
        private static XName w_br = w + "br";
        private static XName w_name = w + "name";
        private static XName w_ilvl = w + "ilvl";
        private static XName w_numId = w + "numId";
        private static XName w_numPr = w + "numPr";
        private static XName w_tr = w + "tr";
        private static XName w_tc = w + "tc";
        private static XName w_tbl = w + "tbl";
        private static XName rels_id = rels + "id";
        private static XName rels_embed = rels + "embed";
        private static XName a_blip = a + "blip";
        private static XName wp_extent = wp + "extent";

        public static void LoadFromWordML(this FlowDocument doc, string path)
        {
            using (WordprocessingDocument wdoc = WordprocessingDocument.Open(path, false))
            {
                //get default style
                XDocument xstyle, xdoc;
                using (StreamReader sr = new StreamReader(wdoc.MainDocumentPart.StyleDefinitionsPart.GetStream()))
                {
                    xstyle = XDocument.Load(sr);

                    var styles = from style in xstyle
                                     .Root
                                     .Descendants(w_style)
                                 let pPr = style
                                     .Elements(w_pPr)
                                     .FirstOrDefault()
                                 let rPr = style
                                     .Elements(w_rPr)
                                     .FirstOrDefault()
                                 select new
                                 {
                                     pStyleName = style.Attribute(w_styleId).Value,
                                     pName = style.Element(w_name).Attribute(w_val).Value,
                                     pPStyle = pPr,
                                     pRStyle = rPr
                                 };

                    foreach (var style in styles)
                    {
                        Style pStyle = style.pPStyle.ToWPFStyle();
                        pStyle.BasedOn = style.pRStyle.ToWPFStyle();

                        doc.Resources.Add(style.pStyleName, pStyle);
                    }
                }

                //get document
                using (StreamReader sr = new StreamReader(wdoc.MainDocumentPart.GetStream()))
                {
                    xdoc = XDocument.Load(sr);

                    var paragraphs = from par in xdoc
                                         .Root
                                         .Element(w_body)
                                         .Descendants(w_p)
                                     let par_style = par
                                         .Elements(w_pPr)
                                         .Elements(w_pStyle)
                                         .FirstOrDefault()
                                     let par_inline = par
                                         .Elements(w_pPr)
                                         .FirstOrDefault()
                                     let par_list = par
                                         .Elements(w_pPr)
                                         .Elements(w_numPr)
                                         .FirstOrDefault()
                                     select new
                                     {
                                         pElement = par,
                                         pStyle = par_style != null ? par_style.Attribute(w_val).Value : (from d_style in xstyle
                                                                                                               .Root
                                                                                                               .Elements(w_style)
                                                                                                          where
                                                                                                              d_style.Attribute(w_type).Value == "paragraph" &&
                                                                                                              d_style.Attribute(w_default).Value == "1"
                                                                                                          select d_style).First().Attribute(w_styleId).Value,
                                         pAttrs = par_inline,
                                         pRuns = par.Elements().Where(e => e.Name == w_r || e.Name == w_ins || e.Name == w_link || e.Name == w_numId || e.Name == w_numPr || e.Name == w_ilvl),
                                         pList = par_list
                                     };

                    foreach (var par in paragraphs)
                    {
                        Paragraph p = new Paragraph();

                        Style pStyle = par.pAttrs.ToWPFStyle();
                        if (par.pStyle != string.Empty)
                        {
                            pStyle.BasedOn = doc.Resources[par.pStyle] as Style;
                        }
                        p.Style = pStyle;

                        var runs = from run in par.pRuns
                                   let run_style = run
                                       .Elements(w_rPr)
                                       .FirstOrDefault()
                                   let run_istyle = run
                                       .Elements(w_rPr)
                                       .Elements(w_rStyle)
                                       .FirstOrDefault()
                                   let run_graph = run
                                       .Elements(w_drawing)
                                   select new
                                   {
                                       pRun = run,
                                       pRunType = run.Name.LocalName,
                                       pStyle = run_istyle != null ? run_istyle.Attribute(w_val).Value : string.Empty,
                                       pAttrs = run_style,
                                       pText = run.Descendants(w_t),
                                       pBB = run.Elements(w_br) != null,
                                       pExRelID = run.Name == w_link ? run.Attribute(rels_id).Value : string.Empty,
                                       pGraphics = run_graph
                                   };

                        foreach (var run in runs)
                        {
                            Run r = new Run();
                            Style rStyle = run.pAttrs.ToWPFStyle();
                            if (run.pStyle != string.Empty)
                            {
                                rStyle.BasedOn = doc.Resources[run.pStyle] as Style;
                            }
                            r.Style = rStyle;

                            r.Text = run.pText.ToString(txt => txt.Value);

                            if (run.pRunType == "hyperlink")
                            {
                                ExternalRelationship er = (from rel in wdoc.MainDocumentPart.ExternalRelationships
                                                           where rel.Id == run.pExRelID
                                                           select rel).FirstOrDefault() as ExternalRelationship;
                                if (er != null)
                                {
                                    Hyperlink hl = new Hyperlink(r);
                                    hl.NavigateUri = er.Uri;
                                    p.Inlines.Add(hl);
                                }
                            }
                            else
                            {
                                p.Inlines.Add(r);
                            }

                            var graphics = from graph in run.pGraphics
                                           let pBlip = graph
                                           .Descendants(a_blip)
                                           .Where(x => x.Attribute(rels_embed) != null)
                                           .FirstOrDefault()
                                           let pExtent = graph
                                           .Descendants(wp_extent)
                                           .FirstOrDefault()
                                           select new
                                           {
                                               pWidth = pExtent != null ? pExtent.Attribute("cx").Value : "0",
                                               pHeight = pExtent != null ? pExtent.Attribute("cy").Value : "0",
                                               pExRelID = pBlip != null ? pBlip.Attribute(rels_embed).Value : string.Empty
                                           };
                            foreach (var graphic in graphics)
                            {
                                Console.WriteLine(graphic);
                            }
                        }

                        if (par.pList != null)
                        {
                            int level = int.Parse(par.pList.Element(w_ilvl).Attribute(w_val).Value);
                            List lst = doc.Blocks.LastBlock as List;
                            ListItem nli = new ListItem(p);
                            if (lst != null && lst.Tag != null && (int)lst.Tag < level)
                            {
                                List nlst = new List(nli);
                                nlst.Tag = level;
                                lst.ListItems.LastListItem.Blocks.Add(nlst);
                            }
                            else
                            {
                                lst = new List(nli);
                                lst.Tag = level;
                                doc.Blocks.Add(lst);
                            }
                        }

                        else
                        {
                            doc.Blocks.Add(p);
                        }
                    }
                }
            }
        }

        internal static string ToString<T>(this IEnumerable<T> source, Func<T, string> func)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in source)
                sb.Append(func(item));
            return sb.ToString();
        }

        internal static Style ToWPFStyle(this XElement elem)
        {
            Style style = new Style();
            if (elem != null)
            {
                var setters = elem.Descendants().Select(elm =>
                {
                    Setter setter = null;
                    if (elm.Name == w + "left" || elm.Name == w + "right" || elm.Name == w + "top" || elm.Name == w + "bottom")
                    {
                        ThicknessConverter tk = new ThicknessConverter();
                        Thickness thinkness = (Thickness)tk.ConvertFrom(elm.Attribute(w + "sz").Value);

                        BrushConverter bc = new BrushConverter();
                        Brush color = (Brush)bc.ConvertFrom(string.Format("#{0}", elm.Attribute(w + "color").Value));

                        setter = new Setter(Block.BorderThicknessProperty, thinkness);

                        //style.Setters.Add(new Setter(Block.BorderBrushProperty,color));
                    }
                    else if (elm.Name == w + "rFonts")
                    {
                        FontFamilyConverter ffc = new FontFamilyConverter();
                        try
                        {
                            setter = new Setter(TextElement.FontFamilyProperty, ffc.ConvertFrom(elm.Attribute(w + "ascii").Value));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (elm.Name == w + "b")
                    {
                        setter = new Setter(TextElement.FontWeightProperty, FontWeights.Bold);
                    }
                    else if (elm.Name == w + "color")
                    {
                        BrushConverter bc = new BrushConverter();
                        setter = new Setter(TextElement.ForegroundProperty, bc.ConvertFrom(string.Format("#{0}", elm.Attribute(w_val).Value)));
                    }
                    else if (elm.Name == w + "em" || elm.Name == w + "i")
                    {
                        setter = new Setter(TextElement.FontStyleProperty, FontStyles.Italic);
                    }
                    else if (elm.Name == w + "strike")
                    {
                        setter = new Setter(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                    }
                    else if (elm.Name == w + "sz")
                    {
                        FontSizeConverter fsc = new FontSizeConverter();
                        setter = new Setter(TextElement.FontSizeProperty, fsc.ConvertFrom(elm.Attribute(w_val).Value));
                    }
                    else if (elm.Name == w + "ilvl")
                    {
                        Console.WriteLine(elm.Attribute(w_val));
                    }
                    else if (elm.Name == w + "numPr")
                    {
                        Console.WriteLine(elm.Value);
                    }
                    else if (elm.Name == w + "numId")
                    {
                        Console.WriteLine(elm.Attribute(w_val));
                    }
                    else if (elm.Name == w + "u")
                    {
                        setter = new Setter(Inline.TextDecorationsProperty, TextDecorations.Underline);
                    }
                    else
                    {
                        Console.WriteLine(elm.Name);
                    }

                    return setter;
                });

                foreach (SetterBase setter in setters)
                {
                    if (setter != null)
                    {
                        style.Setters.Add(setter);
                    }
                }
            }

            return style;
        }

        private static PackagePart GetPartDebug(this Package pkg, Uri source)
        {
            source = new Uri(string.Format("/{0}", source.OriginalString), UriKind.Relative);
            PackagePart pp = pkg.GetPart(source);
            Console.WriteLine(pp.Uri);
            return pp;
        }
    }
}