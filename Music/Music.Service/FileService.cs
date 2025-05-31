
using AutoMapper;
//using iText.Commons.Datastructures;
//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Canvas.Parser;
//using iText.Kernel.Pdf.Canvas.Parser.Data;
//using iText.Kernel.Pdf.Canvas.Parser.Listener;
//using iText.Kernel.Pdf.Xobject;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IRepositories;
using Music.Core.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using File = Music.Core.Entities.File;
//using iText.Kernel.Pdf.Canvas;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System.Net;
using Microsoft.VisualBasic;
using System.Xml;
using Tesseract;
//using Manufaktura.Controls.Parser;
using System.Xml.Linq;
using NAudio.Midi;
using NAudio.Wave;
using Ghostscript.NET.Rasterizer;

using System.Net;
using MeltySynth;

namespace Music.Service
{
    public class FileService : IFileService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IAWSService _awsService;

        public FileService(IRepositoryManager repositoryManager, IMapper mapper, IAWSService awsService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _awsService = awsService;
        }
        public async Task<IEnumerable<FileDTO>> GetAllAsync()
        {
            return _mapper.Map<List<FileDTO>>(await _repositoryManager.Files.GetAllAsync());
        }
        public async Task<IEnumerable<FileDTO>> GetFilesAsync()
        {
            var files = await _repositoryManager.Files.GetFilesAsync();
            var filesDto = _mapper.Map<List<FileDTO>>(files);
            return filesDto;
        }

        public async Task<FileDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<FileDTO>(await _repositoryManager.Files.GetByIdAsync(id));
        }
        public async Task<FileDTO> AddAsync(FileDTO fileDto)
        {
            if (!IsValidFileType(fileDto.Type) || string.IsNullOrEmpty(fileDto.Name) || fileDto.Size > 5 * 1024 * 1024)
                throw new ArgumentException();

            var file = _mapper.Map<File>(fileDto);
            fileDto = _mapper.Map<FileDTO>(await _repositoryManager.Files.AddAsync(file));
            //ConvertToSong()
            await _repositoryManager.SaveAsync();
            return fileDto;
        }
        public async Task<FileDTO> UpdateAsync(int id, FileDTO fileDto)
        {
            var f = await _repositoryManager.Files.GetByIdAsync(id);
            if (f == null)
                throw new KeyNotFoundException();
            if (!IsValidFileType(fileDto.Type) || string.IsNullOrEmpty(fileDto.Name) || fileDto.Size > 5 * 1024 * 1024)
                throw new ArgumentException();

            var file = _mapper.Map<File>(fileDto);
            fileDto = _mapper.Map<FileDTO>(await _repositoryManager.Files.UpdateAsync(id, file));
            await _repositoryManager.SaveAsync();
            return fileDto;
        }
        public async Task<FileDTO> DeleteAsync(int id)
        {
            var f = await _repositoryManager.Files.GetByIdAsync(id);
            if (f == null)
                throw new KeyNotFoundException();
            var fileDto = _mapper.Map<FileDTO>(await _repositoryManager.Files.DeleteAsync(id));
            await _repositoryManager.SaveAsync();
            return fileDto;
        }
        static bool IsValidFileType(string type)
        {
            string[] validExtensions = { "pdf", "mp3", "mpeg", "jpg", "wav" };
            type = type.ToLower().Split('/')[1];
            return Array.Exists(validExtensions, validExtension => validExtension == type);
        }


        public async Task<byte[]> ConvertToSong(string fileName,string path)
        {
            //string fileUrl = "https://www.imrikeren.com/_files/ugd/c52e4f_aa6d3b95834a4f3da7a6b2d9a9f813cb.pdf?index=true&~nfopt(fileDistorted=7428808542908334)";
            string tempFolder = Path.GetTempPath();
            string outputImagePath = Path.Combine(tempFolder, "output_image.png");
            string outputXmlPath = Path.Combine(tempFolder, "output_image.mxl");///???mxl
            string outputMidiPath = Path.Combine(tempFolder, "output_image.mid");
            string outputWavPath = Path.Combine(tempFolder, "output_image.wav");
            string step1Output, step2Output, step3Output;
            List<byte> byteList = new List<byte>();
            try
            {

                // הורדת הקובץ PDF
                using (var client = new WebClient())
                {
                    string localPdfPath = Path.Combine(tempFolder, "temp.pdf");
                    client.DownloadFile(path, localPdfPath);

                    // רינדור העמוד לתמונה
                    step1Output = PdfToImageRenderer.RenderPdfPageToImage(localPdfPath, outputImagePath);

                    Console.WriteLine("PDF page rendered to image successfully!");
                    AudiverisWrapper.ConvertImageToMusicXml(outputImagePath, tempFolder);
                    Console.WriteLine("xml successfully!");

                    AudiverisWrapper.ConvertToMidi(outputXmlPath, outputMidiPath);
                    Console.WriteLine("midi successfully!");


                    byteList = AudiverisWrapper.ConvertMidiToWav(outputMidiPath, outputWavPath);
                   
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return byteList.ToArray();
        }



    }
    public class TempFileManager
    {
        public static string CreateTemporaryDirectory(string baseFolder, string step)
        {
            string tempFolder = Path.Combine(baseFolder, step);
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            return tempFolder;
        }

        public static void DeleteTemporaryDirectory(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true); // מחיקה ריקורסיבית
                Console.WriteLine($"Temporary folder deleted: {folderPath}");
            }
        }
    }
    //level a

    public class PdfToImageRenderer
    {
        //convert pdf to image
        public static string RenderPdfPageToImage(string pdfFilePath, string outputImagePath, int pageNumber = 1, int dpi = 300)
        {
            // יצירת אובייקט GhostscriptRasterizer
            using (var rasterizer = new GhostscriptRasterizer())
            {
                // פתיחת קובץ PDF
                rasterizer.Open(pdfFilePath);

                // רינדור העמוד הרצוי (עמוד 1 כברירת מחדל)
                System.Drawing.Image image = rasterizer.GetPage(dpi, pageNumber);

                // שמירת התמונה כקובץ בפורמט PNG
                image.Save(outputImagePath, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine($"Image saved: {outputImagePath}");
            }
            return outputImagePath;
        }
        }
    //level b

    public class AudiverisWrapper
    {
        //new from copilot mxl
        //public static void ConvertToXml(string inputXmlPath, string outputMidiPath)
        //{

        //    var process = new Process();
        //    process.StartInfo.FileName = @"C:\Program Files\MuseScore 3\bin\MuseScore3.exe";
        //    process.StartInfo.Arguments = $"\"{inputXmlPath}\" -o \"{outputMidiPath}\"";
        //    process.StartInfo.UseShellExecute = false;
        //    process.Start();
        //    process.WaitForExit();
        //}

        //convert .mxl to .mid
        public static void ConvertToMidi(string inputXmlPath, string outputMidiPath)
        {
            var process = new Process();
            process.StartInfo.FileName = @"..\MuseScore 3\bin\MuseScore3.exe";
            process.StartInfo.Arguments = $"\"{inputXmlPath}\" -o \"{outputMidiPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }

        //convert mid file to music wav
        public static List<byte> ConvertMidiToWav(string midiPath, string wavPath)
        {
            // 1. קישור לקובץ ה-SoundFont בענן
            string soundFontUrl = "https://cyfuture.dl.sourceforge.net/project/androidframe/soundfonts/FluidR3_GM.sf2?viasf=1";
            string tempDir = Path.GetTempPath();
            string soundFontPath = Path.Combine(tempDir, "FluidR3_GM.sf2");
            // 2. הורדת קובץ ה-SoundFont אם הוא לא קיים עדיין
            if (!System.IO.File.Exists(soundFontPath))
            {
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(soundFontUrl, soundFontPath);
                }
            }


            var process = new Process();
            //string soundFont = @"../fluidsynth/sf2/FluidR3_GM.sf2";
            string fluidsynthExe = @"../fluidsynth/bin/fluidsynth.exe";

            process.StartInfo.FileName = fluidsynthExe;
            process.StartInfo.Arguments = $"-ni \"{soundFontPath}\" \"{midiPath}\" -F \"{wavPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine("שגיאה בהרצה:");
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("המרה הושלמה בהצלחה!");
                Console.WriteLine($"קובץ WAV נוצר ב: {wavPath}");
            }
            // החזר את הקובץ ל-client
            var fileBytes = System.IO.File.ReadAllBytes(wavPath);
            //return  System.IO.File(fileBytes, "audio/wav", Path.GetFileName(wavPath)); // החזר את הקובץ
            byte[] wavBytes = System.IO.File.ReadAllBytes(wavPath);

            // אופציונלי: מחיקת הקובץ הזמני
            //System.IO.File.Delete(wavPath);

            return wavBytes.ToList();

        }

        //convert image to mxl
        public static string ConvertImageToMusicXml(string inputImagePath, string outputXmlPath)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\Audiveris\audiveris.exe", // נתיב לכלי Audiveris
                    Arguments = $"-batch -export -output {outputXmlPath} -- {inputImagePath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.WriteLine($"OUTPUT: {args.Data}");
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.WriteLine($"ERROR: {args.Data}");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception("Audiveris process failed. Check the logs for details.");
            }

            Console.WriteLine($"Conversion completed: {outputXmlPath}");
            return outputXmlPath;
        }

        //public static void CreateXmlFile(string text, string outputPath)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    XmlElement root = xmlDoc.CreateElement("MusicalNotes");
        //    xmlDoc.AppendChild(root);

        //    XmlElement noteElement = xmlDoc.CreateElement("Note");
        //    noteElement.InnerText = text; // כאן תוכל לעבד את הטקסט כדי להוסיף תווים שונים
        //    root.AppendChild(noteElement);

        //    xmlDoc.Save(outputPath);
        //    Console.WriteLine("XML file created at: " + outputPath);
        //}
    }
}
