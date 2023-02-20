using System;
using System.Collections.Generic;

namespace ConsoleApp.DataStructures{


class SuffixTreeOther {
    private class Node {
        public int start, end;
        public Dictionary<char, Node> children = new Dictionary<char, Node>();
        public Node suffixLink;
        public Node(int start, int end) {
            this.start = start;
            this.end = end;
        }
        public int EdgeLength() {
            return Math.Min(end, SuffixTreeOther.lastIndex + 1) - start;
        }
    }

    private Node root = new Node(-1, -1);
    private static string inputString;
    private static int lastIndex;
    private Node activeNode = null;
    private int activeEdge = -1;
    private int activeLength = 0;
    private int remainingSuffixCount = 0;
    private Node lastNewNode = null;
    private static List<Node> nodes = new List<Node>();

    public SuffixTreeOther(string inputString) {
        SuffixTreeOther.inputString = inputString;
        lastIndex = inputString.Length - 1;
        root.suffixLink = root;
        activeNode = root;
        for (int i = 0; i <= lastIndex; i++) {
            AddSuffix(i);
        }
        
    }

    private void AddSuffix(int endIndex) {
        lastNewNode = null;
        remainingSuffixCount++;
        while (remainingSuffixCount > 0) {
            if (activeLength == 0) {
                activeEdge = endIndex;
            }
            if (!activeNode.children.ContainsKey(inputString[activeEdge])) {
                activeNode.children[inputString[activeEdge]] = new Node(endIndex, lastIndex);
                AddSuffixLink(activeNode);
            } else {
                Node next = activeNode.children[inputString[activeEdge]];
                if (WalkDown(next)) {
                    continue;
                }
                if (inputString[next.start + activeLength] == inputString[endIndex]) {
                    activeLength++;
                    AddSuffixLink(activeNode);
                    break;
                }
                Node splitNode = new Node(next.start, next.start + activeLength - 1);
                activeNode.children[inputString[activeEdge]] = splitNode;
                splitNode.children[inputString[endIndex]] = new Node(endIndex, lastIndex);
                next.start += activeLength;
                splitNode.children[inputString[next.start]] = next;
                AddSuffixLink(splitNode);
                lastNewNode = splitNode;
            }
            remainingSuffixCount--;
            if (activeNode == root && activeLength > 0) {
                activeLength--;
                activeEdge = endIndex - remainingSuffixCount + 1;
            } else {
                activeNode = activeNode.suffixLink ?? root;
            }
        }
    }

    private void AddSuffixLink(Node node) {
        if (lastNewNode != null) {
            lastNewNode.suffixLink = node;
        }
        lastNewNode = node;
    }

    private bool WalkDown(Node next) {
        if (activeLength >= next.EdgeLength()) {
            activeEdge += next.EdgeLength();
            activeLength -= next.EdgeLength();
            activeNode = next;
            return true;
        }
        return false;
    }

    private void PrintEdges(Node node) {
        if (node == null) {
            return;
        }
        if (node.start != -1) {
            Console.WriteLine(inputString.Substring(node.start, node.end - node.start + 1));
        }
        foreach (char key in node.children.Keys) {
            PrintEdges(node.children[key]);
        }
    }

    public void Visualize() {
        PrintEdges(root);
    }

    private IEnumerable<int> reportAllOccurrences(String p) {
        HashSet<int> ints = new HashSet<int>();
        Queue<Node> toCheck = new Queue<Node>();
        toCheck.Enqueue(root);
        string rest = p;
        bool matched = false;
        while(toCheck.Count != 0) {
            string currPref = "";
            Node curr = toCheck.Dequeue();
            foreach (var child in curr.children.Values)
            {
                if (inputString[child.start] == rest[0] && !matched)
                {
                    if (rest.Length > (child.end - child.start))
                    {
                        currPref = rest.Substring(0,child.end - child.start);
                        string nodeSubstring = inputString.Substring(child.start, child.end);
                        if (currPref == nodeSubstring)
                        {
                            ints.Add(curr.start);
                            rest = rest.Substring(child.end - child.start);
                            foreach (var c in child.children.Values)
                            {
                                toCheck.Enqueue(c);
                            }
                        }
                    } else {
                        string nodeSubstring = inputString.Substring(child.start, child.start+rest.Length);
                        if (rest == nodeSubstring)
                        {
                            ints.Add(curr.start);
                            rest = "";
                            matched = true;
                            toCheck.Clear();
                            foreach (var c in child.children.Values)
                            {
                                toCheck.Enqueue(c);
                            }
                        }
                    }
                    
                } else if (matched)
                {
                    ints.Add(curr.start);
                    foreach (var c in curr.children.Values)
                    {
                        toCheck.Enqueue(c);
                    }
                }
            }
        }


        return ints;

    }
    public IEnumerable<(int,int)> ReportAllOccurrences(Query query){
        var p1occs = reportAllOccurrences(query.P1);
        var p2occs = reportAllOccurrences(query.P2);
        var occs = new HashSet<(int,int)>();
            foreach (var item in p1occs)
            {
                if (p2occs.Contains(item + query.P1.Length + query.X))
                {
                    occs.Add((item, item + query.P1.Length + query.X + query.P2.Length));
                    global::System.Console.WriteLine((item, item + query.P1.Length + query.X + query.P2.Length));
                }
                    
            }
        return occs;
    }
}
}
