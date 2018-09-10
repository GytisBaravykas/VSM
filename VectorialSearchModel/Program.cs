using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace VectorialSearchModel
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> documentList = new List<string> { @"txt_formatu\antanas-kraujelis.txt", @"txt_formatu\cekistine-kariuomene.txt", @"txt_formatu\Dainavos-partizanai-saruno-rinktine.txt", @"txt_formatu\dejavo-zeme.txt", @"txt_formatu\didziosios_kovos.txt", @"txt_formatu\drasiai-stovesim.txt", @"txt_formatu\garmute-isejo-broliai.txt", @"txt_formatu\giedraiciu-partizanai.txt", @"txt_formatu\kestucio-partizanai.txt", @"txt_formatu\kovoje-del-laisves.txt", @"txt_formatu\laisves-kaina.txt", @"txt_formatu\lietuvos_karas.txt", @"txt_formatu\lietuvos_partizanai.txt", @"txt_formatu\motinele-auginai.txt", @"txt_formatu\pasipriesinimo_istorija.txt", @"txt_formatu\pokario-lietuvos-laisves-kovotojai.txt", @"txt_formatu\ramanauskas-vanagas.txt", @"txt_formatu\tigro-rinktine.txt", @"txt_formatu\Už laisvę ir tėvynę.txt", @"txt_formatu\zalio-velnio-takais.txt", @"txt_formatu\partizano-kelias.txt"};

            List<SearchWord> SearchWords = new List<SearchWord> { };
            List<string> readDodumencts = Read(documentList);

            //Lithuania encoding
            Console.InputEncoding = System.Text.Encoding.GetEncoding(1257);
            Console.OutputEncoding = System.Text.Encoding.GetEncoding(1257);

            // bigrams
            List<string> bigrams = new List<string> { "partizanų būrys", "Lietuvos teritorija", "partizanų vadas", "laisva Lietuva", "raudonoji armija" };

            Console.WriteLine("Paieskos programa, norint iseiti iveskite('Viso'):");
            while (Console.ReadLine() != "Viso")
            {

                Console.Write("Search: ");
                string input = Console.ReadLine();
                while (input == "")
                {
                    input = Console.ReadLine();
                }

                input = Regex.Replace(input, @"[^A-zA-Ž\s]", "");

                List<string> query = input.Split(new char[] { ' ', '\t', '\n' }).ToList();
                List<string> nQuery = new List<string>(query);

                // Query changed if bigrams exist
                for (int x = 0; x < query.Count - 1; x++)
                {
                    string word = query[x] + " " + query[x + 1];
                    for (int k = 0; k < bigrams.Count; k++)
                    {
                        if (word == bigrams[k])
                        {
                            nQuery.Remove(nQuery[x + 1]);
                            nQuery[x] = bigrams[k];

                        }
                    }
                }
                //query = nQuery;
                int sk = 0;
                while (sk < 2)
                {
                    string name = "Be bigramų";
                    if (sk == 1)
                    {
                        name = "Su bigramomis";
                        query = nQuery;
                    }
                    sk++;
                    query = query.Where(x => x.Length > 2).ToList();

                    // Each word in query processed to fill SearchWord class attributes
                    for (int x = 0; x < query.Count; x++)
                    {
                        var word = new SearchWord();
                        var list = new List<int> { };
                        var wordCount = new List<double> { };

                        for (int y = 0; y < readDodumencts.Count; y++)
                        {
                            if (readDodumencts[y].Contains(query[x]))
                            {
                                double numberOfTimes = Regex.Matches(readDodumencts[y], query[x]).Count;
                                numberOfTimes = 1 + Math.Log10(numberOfTimes);
                                list.Add(y);
                                wordCount.Add(numberOfTimes);
                            }

                        }

                        int numWd = 0;
                        foreach(var wd in query)
                        {
                            if (query[x] == wd)
                            {
                                numWd++;
                            }
                        }

                        word.Word = query[x];
                        word.DocumentIndex = list;
                        word.DocumentTermFreq = wordCount;
                        word.QueryTf = numWd;
                        
                        word.Wt = (1 + Math.Log10(numWd)) * Math.Log10((double)readDodumencts.Count / list.Count);
                        SearchWords.Add(word);

                    }

                    SearchWords.Sort((x, y) => x.DocumentIndex.Count.CompareTo(y.DocumentIndex.Count));

                    //Document lenght is counted for each document
                    var documentsLenght = new List<double> {};
                    var leghtIndex = new List<int> { };

                    for (int num = 0; num < readDodumencts.Count; num++)
                    {
                        var temp = new List<int> {};
                        double sum = 0;
                        foreach (var sw in SearchWords)
                        {
                            for(int ind=0;ind<sw.DocumentIndex.Count; ind++)
                            {
                                if (sw.DocumentIndex[ind] == num && !temp.Contains(num))
                                {
                                    temp.Add(num);
                                    leghtIndex.Add(num);
                                    sum += sw.DocumentTermFreq[ind] * sw.DocumentTermFreq[ind];
                                    
                                }
                            }
                        }
                        sum = Math.Sqrt(sum);  
                        //Individual document lenght is added to list of all document lenghts.
                        documentsLenght.Add(sum);
                    }
                    


                    //Similarity is calculated
                    var similarity = new List<double> { };
                    for (int num = 0; num < leghtIndex.Count; num++)
                    {
                        var temp = new List<double> { };
                        double sim = 0;
                        foreach (var sw in SearchWords)
                        {
                            for (int ind = 0; ind < sw.DocumentIndex.Count; ind++)
                            {
                                if (sw.DocumentIndex[ind] == leghtIndex[num] && !temp.Contains(leghtIndex[num]))
                                {
                                    temp.Add(num);
                                    if (sw.DocumentTermFreq[ind] == 0 || documentsLenght[num]==0)
                                    {
                                        sim = 0;
                                    }
                                    else
                                    {
                                        sim += sw.Wt * (double)sw.DocumentTermFreq[ind] / documentsLenght[num];
                                    }
                                    

                                }
                            }
                        }
                        similarity.Add(sim);
                    }

                    Console.WriteLine("Dokumentai: " + name);

                    // Two arrays sorted by similarity value
                    leghtIndex.Sort();
                    var similarit = similarity.ToArray();
                    var leghtInd = leghtIndex.ToArray();
                    Array.Sort(similarit, leghtInd);
                    Array.Reverse(similarit);
                    Array.Reverse(leghtInd);
                    
                    // Printing answers
                    for(int k=0;k<similarit.Length;k++)
                    {
                        if (k < 8)
                        {
                            Console.WriteLine(documentList[leghtInd[k]] + " " + similarit[k]);
                        }
                    }

                    SearchWords.Clear();
                    Console.WriteLine();

                }

            }

            Console.ReadLine();
        }
        static List<string> Read(List<string> documentList)
        {
            var readDocuments = new List<string>();
            foreach (var document in documentList)
            {
                string line;
                string text = "";
                using (StreamReader reader = new StreamReader(document, Encoding.UTF8))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        text += line;
                    }
                    readDocuments.Add(text);
                }
            }
            return readDocuments;
        }
    }
}
