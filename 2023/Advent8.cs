using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Numerics;

namespace Advent
{
    public class Advent8
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent8.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //task1(lines);
            task2(lines);
        }

        private static void task1(string[] lines) {
            CircularEnumerator<char> directions = new CircularEnumerator<char>(lines[0]);
            List<Node> nodes = parseNodes(lines.Skip(1));

            int count = 0;
            Node currentNode = nodes.First(n => n.isRoot);
            while(directions.MoveNext()){
                count ++;
                currentNode = currentNode.Follow(directions.Current);
                if (currentNode.isFinal){
                    break;
                }
            }
            Console.WriteLine("Step count: " + count);
        }


        private static void task2(string[] lines) {
            CircularEnumerator<char> directions = new CircularEnumerator<char>(lines[0]);
            List<Node> nodes = parseNodes(lines.Skip(1));

            BigInteger test1 = leastCommonMultiple(2, 5);
            BigInteger test2 = leastCommonMultiple(4,6,8);


            List<Node> ghostNodes = nodes.Where(n => n.isGhostRoot).ToList();

            List<long> loops = new List<long>();
            foreach (Node n in ghostNodes) {
                loops.Add(findLoop(directions, n));
                directions.Reset();
            }

            BigInteger result = leastCommonMultiple(loops.ToArray());

            Console.WriteLine("Ghost Step count: " + result);

            // while(directions.MoveNext()){
            //     count ++;
            //     List<Node> nextGhostNodes = new List<Node>();
            //     foreach (Node gN in ghostNodes) {
            //         Node n = gN.Follow(directions.Current);
            //         nextGhostNodes.Add(n);
            //     }

            //     if (nextGhostNodes.Count(gn => gn.isGhostFinal) > 1) {
                    
            //     }
            //     ;
            //     if (nextGhostNodes.All(gn => gn.isGhostFinal)) {
            //         break;
            //     }
            //     ghostNodes = nextGhostNodes;
            // }
            // Console.WriteLine("Ghost Step count: " + count);
        }

        private static long findLoop(CircularEnumerator<char> directions, Node searchNode) {
            int count = 0;
            Node startNode = searchNode;
            int startIndex = directions.Index;


            Node node = searchNode;
            while(directions.MoveNext()){
                count ++;
                node = node.Follow(directions.Current);

                if (node.isGhostFinal) {
                    return count;
                }

                // if (nextGhostNodes.All(gn => gn.isGhostFinal)) {
                //     break;
                // }
            }

            throw new Exception("No loop found?!");
        }

        private static BigInteger leastCommonMultiple(params long[] divisors) {
            long[] numbers = divisors.OrderByDescending(x => x).ToArray();
            BigInteger[] multiples = new BigInteger[numbers.Length];
            BigInteger largest=0;
            for (int i=0; i < numbers.Length; i++){
                multiples[i] = numbers[i];
                if (numbers[i] > largest){
                    largest = numbers[i];
                }
            }

            for (int i=0; i < numbers.Length; ) {
                if (multiples[i] < largest) {
                    multiples[i] = (largest / numbers[i]) * numbers[i];
                    if (multiples[i] < largest) {
                        multiples[i] += numbers[i];
                    }
                }

                if (multiples[i] > largest) {
                    largest = multiples[i];
                    i = 0;
                }
                else if (multiples[i] == largest) {
                    i++;
                }
            }

            return largest;

            // long exactMatchCount=0;
            // for (int i=0; ; i = ((i + 1) % numbers.Length)) {
            //     while (multiples[i] < largest){
            //         multiples[i] += numbers[i];
            //     }

            //     if (multiples[i] == largest) {
            //         exactMatchCount ++;
            //     }
            //     else {
            //         largest = multiples[i];
            //         exactMatchCount = 0;
            //     }

            //     if (exactMatchCount == numbers.Length) {
            //         return largest;
            //     }
            // }


        }


        private static List<Node> parseNodes(IEnumerable<string> lines)
        {
            Dictionary<string, Node> nodes = new Dictionary<string, Node>();

            foreach (string line in lines){
                Node raw = parseRawNode(line);
                nodes.Add(raw.Key, raw);
            }

            foreach (Node n in nodes.Values) {
                n.L = nodes[n.lKey];
                n.R = nodes[n.rKey];
            }

            return nodes.Values.ToList();
        }

        private static Node parseRawNode(string line) {
            CharEnumerator c = line.GetEnumerator();
            c.MoveNext();

            Node n = new Node();
            readKey(n, c);
            readLinkedNodes(n, c);
            return n;
        }

        private static void readKey(Node n, IEnumerator<char> c){
            StringBuilder key = new StringBuilder();
            while (c.Current != '=') {
                key.Append(c.Current);
                c.MoveNext();
            }
            n.Key = key.ToString().Trim();
        }

        private static void readLinkedNodes(Node n, IEnumerator<char> c) {
            while (c.Current != '(') {
                c.MoveNext();
            }
            c.MoveNext();

            StringBuilder left = new StringBuilder();
            while (c.Current != ',') {
                left.Append(c.Current);
                c.MoveNext();
            }
            n.lKey = left.ToString().Trim();

            c.MoveNext();
            StringBuilder right = new StringBuilder();
            while (c.Current != ')'){
                right.Append(c.Current);
                c.MoveNext();
            }
            n.rKey = right.ToString().Trim();
        }

        private class Node {
            public string Key;
            public string lKey;
            public Node L;
            public string rKey;
            public Node R;

            public Node (){}
            public Node (string key, string l, string r) {
                Key = key;
                lKey = l;
                rKey = r;
            }

            public bool isRoot {
                get {
                    return this.Key == "AAA";
                }
            }

            public bool isGhostRoot {
                get {
                    return this.Key.EndsWith("A");
                }
            }

            public bool isFinal {
                get {
                    return this.Key == "ZZZ";
                }
            }

            public bool isGhostFinal {
                get {
                    return this.Key.EndsWith('Z');
                }
            }

            public Node Follow (char direction){
                if (direction == 'R'){
                    return R;
                }
                else if (direction == 'L'){
                    return L;
                }
                else {
                    throw new Exception("Invalid direction");
                }
            }

            public override string ToString()
            {
                return Key + " = (" + lKey + ", " + rKey + ")";
            }

        }
    }
}
