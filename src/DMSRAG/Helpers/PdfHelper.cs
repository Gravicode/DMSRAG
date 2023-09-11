using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.IO;

namespace DMSRAG.Web.Helpers
{
    public class PdfHelper
    {
        public static byte[] MergePdf(List<byte[]> pdfs)
        {
            List<PdfDocument> lstDocuments = new List<PdfDocument>();
            foreach (var pdf in pdfs)
            {
                lstDocuments.Add(PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import));
            }

            using (PdfDocument outPdf = new PdfDocument())
            {
                for (int i = 1; i <= lstDocuments.Count; i++)
                {
                    foreach (PdfPage page in lstDocuments[i - 1].Pages)
                    {
                        outPdf.AddPage(page);
                    }
                }

                MemoryStream stream = new MemoryStream();
                outPdf.Save(stream, false);
                byte[] bytes = stream.ToArray();

                return bytes;
            }
        }
        public static string GetTextFromPdf(string FilePdf)
        {
            PdfDocument PDFDoc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);
            PdfDocument PDFNewDoc = new PdfDocument();
            var sb = new StringBuilder();
            foreach (var page in PDFDoc.Pages)
            {
                foreach (var text in page.ExtractText())
                {
                    sb.AppendLine(text);
                }
            }
            return sb.ToString();

        }
        public static string GetTextFromPdf(Stream FilePdf)
        {
            PdfDocument doc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);

            StringBuilder sb = new StringBuilder();
            using (PdfSharpTextExtractor.Extractor extractor = new PdfSharpTextExtractor.Extractor(doc))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    extractor.ExtractText(page, sb);
                }

            }
            return sb.ToString();

        }
        public static IEnumerable<string> GetTextPerPageFromPdf(Stream FilePdf)
        {
            PdfDocument doc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);

            StringBuilder sb = new StringBuilder();
            using (PdfSharpTextExtractor.Extractor extractor = new PdfSharpTextExtractor.Extractor(doc))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    sb.Clear();
                    extractor.ExtractText(page, sb);
                    yield return sb.ToString();
                }
            }
        }
    }
}
