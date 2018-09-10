using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorialSearchModel
{
    class SearchWord
    {
        public string Word { get; set; } // Query word
        public List<int> DocumentIndex { get; set; } // document index in which a word accurs
        public List<double> DocumentTermFreq { get; set; } //Query word count in each document
        public int QueryTf { get; set; } //word appearance in query

        public double Wt { get; set; }   // query calculated weight
    }
}
