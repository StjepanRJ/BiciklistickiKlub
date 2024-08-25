using BiciklistickiKlub.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace BiciklistickiKlub.Reports
{
    public class ClanoviReport
    {
        public byte[] Podaci { get; private set; }

        private PdfPCell GenerirajCeliju(string sadrzaj, Font font, BaseColor boja, bool wrap)
        {
            PdfPCell c1 = new PdfPCell(new Phrase(sadrzaj, font));
            c1.BackgroundColor = boja;
            c1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            c1.Padding = 5;
            c1.NoWrap = wrap;
            c1.Border = Rectangle.BOTTOM_BORDER;
            c1.BorderColor = BaseColor.LIGHT_GRAY;
            return c1;
        }

        public void ListaClanova(List<Clan> clanovi)
        {
            BaseFont bfontZaglavlje = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, false);
            BaseFont bfontText = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
            BaseFont bfontPodnozje = BaseFont.CreateFont(BaseFont.TIMES_ITALIC, BaseFont.CP1250, false);

            Font fontZaglavlje = new Font(bfontZaglavlje, 12, Font.NORMAL, BaseColor.DARK_GRAY);
            Font fontZaglavljebold = new Font(bfontZaglavlje, 12, Font.BOLD, BaseColor.DARK_GRAY);
            Font fontNaslov = new Font(bfontText, 14, Font.BOLDITALIC, BaseColor.DARK_GRAY);
            Font fontTablicZaglavlja = new Font(bfontText, 10, Font.BOLD, BaseColor.WHITE);
            Font fontTekst = new Font(bfontText, 10, Font.NORMAL, BaseColor.BLACK);

            BaseColor tPozadinaZaglavlje = new BaseColor(11, 65, 121);

            BaseColor tPozadinaSadrzaj = BaseColor.WHITE;

            using(MemoryStream mStream = new MemoryStream())
            {
                using(Document pdfDokument = new Document(PageSize.A4, 50, 50, 20, 50))
                {
                    PdfWriter.GetInstance(pdfDokument, mStream).CloseStream = false;

                    pdfDokument.Open();

                    PdfPTable tZaglavlje = new PdfPTable(2);
                    tZaglavlje.HorizontalAlignment = Element.ALIGN_LEFT;
                    tZaglavlje.DefaultCell.Border = Rectangle.NO_BORDER;
                    tZaglavlje.WidthPercentage = 100f;
                    float[] sirinaKolonaZag = new float[] { 1f, 3f };
                    tZaglavlje.SetWidths(sirinaKolonaZag);

                    var logo = iTextSharp.text.Image.GetInstance(HostingEnvironment.MapPath("~/Slike/depositphotos_31415681-stock-illustration-stylized-bicycle-vector-illustration.jpg"));

                    logo.Alignment = Element.ALIGN_LEFT;
                    logo.ScaleAbsoluteWidth(50);
                    logo.ScaleAbsoluteHeight(50);

                    PdfPCell clogo = new PdfPCell(logo);
                    clogo.Border = Rectangle.NO_BORDER;
                    tZaglavlje.AddCell(clogo);

                    Paragraph info = new Paragraph();
                    info.Alignment = Element.ALIGN_RIGHT;
                    info.SetLeading(0, 1.2f);
                    info.Add(new Chunk("BICIKLISTIČKI KLUB", fontZaglavljebold));

                    PdfPCell cInfo = new PdfPCell();
                    cInfo.AddElement(info);
                    cInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cInfo.Border = Rectangle.NO_BORDER;
                    tZaglavlje.AddCell(cInfo);

                    pdfDokument.Add(tZaglavlje);

                    Paragraph pNaslov = new Paragraph("POPIS ČLANOVA KLUBA", fontNaslov);
                    pNaslov.Alignment = Element.ALIGN_CENTER;
                    pNaslov.SpacingBefore = 20;
                    pNaslov.SpacingAfter = 20;
                    pdfDokument.Add(pNaslov);

                    PdfPTable t = new PdfPTable(5);
                    t.WidthPercentage = 100;
                    t.SetWidths(new float[] { 1, 3, 2, 3, 3 });

                    t.AddCell(GenerirajCeliju("R.br.", fontTablicZaglavlja, tPozadinaZaglavlje, true));
                    t.AddCell(GenerirajCeliju("Ime i prezime", fontTablicZaglavlja, tPozadinaZaglavlje, true));
                    t.AddCell(GenerirajCeliju("OIB", fontTablicZaglavlja, tPozadinaZaglavlje, true));
                    t.AddCell(GenerirajCeliju("Funkcija", fontTablicZaglavlja, tPozadinaZaglavlje, true));
                    t.AddCell(GenerirajCeliju("Kategorija članstva", fontTablicZaglavlja, tPozadinaZaglavlje, true));

                    int i = 1;
                    foreach ( Clan c in clanovi)
                    {
                        t.AddCell(GenerirajCeliju(i.ToString() + ".", fontTekst, tPozadinaSadrzaj, false));
                        t.AddCell(GenerirajCeliju(c.PrezimeIme, fontTekst, tPozadinaSadrzaj, false));
                        t.AddCell(GenerirajCeliju(c.Oib, fontTekst, tPozadinaSadrzaj, false));
                        t.AddCell(GenerirajCeliju(c.SifraFunkcije, fontTekst, tPozadinaSadrzaj, false));
                       // t.AddCell(GenerirajCeliju(c.KategorijaClanstva.ToString(), fontTekst, tPozadinaSadrzaj, false));
                        i++;
                    }

                    pdfDokument.Add(t);

                    pNaslov = new Paragraph("Čakovec, " + System.DateTime.Now.ToString("dd.MM.yyyy"), fontTekst);
                    pNaslov.Alignment = Element.ALIGN_RIGHT;
                    pNaslov.SpacingBefore = 30;
                    pdfDokument.Add(pNaslov);
                }
                Podaci = mStream.ToArray();

                using (var reader = new PdfReader(Podaci))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var stamper = new PdfStamper(reader, ms))
                        {
                            int PageCount = reader.NumberOfPages;
                            for (int i = 1; i <= PageCount; i++)
                            {
                                Rectangle pageSize = reader.GetPageSize(i);
                                PdfContentByte canvas = stamper.GetOverContent(i);

                                canvas.BeginText();
                                canvas.SetFontAndSize(bfontPodnozje, 10);

                                canvas.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, $"Stranica {i} / {PageCount}", pageSize.Right - 50, 30, 0);
                                canvas.EndText();
                            }
                        }
                        Podaci = ms.ToArray();
                    }
                }
            }
        }
    }
}