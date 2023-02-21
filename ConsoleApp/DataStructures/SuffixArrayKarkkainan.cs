using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures
{
    internal class SuffixArrayKarkkainan
    {
        private readonly string S; //The original input text
        private readonly int n; // The length of the original input text
        private readonly int[] suftab; //The suffix array
        private readonly int[] lcptab; //The longest common prefix array
        private readonly int[] childtab; 
        private readonly int[] suflink;

      
        
        RangeMinimumQuery RangeMinimumQuery;

        public SuffixArrayKarkkainan(string text)
        {
            S = text;
            n = text.Length;
            suftab = new int[n + 3];
            ConstructESA();

            
           

            
        }

        public void ConstructESA()
        {
            int[] ints = convertStringToArrayInt(this.S, 0);
            int alphabetSize = max(ints, 0, n);
            var S = new int[n + 3];
            int[] end = new int[3] { 0, 0, 0 };
            Array.Copy(ints, 0, S, 0, n);
            Array.Copy(end, 0, S, n, 3);
            SuffixArray(S, suftab, n, alphabetSize, 0);
        }

        public void ConstructLCP()
        {
            for (int i = 1; i < suftab.Length; i++)
            {
                lcptab[i] = CalcLcp(suftab[i - 1], suftab[i]);
            }
            RangeMinimumQuery = new SparseTable(lcptab);
        }





        

        

        

        /// 
        /// <summary>Find the index of a substring </summary>
        /// <param name="substr">Substring to look for</param>
        /// <returns>First index in the original string. -1 if not found</returns>
        public int IndexOf(string substr)
        {
            int l = 0;
            int r = suftab.Length;
            int m = -1;

            if ((substr == null) || (substr.Length == 0))
            {
                return -1;
            }

            // Binary search for substring
            while (r > l)
            {
                m = (l + r) / 2;
                if (S.Substring(suftab[m]).CompareTo(substr) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    r = m;
                }
            }
            if ((l == r) && (l < S.Length) && (S.Substring(suftab[l]).StartsWith(substr)))
            {
                return suftab[l];
            }
            else
            {
                return -1;
            }
        }

        private HashSet<int> ReportAllOccurrences(string p)
        {
            
            HashSet<int> ints = new HashSet<int>();
            int index = IndexOf(p);
            if (index == -1) return ints;
            for (int u = index; u < S.Length; u++) 
            {
                if (S.Substring(suftab[u], S.Length - suftab[u]).StartsWith(p))
                {
                    ints.Add(suftab[u]);
                }
            }
            return ints;
        }

    

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = S.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (S[i + lcp] == S[j + lcp]); lcp++) ;
            return lcp;
        }

        public IEnumerable<(int,int)> ReportAllOccurrences(Query query)
        {
            var p1occs = ReportAllOccurrences(query.P1);
            var p2occs = ReportAllOccurrences(query.P2);
            var occs = new HashSet<(int,int)>();
            foreach (var item in p1occs)
            {
                if (p2occs.Contains(item + query.P1.Length + query.X))
                {
                    occs.Add((item, item + query.P1.Length + query.X + query.P2.Length));
                   
                }
                    
            }
            return occs;
        }




        //function is finding suffix array SA of s[0..n-1] in {1..K}^n
        //require s[n]=s[n+1]=s[n+2]=0, n>=2
        static public void SuffixArray(int[] s, int[] SA, int n, int K, int start)
        {

            //converting int n to double n
            double nDouble = n;

            //number of triplets on start position i = 0 (mod 3)
            int n0 = (int)Math.Ceiling(nDouble / 3);

            //number of triplets on start position i = 1 (mod 3)
            int n1 = (int)Math.Ceiling((nDouble - 1) / 3);

            //number of triplets on start position i = 2 (mod 3)
            int n2 = (int)Math.Ceiling((nDouble - 2) / 3);

            //number of triplets in T'
            int n02 = n0 + n2;

            //array for positions i != 0 (mod 3)
            int[] s12 = new int[n02 + 3];

            //put $ (0) on last three places
            s12[n02] = s12[n02 + 1] = s12[n02 + 2] = 0;

            //finding positions i != 0 (mod 3)
            if ((n0 - n1) == 0)
            {
                int position = 0;
                for (int i = 0; i < n; i++)
                {
                    if ((i % 3) != 0)
                    {
                        s12[position] = i;
                        position++;
                    }
                }
            }
            else if ((n0 - n1) == 1)            //if (n0-n1) == 1 -> add triplet $$$ on the end of t1
            {
                int position = 0;
                for (int i = 0; i < n + 1; i++)
                {
                    if ((i % 3) != 0)
                    {
                        s12[position] = i;
                        position++;
                    }
                }
            }

            //temporary array for radix sort
            int[] sorted = new int[n02 + 3];
            sorted[n02] = sorted[n02 + 1] = sorted[n02 + 2] = 0;

            //radix sort of triplets by last, second and first character
            sorted = radix(s12, s, 2, n02, K, start);
            sorted = radix(sorted, s, 1, n02, K, start);
            sorted = radix(sorted, s, 0, n02, K, start);

            //assign lexicographic names
            int name = 0;
            int c0 = -1;
            int c1 = -1;
            int c2 = -1;
            for (int i = 0; i < n02; i++)
            {
                if ((s[start + sorted[i]] != c0) || (s[start + sorted[i] + 1] != c1) || (s[start + sorted[i] + 2] != c2))
                {
                    name++;
                    c0 = s[start + sorted[i]];
                    c1 = s[start + sorted[i] + 1];
                    c2 = s[start + sorted[i] + 2];
                }
                if (sorted[i] % 3 == 1) //left part
                {
                    s12[sorted[i] / 3] = name;
                }
                else if (sorted[i] % 3 == 2) //right part	
                {
                    s12[sorted[i] / 3 + n0] = name;
                }
            }

            //array for suffix array
            int[] SA12 = new int[n02 + 3];
            SA12 = sorted;

            //in s12 now are triplet names

            //check if names are unique
            if (name < n02) //names are not unique
            {
                //recursive call
                SuffixArray(s12, SA12, n02, name, start);
                //put unique names in s12
                for (int i = 0; i < n02; i++)
                {
                    s12[SA12[i]] = i + 1;
                }
            }
            else //names are unique			
            {
                //make suffix array SA12
                for (int i = 0; i < n02; i++)
                {
                    SA12[s12[i] - 1] = i;
                }
            }

            int[] s0 = new int[n0];
            int[] SA0 = new int[n0];

            //calculating s0 from SA12 - radix sort the mod 0 suffixes from SA12 by their first character
            int l = 0;
            for (int i = 0; i < n02; i++)
            {
                if (SA12[i] < n0)
                {
                    s0[l++] = 3 * SA12[i];
                }
            }
            SA0 = radix(s0, s, 0, n0, K, start);

            //make SA from SA0 and SA12
            for (int p = 0, t = n0 - n1, k = 0; k < n; k++)
            {
                int i = (SA12[t] < n0 ? SA12[t] * 3 + 1 : (SA12[t] - n0) * 3 + 2);
                int j = SA0[p];

                // comparing suffixes
                if (SA12[t] < n0 ? leq(s[start + i], s12[SA12[t] + n0], s[start + j],
                    s12[j / 3]) : leq(s[start + i], s[start + i + 1], s12[SA12[t] - n0 + 1],
                    s[start + j], s[start + j + 1], s12[j / 3 + n0]))
                {
                    // suffix from SA12 is smaller
                    SA[k] = i;
                    t++;
                    if (t == n02)
                    {
                        for (k++; p < n0; p++, k++)
                        {
                            SA[k] = SA0[p];
                        }
                    }
                }
                else
                {
                    // suffix from SA12 is bigger
                    SA[k] = j;
                    p++;
                    if (p == n0)
                    {
                        for (k++; t < n02; t++, k++)
                        {
                            SA[k] = (SA12[t] < n0 ? SA12[t] * 3 + 1 : (SA12[t] - n0) * 3 + 2);
                        }
                    }
                }
            }
        }

        //radix sort a[0..n-1] with keys in 0..K from r
        static public int[] radix(int[] a, int[] r, int rIndex, int n, int K, int start)
        {
            //sorted array
            int[] sorted = new int[a.Length];

            //counter array
            int[] count = new int[K + 1];

            //count frequencies
            for (int i = 0; i < n; i++)
            {
                count[r[start + rIndex + a[i]]]++;
            }

            //exclusive prefix sums
            int sum = 0;
            for (int i = 0; i < (K + 1); i++)
            {
                int temp = count[i];
                count[i] = sum;
                sum += temp;
            }

            //sort
            for (int i = 0; i < n; i++)
            {
                sorted[count[r[start + rIndex + a[i]]]++] = a[i];
            }

            return sorted;
        }

        //lexic order for pairs
        static public bool leq(int a1, int a2, int b1, int b2)
        {
            return (a1 < b1 || (a1 == b1 && a2 <= b2));
        }
        //lexic order for triples
        static public bool leq(int a1, int a2, int a3, int b1, int b2, int b3)
        {
            return (a1 < b1 || (a1 == b1 && leq(a2, a3, b2, b3)));
        }
        //find max value in slice of an array
        static public int max(int[] input, int start, int length)
        {
            int max = input[start];
            for (int i = length - 2, index = start + 1; i >= 0; i--, index++)
            {
                int v = input[index];
                if (v > max)
                    max = v;

            }
            return max;
        }

        //function that converts string to integer array, keeping alphabetic order
        public static int[] convertStringToArrayInt(string s, int type)
        {

            int length = s.Length;

            int[] returnArray = new int[length + 1];

            if (type == 1) //if type is FASTA
            {
                //convert characters to upper
                s = s.ToUpper();

                // put $ char on the end of array
                returnArray[length] = 1;

                for (int i = 0; i < length; i++)
                {
                    //in FASTA only can be letters from A to Z, char '-' and '*'
                    if (!((s[i] >= 'A' && s[i] <= 'Z') || s[i] == '-' || s[i] == '*'))
                    {
                        throw new Exception("Wrong format!");
                    }

                    //convert chat to int
                    returnArray[i] = Convert.ToInt32(s[i]);
                }
            }
            else //type is plain S		
            {
                // put $ char on the end of array
                returnArray[length] = 1;

                for (int i = 0; i < length; i++)
                {
                    //convert chat to int
                    returnArray[i] = Convert.ToInt32(s[i]);
                }

            }
            return returnArray;
        }
        
        
    }

    internal abstract class RangeMinimumQuery
    {
        protected int[] arr;
        protected int N;

        protected RangeMinimumQuery(int[] arr)
        {
            this.arr = arr;
            this.N = arr.Length;
        }

        public abstract int RMQ(int startIndex, int endIndex);
    }

    internal class SparseTable : RangeMinimumQuery
    {
        private int[,] lookup;
        public SparseTable(int[] arr) : base(arr)
        {
            int logArr = (int)Math.Ceiling(Math.Log2(N));
            lookup = new int[logArr, logArr];
            // Initialize M for the intervals with length 1
            for (int i = 0; i < N; i++)
                lookup[i, 0] = arr[i];

            // Compute values from smaller to bigger intervals
            for (int j = 1; (1 << j) <= N; j++)
            {

                // Compute minimum value for all intervals with
                // size 2^j
                for (int i = 0; (i + (1 << j) - 1) < N; i++)
                {

                    // For arr[2][10], we compare arr[lookup[0][7]]
                    // and arr[lookup[3][10]]
                    if (lookup[i, j - 1] <
                                lookup[i + (1 << (j - 1)), j - 1])
                        lookup[i, j] = lookup[i, j - 1];
                    else
                        lookup[i, j] =
                                lookup[i + (1 << (j - 1)), j - 1];
                }
            }
        }

        public override int RMQ(int startIndex, int endIndex)
        {
            // Find highest power of 2 that is smaller
            // than or equal to count of elements in given
            // range. For [2, 10], j = 3
            int j = (int)Math.Log(endIndex - startIndex + 1);

            // Compute minimum of last 2^j elements with first
            // 2^j elements in range.
            // For [2, 10], we compare arr[lookup[0][3]] and
            // arr[lookup[3][3]],
            if (lookup[startIndex, j] <= lookup[endIndex - (1 << j) + 1, j])
                return lookup[startIndex, j];

            else
                return lookup[endIndex - (1 << j) + 1, j];
        }
    }
}
