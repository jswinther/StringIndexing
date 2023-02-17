using System;
using System.Collections.Generic;

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
}
