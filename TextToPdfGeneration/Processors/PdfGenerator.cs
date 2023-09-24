using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToPdfGeneration.Interfaces;

namespace TextToPdfGeneration.Processors
{
    internal class PdfGenerator : IPdfGenerator
    {
        public void GeneratePdfFiles(IEnumerable<(string fileName, IEnumerable<string> fileContent)> fileWithContents)
        {
            Parallel.ForEach(fileWithContents, (file) =>
            {
                WriteFileStreamAsPdf(file.fileName, file.fileContent);
            });
        }

        public bool WriteFileStreamAsPdf(string outputPath, IEnumerable<string> fileRowContent)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {

                Console.WriteLine($"{outputPath} should not be empty");

                return false;
            }

            if (File.Exists(outputPath))
            {

                Console.WriteLine($"{outputPath} Already Exist: Hence deleting and proceeing.");

                File.Delete(outputPath);
            }

            if (fileRowContent.Count() == 0)
            {
                Console.WriteLine($"{fileRowContent} should not be empty");
                return false;
            }

            try

            {
                GeneratePdf(outputPath, fileRowContent);
                return true;
            }
            catch (System.Exception ex)

            {
                Console.WriteLine($"PDF generation failed for {outputPath}, Exception : {ex.Message}");
                return false;
            }

        }

        private static void GeneratePdf(string outputPath, IEnumerable<string> fileRowContent)
        {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputPath)))
            {
                Document document = new Document(pdfDocument);

                foreach (string rowContent in fileRowContent)
                {
                    document.Add(new Paragraph(rowContent));
                }

                document.Close();
            }

            Console.WriteLine("PDF generated successfully at: " + outputPath);
        }
    }
}
