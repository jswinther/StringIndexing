using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp.DataStructures
{
    public class SuffixTree : PatternMatcher
    {
        public char? CanonizationChar { get; set; }
        public string Word { get; private set; }
        private int CurrentSuffixStartIndex { get; set; }
        private int CurrentSuffixEndIndex { get; set; }
        private Node LastCreatedNodeInCurrentIteration { get; set; }
        private int UnresolvedSuffixes { get; set; }
        public Node RootNode { get; private set; }
        private Node ActiveNode { get; set; }
        private Edge ActiveEdge { get; set; }
        private int DistanceIntoActiveEdge { get; set; }
        private char LastCharacterOfCurrentSuffix { get; set; }
        private int NextNodeNumber { get; set; }
        private int NextEdgeNumber { get; set; }

        public SuffixTree(string word) : base(word)
        {
            Word = word;
            RootNode = new Node(this);
            ActiveNode = RootNode;
        }

        public event Action<SuffixTree> Changed;
        private void TriggerChanged()
        {
            var handler = Changed;
            if (handler != null)
                handler(this);
        }

        public event Action<string, object[]> Message;
        private void SendMessage(string format, params object[] args)
        {
            var handler = Message;
            if (handler != null)
                handler(format, args);
        }

        public static SuffixTree Create(string word, char canonizationChar = '$')
        {
            var tree = new SuffixTree(word);
            tree.Build(canonizationChar);
            return tree;
        }

        public void Build(char canonizationChar)
        {
            var n = Word.IndexOf(Word[Word.Length - 1]);
            var mustCanonize = n < Word.Length - 1;
            if (mustCanonize)
            {
                CanonizationChar = canonizationChar;
                Word = string.Concat(Word, canonizationChar);
            }

            for (CurrentSuffixEndIndex = 0; CurrentSuffixEndIndex < Word.Length; CurrentSuffixEndIndex++)
            {
                SendMessage("=== ITERATION {0} ===", CurrentSuffixEndIndex);
                LastCreatedNodeInCurrentIteration = null;
                LastCharacterOfCurrentSuffix = Word[CurrentSuffixEndIndex];

                for (CurrentSuffixStartIndex = CurrentSuffixEndIndex - UnresolvedSuffixes; CurrentSuffixStartIndex <= CurrentSuffixEndIndex; CurrentSuffixStartIndex++)
                {
                    var wasImplicitlyAdded = !AddNextSuffix();
                    if (wasImplicitlyAdded)
                    {
                        UnresolvedSuffixes++;
                        break;
                    }
                    if (UnresolvedSuffixes > 0)
                        UnresolvedSuffixes--;
                }
            }
        }

        private bool AddNextSuffix()
        {
            var suffix = string.Concat(Word.Substring(CurrentSuffixStartIndex, CurrentSuffixEndIndex - CurrentSuffixStartIndex), "{", Word[CurrentSuffixEndIndex], "}");
            SendMessage("The next suffix of '{0}' to add is '{1}' at indices {2},{3}", Word, suffix, CurrentSuffixStartIndex, CurrentSuffixEndIndex);
            SendMessage(" => ActiveNode:             {0}", ActiveNode);
            SendMessage(" => ActiveEdge:             {0}", ActiveEdge == null ? "none" : ActiveEdge.ToString());
            SendMessage(" => DistanceIntoActiveEdge: {0}", DistanceIntoActiveEdge);
            SendMessage(" => UnresolvedSuffixes:     {0}", UnresolvedSuffixes);
            if (ActiveEdge != null && DistanceIntoActiveEdge >= ActiveEdge.Length)
                throw new Exception("BOUNDARY EXCEEDED");

            if (ActiveEdge != null)
                return AddCurrentSuffixToActiveEdge();

            if (GetExistingEdgeAndSetAsActive())
                return false;

            ActiveNode.AddNewEdge();
            TriggerChanged();

            UpdateActivePointAfterAddingNewEdge();
            return true;
        }

        private bool GetExistingEdgeAndSetAsActive()
        {
            Edge edge;
            if (ActiveNode.Edges.TryGetValue(LastCharacterOfCurrentSuffix, out edge))
            {
                SendMessage("Existing edge for {0} starting with '{1}' found. Values adjusted to:", ActiveNode, LastCharacterOfCurrentSuffix);
                ActiveEdge = edge;
                DistanceIntoActiveEdge = 1;
                TriggerChanged();

                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(ActiveEdge.StartIndex);
                SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
                SendMessage(" => DistanceIntoActiveEdge is now: {0}", DistanceIntoActiveEdge);
                SendMessage(" => UnresolvedSuffixes is now: {0}", UnresolvedSuffixes);

                return true;
            }
            SendMessage("Existing edge for {0} starting with '{1}' not found", ActiveNode, LastCharacterOfCurrentSuffix);
            return false;
        }

        private bool AddCurrentSuffixToActiveEdge()
        {
            var nextCharacterOnEdge = Word[ActiveEdge.StartIndex + DistanceIntoActiveEdge];
            if (nextCharacterOnEdge == LastCharacterOfCurrentSuffix)
            {
                SendMessage("The next character on the current edge is '{0}' (suffix added implicitly)", LastCharacterOfCurrentSuffix);
                DistanceIntoActiveEdge++;
                TriggerChanged();

                SendMessage(" => DistanceIntoActiveEdge is now: {0}", DistanceIntoActiveEdge);
                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(ActiveEdge.StartIndex);

                return false;
            }

            SplitActiveEdge();
            ActiveEdge.Tail.AddNewEdge();
            TriggerChanged();

            UpdateActivePointAfterAddingNewEdge();

            return true;
        }

        private void UpdateActivePointAfterAddingNewEdge()
        {
            if (ReferenceEquals(ActiveNode, RootNode))
            {
                if (DistanceIntoActiveEdge > 0)
                {
                    SendMessage("New edge has been added and the active node is root. The active edge will now be updated.");
                    DistanceIntoActiveEdge--;
                    SendMessage(" => DistanceIntoActiveEdge decremented to: {0}", DistanceIntoActiveEdge);
                    ActiveEdge = DistanceIntoActiveEdge == 0 ? null : ActiveNode.Edges[Word[CurrentSuffixStartIndex + 1]];
                    SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
                    TriggerChanged();

                    NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(CurrentSuffixStartIndex + 1);
                }
            }
            else
                UpdateActivePointToLinkedNodeOrRoot();
        }

        private void NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(int firstIndexOfOriginalActiveEdge)
        {
            var walkDistance = 0;
            while (ActiveEdge != null && DistanceIntoActiveEdge >= ActiveEdge.Length)
            {
                SendMessage("Active point is at or beyond edge boundary and will be moved until it falls inside an edge boundary");
                DistanceIntoActiveEdge -= ActiveEdge.Length;
                ActiveNode = ActiveEdge.Tail ?? RootNode;
                if (DistanceIntoActiveEdge == 0)
                    ActiveEdge = null;
                else
                {
                    walkDistance += ActiveEdge.Length;
                    var c = Word[firstIndexOfOriginalActiveEdge + walkDistance];
                    ActiveEdge = ActiveNode.Edges[c];
                }
                TriggerChanged();
            }
        }

        private void SplitActiveEdge()
        {
            ActiveEdge = ActiveEdge.SplitAtIndex(ActiveEdge.StartIndex + DistanceIntoActiveEdge);
            SendMessage(" => ActiveEdge is now: {0}", ActiveEdge);
            TriggerChanged();
            if (LastCreatedNodeInCurrentIteration != null)
            {
                LastCreatedNodeInCurrentIteration.LinkedNode = ActiveEdge.Tail;
                SendMessage(" => Connected {0} to {1}", LastCreatedNodeInCurrentIteration, ActiveEdge.Tail);
                TriggerChanged();
            }
            LastCreatedNodeInCurrentIteration = ActiveEdge.Tail;
        }

        private void UpdateActivePointToLinkedNodeOrRoot()
        {
            SendMessage("The linked node for active node {0} is {1}", ActiveNode, ActiveNode.LinkedNode == null ? "[null]" : ActiveNode.LinkedNode.ToString());
            if (ActiveNode.LinkedNode != null)
            {
                ActiveNode = ActiveNode.LinkedNode;
                SendMessage(" => ActiveNode is now: {0}", ActiveNode);
            }
            else
            {
                ActiveNode = RootNode;
                SendMessage(" => ActiveNode is now ROOT", ActiveNode);
            }
            TriggerChanged();

            if (ActiveEdge != null)
            {
                var firstIndexOfOriginalActiveEdge = ActiveEdge.StartIndex;
                ActiveEdge = ActiveNode.Edges[Word[ActiveEdge.StartIndex]];
                TriggerChanged();
                NormalizeActivePointIfNowAtOrBeyondEdgeBoundary(firstIndexOfOriginalActiveEdge);
            }
        }

        public string RenderTree()
        {
            var writer = new StringWriter();
            RootNode.RenderTree(writer, "");
            return writer.ToString();
        }

        public string WriteDotGraph()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph {");
            sb.AppendLine("rankdir = LR;");
            sb.AppendLine("edge [arrowsize=0.5,fontsize=11];");
            for (var i = 0; i < NextNodeNumber; i++)
                sb.AppendFormat("node{0} [label=\"{0}\",style=filled,fillcolor={1},shape=circle,width=.1,height=.1,fontsize=11,margin=0.01];",
                    i, ActiveNode.NodeNumber == i ? "cyan" : "lightgrey").AppendLine();
            RootNode.WriteDotGraph(sb);
            sb.AppendLine("}");
            return sb.ToString();
        }

        public HashSet<string> ExtractAllSubstrings()
        {
            var set = new HashSet<string>();
            ExtractAllSubstrings("", set, RootNode);
            return set;
        }

        private void ExtractAllSubstrings(string str, HashSet<string> set, Node node)
        {
            foreach (var edge in node.Edges.Values)
            {
                var edgeStr = edge.StringWithoutCanonizationChar;
                var edgeLength = !edge.EndIndex.HasValue && CanonizationChar.HasValue ? edge.Length - 1 : edge.Length; // assume tailing canonization char
                for (var length = 1; length <= edgeLength; length++)
                    set.Add(string.Concat(str, edgeStr.Substring(0, length)));
                if (edge.Tail != null)
                    ExtractAllSubstrings(string.Concat(str, edge.StringWithoutCanonizationChar), set, edge.Tail);
            }
        }

        public List<string> ExtractSubstringsForIndexing(int? maxLength = null)
        {
            var list = new List<string>();
            ExtractSubstringsForIndexing("", list, maxLength ?? Word.Length, RootNode);
            return list;
        }

        private void ExtractSubstringsForIndexing(string str, List<string> list, int len, Node node)
        {
            foreach (var edge in node.Edges.Values)
            {
                var newstr = string.Concat(str, Word.Substring(edge.StartIndex, Math.Min(len, edge.Length)));
                if (len > edge.Length && edge.Tail != null)
                    ExtractSubstringsForIndexing(newstr, list, len - edge.Length, edge.Tail);
                else
                    list.Add(newstr);
            }
        }

        public override IEnumerable<int> Matches(string pattern)
        {
            Node currNode = RootNode;
            int matchingIndex = 0;
            if (currNode.Edges.TryGetValue(pattern[0], out Edge edge))
            {
                int min = Math.Min(pattern.Length, edge.Length);
                for (int i = 0; i < min; i++)
                {
                    if (pattern[matchingIndex + i] != edge.String[i])
                    {

                    }
                }
            }
            return new List<int>();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int x, string pattern2)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(int, int)> Matches(string pattern1, int y_min, int y_max, string pattern2)
        {
            throw new NotImplementedException();
        }

        public class Edge
        {
            private readonly SuffixTree _tree;

            public Edge(SuffixTree tree, Node head)
            {
                _tree = tree;
                Head = head;
                StartIndex = tree.CurrentSuffixEndIndex;
                EdgeNumber = _tree.NextEdgeNumber++;
            }

            public Node Head { get; private set; }
            public Node Tail { get; private set; }
            public int StartIndex { get; private set; }
            public int? EndIndex { get; set; }
            public int EdgeNumber { get; private set; }
            public int Length { get { return (EndIndex ?? _tree.Word.Length - 1) - StartIndex + 1; } }

            public Edge SplitAtIndex(int index)
            {
                _tree.SendMessage("Splitting edge {0} at index {1} ('{2}')", this, index, _tree.Word[index]);
                var newEdge = new Edge(_tree, Head);
                var newNode = new Node(_tree);
                newEdge.Tail = newNode;
                newEdge.StartIndex = StartIndex;
                newEdge.EndIndex = index - 1;
                Head = newNode;
                StartIndex = index;
                newNode.Edges.Add(_tree.Word[StartIndex], this);
                newEdge.Head.Edges[_tree.Word[newEdge.StartIndex]] = newEdge;
                _tree.SendMessage(" => Hierarchy is now: {0} --> {1} --> {2} --> {3}", newEdge.Head, newEdge, newNode, this);
                return newEdge;
            }

            public override string ToString()
            {
                return string.Concat(_tree.Word.Substring(StartIndex, (EndIndex ?? _tree.CurrentSuffixEndIndex) - StartIndex + 1), "(",
                    StartIndex, ",", EndIndex.HasValue ? EndIndex.ToString() : "#", ")");
            }

            public string StringWithoutCanonizationChar
            {
                get { return _tree.Word.Substring(StartIndex, (EndIndex ?? _tree.CurrentSuffixEndIndex - (_tree.CanonizationChar.HasValue ? 1 : 0)) - StartIndex + 1); }
            }

            public string String
            {
                get { return _tree.Word.Substring(StartIndex, (EndIndex ?? _tree.CurrentSuffixEndIndex) - StartIndex + 1); }
            }

            public void RenderTree(TextWriter writer, string prefix, int maxEdgeLength)
            {
                var strEdge = _tree.Word.Substring(StartIndex, (EndIndex ?? _tree.CurrentSuffixEndIndex) - StartIndex + 1);
                writer.Write(strEdge);
                if (Tail == null)
                    writer.WriteLine();
                else
                {
                    var line = new string(RenderChars.HorizontalLine, maxEdgeLength - strEdge.Length + 1);
                    writer.Write(line);
                    Tail.RenderTree(writer, string.Concat(prefix, new string(' ', strEdge.Length + line.Length)));
                }
            }

            public void WriteDotGraph(StringBuilder sb)
            {
                if (Tail == null)
                    sb.AppendFormat("leaf{0} [label=\"\",shape=point]", EdgeNumber).AppendLine();
                string label, weight, color;
                if (_tree.ActiveEdge != null && ReferenceEquals(this, _tree.ActiveEdge))
                {
                    if (_tree.ActiveEdge.Length == 0)
                        label = "";
                    else if (_tree.DistanceIntoActiveEdge > Length)
                        label = "<<FONT COLOR=\"red\" SIZE=\"11\"><B>" + String + "</B> (" + _tree.DistanceIntoActiveEdge + ")</FONT>>";
                    else if (_tree.DistanceIntoActiveEdge == Length)
                        label = "<<FONT COLOR=\"red\" SIZE=\"11\">" + String + "</FONT>>";
                    else if (_tree.DistanceIntoActiveEdge > 0)
                        label = "<<TABLE BORDER=\"0\" CELLPADDING=\"0\" CELLSPACING=\"0\"><TR><TD><FONT COLOR=\"blue\"><B>" + String.Substring(0, _tree.DistanceIntoActiveEdge) + "</B></FONT></TD><TD COLOR=\"black\">" + String.Substring(_tree.DistanceIntoActiveEdge) + "</TD></TR></TABLE>>";
                    else
                        label = "\"" + String + "\"";
                    color = "blue";
                    weight = "5";
                }
                else
                {
                    label = "\"" + String + "\"";
                    color = "black";
                    weight = "3";
                }
                var tail = Tail == null ? "leaf" + EdgeNumber : "node" + Tail.NodeNumber;
                sb.AppendFormat("node{0} -> {1} [label={2},weight={3},color={4},size=11]", Head.NodeNumber, tail, label, weight, color).AppendLine();
                if (Tail != null)
                    Tail.WriteDotGraph(sb);
            }
        }

        public class Node
        {
            private readonly SuffixTree _tree;

            public Node(SuffixTree tree)
            {
                _tree = tree;
                Edges = new Dictionary<char, Edge>();
                NodeNumber = _tree.NextNodeNumber++;
            }

            public Dictionary<char, Edge> Edges { get; private set; }
            public Node LinkedNode { get; set; }
            public int NodeNumber { get; private set; }

            public void AddNewEdge()
            {
                _tree.SendMessage("Adding new edge to {0}", this);
                var edge = new Edge(_tree, this);
                Edges.Add(_tree.Word[_tree.CurrentSuffixEndIndex], edge);
                _tree.SendMessage(" => {0} --> {1}", this, edge);
            }

            public void RenderTree(TextWriter writer, string prefix)
            {
                var strNode = string.Concat("(", NodeNumber.ToString(new string('0', _tree.NextNodeNumber.ToString().Length)), ")");
                writer.Write(strNode);
                var edges = Edges.Select(kvp => kvp.Value).OrderBy(e => _tree.Word[e.StartIndex]).ToArray();
                if (edges.Any())
                {
                    var prefixWithNodePadding = prefix + new string(' ', strNode.Length);
                    var maxEdgeLength = edges.Max(e => (e.EndIndex ?? _tree.CurrentSuffixEndIndex) - e.StartIndex + 1);
                    for (var i = 0; i < edges.Length; i++)
                    {
                        char connector, extender = ' ';
                        if (i == 0)
                        {
                            if (edges.Length > 1)
                            {
                                connector = RenderChars.TJunctionDown;
                                extender = RenderChars.VerticalLine;
                            }
                            else
                                connector = RenderChars.HorizontalLine;
                        }
                        else
                        {
                            writer.Write(prefixWithNodePadding);
                            if (i == edges.Length - 1)
                                connector = RenderChars.CornerRight;
                            else
                            {
                                connector = RenderChars.TJunctionRight;
                                extender = RenderChars.VerticalLine;
                            }
                        }
                        writer.Write(string.Concat(connector, RenderChars.HorizontalLine));
                        var newPrefix = string.Concat(prefixWithNodePadding, extender, ' ');
                        edges[i].RenderTree(writer, newPrefix, maxEdgeLength);
                    }
                }
            }

            public override string ToString()
            {
                return string.Concat("node #", NodeNumber);
            }

            public void WriteDotGraph(StringBuilder sb)
            {
                if (LinkedNode != null)
                    sb.AppendFormat("node{0} -> node{1} [label=\"\",weight=.01,style=dotted]", NodeNumber, LinkedNode.NodeNumber).AppendLine();
                foreach (var edge in Edges.Values)
                    edge.WriteDotGraph(sb);
            }
        }

        public static class RenderChars
        {
            public const char TJunctionDown = '┬';
            public const char HorizontalLine = '─';
            public const char VerticalLine = '│';
            public const char TJunctionRight = '├';
            public const char CornerRight = '└';
        }
    }
}