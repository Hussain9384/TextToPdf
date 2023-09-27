using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TextToPdfGeneration.Interfaces;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Processors
{
    internal class WordDocGenerator : IWordDocGenerator
    {
        public void GenerateWordDocument(IEnumerable<(string fileName, DataModel dataModel)> fileNameAndContents)
        {
            string originalFilePath = "WordTemplate.docx";
            byte[] originalDocumentBytes = File.ReadAllBytes(originalFilePath);

            foreach (var file in fileNameAndContents) { 
                WriteFileStreamAsPdf(file.fileName, file.dataModel, originalDocumentBytes);
            }
        }

        public bool WriteFileStreamAsPdf(string outputPath, DataModel content, byte[] originalDocumentBytes)
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

            if (content == null)
            {
                Console.WriteLine($"file content should not be empty");
                return false;
            }

            try

            {
                GenerateWord(outputPath, content, originalDocumentBytes);
                return true;
            }
            catch (System.Exception ex)

            {
                Console.WriteLine($"PDF generation failed for {outputPath}, Exception : {ex.Message}");
                return false;
            }

        }

        private static void GenerateWord(string outputPath, DataModel content, byte[] originalDocumentBytes)
        {

            // Create a copy of the original document in memory
            using (MemoryStream originalMemoryStream = new MemoryStream(originalDocumentBytes))
            {
                using (MemoryStream copiedMemoryStream = new MemoryStream())
                {
                    // Copy the original document content to the copied memory stream
                    originalMemoryStream.CopyTo(copiedMemoryStream);

                    // Modify the copied document
                    ModifyDocument(copiedMemoryStream, content);

                    // Save the modified document to a file or do further processing as needed
                    File.WriteAllBytes($"{outputPath}.docx", copiedMemoryStream.ToArray());
                }
            }

            Console.WriteLine("Modified document saved successfully.");
        }

        private static void ModifyDocument(MemoryStream documentStream, DataModel content)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentStream, true))
            {
                // Get the main document part
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                // Load the document content
                Document document = mainPart.Document;

                // Find and replace placeholders with new values

                var properties = content.GetType().GetProperties()
                    .Select(p => new { Name = $"{p.Name.ToLower()}", Val = p.GetValue(content)?.ToString() })
                    .ToList();

                foreach (var text in document.Descendants<Text>())
                {
                    var property = properties.FirstOrDefault(s => s.Name == text.Text);

                    if (property != null)
                    {
                        text.Text = text.Text.Replace($"{property.Name}", property.Val);
                    }
                }

                mainPart.Document.Save();
            }
        }
    }
}
