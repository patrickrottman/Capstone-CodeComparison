using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.IO.Compression;
using Capstone_CodeComparison.Models;
using System.Text;

namespace Capstone_CodeComparison.Controllers
{
    public class FileProcessingController : Controller
    {
        // GET: FileProcessing

        //things to do
        //create output directory
        //finish unzipping code
        //paste remaining prewritten code
        //change to work with asp.net
        //take output and push to the view
        //create modal to show similarity
        //after everything working, add additional languages


        //List<String> PythonFilter = new List<String> { "using", @"/" };
        //List<String> 

        String LanguageFileType = null;
        List<String> StringsToFilterOut = new List<String>();

        String CFileType = "*.c";
        String CSharpFileType = "*.cs";
        String WPFFileType = "*.xaml.cs";
        String HTMLFileType = "*.html";
        String JavaFileType = "*.java";
        String CSSFileType = "*.css";
        String PythonFileType = "*.py";

        List<String> PythonFilter = new List<String> { "#", "import" };
        List<String> CSharpFilter = new List<String> { "///", "using", "//" };
        List<String> HTMLFilter = new List<String> { "//" };
        List<String> CFilter = new List<String> { "#include", "//", "/*", "*", "*/" };
        List<String> CSSFilter = new List<String> { "//", "/*", "*/" };
        List<String> JavaFilter = new List<String> { "//", "/*", "*/" };



        List<StudentFile> studentFileList = new List<StudentFile>();
        List<SimilarStudentData> SimilarStudentDataList = new List<SimilarStudentData>();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Start(String SelectedLanguage)
        {
            if (SelectedLanguage != null && SelectedLanguage != "" && SelectedLanguage != "Choose Language:")
            {
                Session["SelectedLanguage"] = SelectedLanguage;
                return Json(new { success = true, responseText = "Success." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Choose a language.", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public PartialViewResult Upload()
        {
            if (Session["SelectedLanguage"] != null)
            {
                String SelectedLanguage = Session["SelectedLanguage"].ToString();
                if (SelectedLanguage == "C#")
                {
                    LanguageFileType = CSharpFileType;
                    StringsToFilterOut = CSharpFilter;
                }
                if (SelectedLanguage == "HTML")
                {
                    LanguageFileType = HTMLFileType;
                    StringsToFilterOut = HTMLFilter;
                }
                if (SelectedLanguage == "C")
                {
                    LanguageFileType = CFileType;
                    StringsToFilterOut = CFilter;
                }
                if (SelectedLanguage == "Python")
                {
                    LanguageFileType = PythonFileType;
                    StringsToFilterOut = PythonFilter;
                }
                if (SelectedLanguage == "CSS")
                {
                    LanguageFileType = CSSFileType;
                    StringsToFilterOut = CSSFilter;
                }
                if (SelectedLanguage == "Java")
                {
                    LanguageFileType = JavaFileType;
                    StringsToFilterOut = JavaFilter;
                }
                if (SelectedLanguage == "WPF")
                {
                    LanguageFileType = WPFFileType;
                    StringsToFilterOut = CSharpFilter;
                }
            }
            else
            {
                //user must be downloading
                LanguageFileType = ".doesntExist";
            }

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        var fileName = Path.GetFileName(file);

                        byte[] data = new byte[stream.Length];
                        int br = stream.Read(data, 0, data.Length);


                        Session["FileContent"] = data;
                        Session["FileContentName"] = fileName;
                        String PersonalFolderPath = @"c:\" + Guid.NewGuid();
                        Session["PersonalFolderPath"] = PersonalFolderPath;
                        Directory.CreateDirectory(PersonalFolderPath);
                        Session["OuputFolderPath"] = PersonalFolderPath + @"\" + "output";
                        Directory.CreateDirectory(Session["OuputFolderPath"].ToString());
                        Session["ZipFileLocation"] = PersonalFolderPath + @"\" + fileName;
                        stream.Position = 0;
                        using (var fileStream = System.IO.File.Create(PersonalFolderPath + @"\" + fileName))
                        {
                            stream.CopyTo(fileStream);
                        }

                        //https://stackoverflow.com/questions/31914568/save-an-attachment-in-session

                        startUnzip_Click();
                        StartCheckingStudents();
                        return PartialView("_StudentList", SimilarStudentDataList.OrderByDescending(x => x.SimilarityPercentage).Take(25).ToList());
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

            return null;

        }

        [HttpPost]
        public void DeleteSessionFolder()
        {

            if (Session["PersonalFolderPath"] != null)
            {
                String path = Session["PersonalFolderPath"].ToString();
                if (Directory.Exists(path))
                {

                    Directory.Delete(path, true);
                    Session["FileContent"] = null;
                    Session["FileContentName"] = null;
                    Session["PersonalFolderPath"] = null;
                    Session["OuputFolderPath"] = null;
                    Session["ZipFileLocation"] = null;
                }
            }
        }

        public FileResult Download()
        {
            if (Session["OuputFolderPath"] != null)
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(Session["OuputFolderPath"] as String, (Session["PersonalFolderPath"] as String) + @"\" + "output.zip", System.IO.Compression.CompressionLevel.Optimal, true, new MyEncoder());


                byte[] fileBytes = System.IO.File.ReadAllBytes((Session["PersonalFolderPath"] as String) + @"\" + "output.zip");
                string fileName = "output.zip";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return null;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        public void startUnzip_Click()
        {
            String ExportFolderLocation = Session["OuputFolderPath"] as String;
            String ZipFilesLocation = Session["ZipFileLocation"] as String;

            //Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // set the cursor to loading spinner
            if ((ZipFilesLocation != null && ZipFilesLocation != "") || ExportFolderLocation != null && ExportFolderLocation != "")
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(ZipFilesLocation, ExportFolderLocation);
                DirectoryInfo d = new DirectoryInfo(ExportFolderLocation);
                FileInfo[] Files = d.GetFiles("*.zip");
                String customFolder;
                foreach (FileInfo file in Files)
                {
                    try
                    {
                        customFolder = ExportFolderLocation + @"\" + file.Name.Substring(0, file.Name.Length - 4);
                        Directory.CreateDirectory(customFolder);
                        ZipFile.ExtractToDirectory(file.FullName, customFolder);
                        System.IO.File.Delete(file.FullName);

                    }
                    catch (Exception error)
                    {
                        //System.Windows.MessageBox.Show("Unable to extract " + file.Name + " due to: " + error.Message);
                    }
                }
            }
            //else
            {
                //System.Windows.MessageBox.Show("Please choose file directory first.");
                //Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // set the cursor back to arrow
                //return;
            }
            //Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // set the cursor back to arrow
        }

        public void StartCheckingStudents()
        {
            WalkDirectoryTree(new DirectoryInfo(Session["OuputFolderPath"] as String));
            List<StudentFile> outerList = studentFileList;
            List<StudentFile> innerList = studentFileList;

            foreach (StudentFile student in outerList.ToList())
            {
                innerList = studentFileList;
                foreach (StudentFile innerStudent in innerList.ToList())
                {
                    if (student.studentName != innerStudent.studentName)
                    {
                        if (student.fileContents != "" && innerStudent.fileContents != "")
                        {
                            Tuple<double, List<String>> similarity = SimilarityinPercentage(student.fileContents, innerStudent.fileContents);
                            if (similarity.Item1 > 0)
                            {
                                SimilarStudentDataList.Add(new SimilarStudentData(student.studentName, innerStudent.studentName, student.fileContents, innerStudent.fileContents, student.fileName, innerStudent.fileName, Math.Round(similarity.Item1, 3, MidpointRounding.ToEven), similarity.Item2));// "<tr><td>" + student.fileName + "</td><td>" + innerStudent.fileName + "</td><td>" + similarity.ToString("0.00") + "%</td></tr>");
                            }
                        }
                    }
                }
                outerList.Remove(student);
            }
        }

        private void WalkDirectoryTree(DirectoryInfo dr)
        {
            String studentName;
            foreach (FileInfo file in FindFiles(dr, LanguageFileType))
            {
                // process file
                if (file.Name != "App.xaml.cs" && file.Name[0] != '.')
                {
                    String outputLocation = Session["OuputFolderPath"] as String;
                    studentName = file.FullName.Replace(outputLocation, "");
                    studentName = studentName.Substring(1, studentName.Length - 1);
                    if (studentName.IndexOf(@"\") == -1) //this means the person hasnt zipped their file. we can then use the begining of the file type to find the end of their name
                    {
                        studentName = studentName.Substring(0, studentName.IndexOf("."));
                    }
                    else
                    {
                        studentName = studentName.Substring(0, studentName.IndexOf(@"\"));
                    }
                    studentName = studentName.Substring(0, studentName.IndexOf("_"));
                    //file.Name.Substring(0, file.Name.Length - 4);


                    StudentFile tempStudent = new StudentFile(studentName, file.Name, System.IO.File.ReadAllText(file.FullName));
                    studentFileList.Add(tempStudent);

                    //bool checking = true;

                    //if (!FileNames.HasItems)
                    //{
                    //    FileNames.Items.Add(tempStudent.fileName);
                    //}
                    //else
                    //{
                    //    foreach (var name in FileNames.Items)
                    //    {
                    //        if (name.ToString().Equals(tempStudent.fileName))
                    //        {
                    //            checking = false;
                    //        }
                    //    }
                    //    if (checking == true)
                    //    {
                    //        FileNames.Items.Add(tempStudent.fileName);
                    //    }
                    //}
                }
            }
        }

        public IEnumerable<FileInfo> FindFiles(DirectoryInfo startDirectory, string pattern)
        {
            IEnumerable<FileInfo> files = startDirectory.EnumerateFiles(pattern, SearchOption.AllDirectories);
            return files;
        }

        public Tuple<double, List<String>> SimilarityinPercentage(String original, String comparison)
        {
            original = original.Replace("\r", "");
            comparison = comparison.Replace("\r", "");

            //original = original.Replace("\"", "");
            //comparison = comparison.Replace("\"", "");

            original = original.Replace("{", "");
            comparison = comparison.Replace("{", "");

            original = original.Replace("}", "");
            comparison = comparison.Replace("}", "");



            IQueryable<String> splitString1 = original.Split('\n').AsQueryable();
            IQueryable<String> splitString2 = comparison.Split('\n').AsQueryable();

            splitString1 = splitString1.Where(s => !string.IsNullOrWhiteSpace(s)).AsQueryable();
            splitString2 = splitString2.Where(s => !string.IsNullOrWhiteSpace(s)).AsQueryable();

            foreach (String filter in StringsToFilterOut)
            {
                splitString1 = splitString1.Where(x => !x.Contains(filter)).AsQueryable();
                splitString2 = splitString2.Where(x => !x.Contains(filter)).AsQueryable();
            }



            // code from http://www.dotnetworld.in/2013/05/c-find-similarity-between-two-strings.html
            List<String> strCommon = splitString1.Intersect(splitString2).ToList();
            //Formula : Similarity (%) = 100 * (CommonItems * 2) / (Length of String1 + Length of String2)
            double Similarity = (double)(100 * (strCommon.Count() * 2)) / (splitString1.Count() + splitString2.Count());
            Console.WriteLine("Strings are {0}% similar", Similarity.ToString("0.00"));

            //ulong oldCount = ulong.Parse(counter.Text) + ulong.Parse(splitString1.Count().ToString()) * ulong.Parse(splitString2.Count().ToString());

            //counter.Text = oldCount.ToString();

            return Tuple.Create(Similarity, strCommon);
        }
    }
    class MyEncoder : UTF8Encoding
    {
        public MyEncoder() : base(true)
        {

        }
        public override byte[] GetBytes(string s)
        {
            s = s.Replace("\\", "/");
            return base.GetBytes(s);
        }
    }
}