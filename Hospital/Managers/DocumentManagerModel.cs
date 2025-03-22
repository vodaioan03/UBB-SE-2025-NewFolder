//using ABI.System;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using Document = Hospital.Models.Document;


namespace Hospital.Managers
{
    public class DocumentManagerModel
    {
        public List<Document> s_documentList { get; private set; }
        private readonly DocumentDatabaseService _documentDBService;

        public DocumentManagerModel(DocumentDatabaseService dbService)
        {
            _documentDBService = dbService;
            s_documentList = new List<Document>();
        }

        public List<Document> GetDocuments()
        {
            return s_documentList;
        }

        public async Task AddDocumentToMedicalRecord(Document document)
        {
            try
            {
                bool success = await _documentDBService.UploadDocumentToDB(document).ConfigureAwait(false);
                if (success)
                {
                    s_documentList.Add(document);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding document: {ex.Message}");
            }
        }

        public void LoadDocuments(int MedicalRecordId)
        {
            s_documentList = _documentDBService.GetDocumentsByMedicalRecordId(MedicalRecordId).Result;
        }

        public async Task DownloadDocuments(int patientId)
        {
            List<string> filePaths = new List<string>();
            foreach (Document document in s_documentList)
            {
                filePaths.Add(document.Files);
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            var fileName = Path.GetFileName(filePath);
                            var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            {
                                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                {
                                    await fileStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                        else
                        {
                            throw new DocumentNotFoundException($"Document not found at path: {filePath}");
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                var zipFile = memoryStream.ToArray();
                string zipFileName = $"Documents_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", zipFileName);
                File.WriteAllBytes(path, zipFile);

                Process.Start("explorer.exe", "/select, " + path);
            }
        }
    }
}
