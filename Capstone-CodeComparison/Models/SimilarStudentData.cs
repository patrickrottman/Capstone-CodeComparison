using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_CodeComparison.Models
{
    public class SimilarStudentData
    {
        public String FirstPawprint;
        public String SecondPawprint;

        public String FirstPawprintFileName;
        public String SecondPawprintFileName;

        public String FirstPawprintFileContents;
        public String SecondPawprintFileContents;

        public List<String> SimilarFileContents = new List<String>();
        public double SimilarityPercentage;

        public SimilarStudentData(String firstPawprint, String secondPawprint, String firstPawprintFileContents, String secondPawprintFileContents, String firstPawprintFileName, String secondPawprintFileName, double similarityPercentage, List<String> similarFileContents)
        {
            this.FirstPawprint = firstPawprint;
            this.FirstPawprintFileName = firstPawprintFileName;
            this.FirstPawprintFileContents = firstPawprintFileContents;

            this.SecondPawprint = secondPawprint;
            this.SecondPawprintFileName = secondPawprintFileName;
            this.SecondPawprintFileContents = secondPawprintFileContents;

            this.SimilarFileContents = similarFileContents;
            this.SimilarityPercentage = similarityPercentage;
        }
    }
}