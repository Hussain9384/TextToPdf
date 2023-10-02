using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualBasic;
using TextToWordGeneration.Interfaces;
using TextToWordGeneration.Models;

namespace TextToWordGeneration.Processors
{
    internal class WordDocGenerator : IWordDocGenerator
    {
        public async Task<List<(string, DataModel, bool)>> GenerateWordDocument(IEnumerable<(string fileName, DataModel dataModel)> fileNameAndContents)
        {
            string originalFilePath = "WordTemplate.docx";
            byte[] originalDocumentBytes = File.ReadAllBytes(originalFilePath);

            var tasks = fileNameAndContents.Select((s, i) => WriteFileStreamAsword(i+1, $"{s.fileName}.docx", s.dataModel, originalDocumentBytes));

            var results =  await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<(string, DataModel, bool)> WriteFileStreamAsword(int iteration, string outputPath, DataModel content, byte[] originalDocumentBytes)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Console.WriteLine($"{outputPath} should not be empty");

                return (outputPath, content, false);
            }

            if (File.Exists(outputPath))
            {

                Console.WriteLine($"{outputPath} Already Exist: Hence deleting and proceeing.");

                File.Delete(outputPath);
            }

            if (content == null)
            {
                Console.WriteLine($"file content should not be empty");
                return (outputPath, content, false);
            }

            try
            {
                await GenerateWord(iteration, outputPath, content, originalDocumentBytes);
                return (outputPath, content, true);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{iteration} => word generation failed for {outputPath}, Exception : {ex.Message}");
                return (outputPath, content, false);
            }

        }

        private async Task GenerateWord(int iteration, string outputPath, DataModel content, byte[] originalDocumentBytes)
        {

            // Create a copy of the original document in memory
            using (MemoryStream originalMemoryStream = new MemoryStream(originalDocumentBytes))
            {
                using (MemoryStream copiedMemoryStream = new MemoryStream())
                {
                    // Copy the original document content to the copied memory stream
                    await originalMemoryStream.CopyToAsync(copiedMemoryStream);

                    // Modify the copied document
                    ModifyDocument(copiedMemoryStream, content);

                    // Save the modified document to a file or do further processing as needed
                    await File.WriteAllBytesAsync($"{outputPath}", copiedMemoryStream.ToArray());
                }
            }

            Console.WriteLine($"{iteration} => document saved successfully for Customer {content.Name}.");
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
