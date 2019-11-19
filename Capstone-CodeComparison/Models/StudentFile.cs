using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capstone_CodeComparison.Models
{
    public class StudentFile
    {
        protected String StudentName;
        protected String FileName;
        protected String FileContents;

        public StudentFile(String studentName, String fileName, String fileContents)
        {
            this.StudentName = studentName;
            this.FileName = fileName;
            this.FileContents = fileContents;
        }

        public String studentName
        {
            get
            {
                return StudentName;
            }
            set
            {
                StudentName = value;
            }
        }

        public String fileName
        {
            get
            {
                return FileName;
            }
            set
            {
                FileName = value;
            }
        }

        public String fileContents
        {
            get
            {
                return FileContents;
            }

            set
            {
                FileContents = value;
            }
        }

    }
}