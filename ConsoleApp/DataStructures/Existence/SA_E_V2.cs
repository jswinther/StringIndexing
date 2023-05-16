using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.DataStructures.SuffixArrayFinal;

namespace ConsoleApp.DataStructures.Existence
{
    public class SA_E_V2
    {
        private SuffixArrayFinal SA;
        private Dictionary<(int, int), IntervalNode> Tree;
        private Dictionary<(int, int), IntervalNode> Leaves;
        private IntervalNode Root;
        //private Dictionary<(int, int), IntervalNode> TreeSqrt;
        //private HashSet<(int, int)> LeavesSqrt;
        private List<IntervalNode> BotLevel;
        private int FixedGap { get; set; }
        private int MinGap { get; set; }
        private int MaxGap { get; set; }


        private Dictionary<(int, int), HashSet<(int, int)>> ExistsForward = new Dictionary<(int, int), HashSet<(int, int)>>();
        private Dictionary<(int, int), HashSet<(int, int)>> ExistsBackward = new Dictionary<(int, int), HashSet<(int, int)>>();
        //private Dictionary<((int, int), (int, int)), bool> Exists = new Dictionary<((int, int), (int, int)), bool>();


        public SA_E_V2(string str, int fixedGap, int minGap, int maxGap)
        {
            SA = new SuffixArrayFinal(str);
            FixedGap = fixedGap;
            MinGap = minGap;
            MaxGap = maxGap;
            SA.BuildChildTable();
            int minSizeForLcpIntervals = 0;
            int minSizeSaved = (int)Math.Sqrt(SA.n);
            SA.GetAllLcpIntervals(minSizeForLcpIntervals, out Tree, out Leaves, out Root);
            //SA.GetAllLcpIntervals((int)Math.Sqrt(SA.n), out TreeSqrt, out LeavesSqrt);
            ComputeSubSuffixArrays(minSize: minSizeSaved);


            /*Precomputation 1
            foreach (var int1 in Tree.Keys.Where(s => s.Item2 - s.Item1 > Math.Sqrt(SA.n)))
            {
                //var occs2 = new HashSet<int>(SA.GetOccurrencesForInterval(int2));
                var occs1 = SA.GetOccurrencesForInterval(int1);

                foreach (var int2 in Tree.Keys)
                {
                    var occs2 = SA.GetOccurrencesForInterval(int2);
                    if (occs1.Any(occ1 => occs2.Contains(occ1 + Tree[int1].DistanceToRoot + FixedGap)))
                    {
                        ExistsForward[int1].Add(int2);
                    }
                    if (occs1.Any(occ1 => occs2.Contains(occ1 - Tree[int1].DistanceToRoot - FixedGap)))
                    {
                        ExistsBackward[int1].Add(int2);
                    }
                    //Exists.Add((int1, int2), occs1.Any(occ1 => occs2.Contains(occ1 + FixedGap)));
                }
            }*/

            /*Precomputation 2 */
            BotLevel = new List<IntervalNode>();
            findBotLevelRec(Tree.Values.First(), minSizeSaved);

            //var botLevel = Tree.Values.Where(s => s.Size >= minSizeSaved && s.Children.All(e => e.Size < minSizeSaved));

            foreach (var interval1 in BotLevel)
            {
                var occs1 = new HashSet<int>(SA.GetOccurrencesForInterval(interval1.Interval));

                foreach (var interval2 in BotLevel)
                {
                    var occs2 = SA.GetOccurrencesForInterval(interval2.Interval);
                    if (occs2.Any(occ2 => occs1.Contains(occ2 - Tree[interval1.Interval].DistanceToRoot - FixedGap)))
                    {
                        if (ExistsForward.ContainsKey(interval1.Interval))
                        {
                            ExistsForward[interval1.Interval].Add(interval2.Interval);
                        }
                        else
                        {
                            ExistsForward.Add(interval1.Interval, new HashSet<(int, int)>(new (int, int)[] { interval2.Interval }));
                        }
                        
                    }
                    if (occs2.Any(occ2 => occs1.Contains(occ2 + Tree[interval2.Interval].DistanceToRoot + FixedGap)))
                    {
                        if (ExistsBackward.ContainsKey(interval1.Interval))
                        {
                            ExistsBackward[interval1.Interval].Add(interval2.Interval);
                        }
                        else
                        {
                            ExistsBackward.Add(interval1.Interval, new HashSet<(int, int)>(new (int, int)[] { interval2.Interval }));
                        }
                        
                    }
                }
                interval1.Merged = true;
            }
            RecurseMerge(Tree.Values.First(), minSizeSaved);

        }

        private void findBotLevelRec(IntervalNode node, int minSize)
        {
            //!SA.S.Substring(SA[child.Interval.start],1).Equals("|")
            if (node.Children.Any(child => child.Size < minSize && child.Size != 1))
            {
                BotLevel.Add(node);
            } else
            {
                foreach (var child in node.Children)
                {
                    findBotLevelRec(child, minSize);
                }
            }
        }

        private void RecurseMerge(IntervalNode root, int minSize)
        {
            if (!root.Merged)
            {
                foreach (var child in root.Children)
                {
                    if (child.Size < minSize)
                    {
                        continue;
                    }
                    RecurseMerge(child, minSize);
                    HashSet < (int, int) > rootExists = new();
                    if (ExistsForward.TryGetValue(root.Interval, out rootExists))
                    {
                        rootExists.UnionWith(ExistsForward[child.Interval]);
                        //ExistsForward[root.Interval].UnionWith(childExists);
                    } else
                    {
                        ExistsForward.Add(root.Interval, new HashSet<(int, int)> (ExistsForward[child.Interval]));
                    }

                    if (ExistsBackward.TryGetValue(root.Interval, out rootExists))
                    {
                        rootExists.UnionWith(ExistsBackward[child.Interval]);
                        //ExistsBackward[root.Interval].UnionWith(ExistsBackward[child.Interval]);
                    }
                    else
                    {
                        ExistsBackward.Add(root.Interval, new HashSet<(int, int)>(ExistsBackward[child.Interval]));
                    }

                }
                root.Merged = true;
            } else
            {
                return;
            }
        }

        private void ComputeSubSuffixArrays(int minSize)
        {
            foreach (var interval in Tree.Values)
            {

            }
        }

        public bool FixedExists(string pattern1, string pattern2)
        {
            var int1 = SA.ExactStringMatchingWithESA(pattern1);
            var int2 = SA.ExactStringMatchingWithESA(pattern2);
            if (ExistsForward[int1].Contains(int2) || ExistsBackward[int2].Contains(int1))
            {
                return true;
            } else if (int1.j - int1.i > int2.j - int2.i)
            {
                var occs1 = new HashSet<int>(SA.GetOccurrencesForInterval(int1));
                var occs2 = SA.GetOccurrencesForInterval(int2);
                return occs2.Any(occ2 => occs1.Contains(occ2 - Tree[int1].DistanceToRoot - FixedGap));
            } else
            {
                var occs1 = SA.GetOccurrencesForInterval(int1);
                var occs2 = new HashSet<int>(SA.GetOccurrencesForInterval(int2));
                return occs1.Any(occ1 => occs2.Contains(occ1 + Tree[int1].DistanceToRoot + FixedGap));
            }
        }

        public bool VariableExists(string pattern1, string pattern2)
        {
            return true;
        }
    }
}
