using System;
using System.IO;
using System.Linq;

namespace Advent
{
    public class Advent1
    {

        public void task1 (){
            string calDoc;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent1.txt")) {
                calDoc = sr.ReadToEnd();
            }

            int sum = calDoc.Split("\n")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => Int32.Parse(firstNum(line) + lastNum(line)))
                .Sum();
            Console.WriteLine("total sum:" + sum);
        }

        string firstNum(string line){
            for(int i=0; i < line.Length; i++){
                if (line[i] >= '0' && line[i] <= '9') {
                    Console.WriteLine($"first num: {line} = {line[i].ToString()}");
                    return line[i].ToString();
                }
                string w = isWord(line, i);
                if (w != null) {
                    Console.WriteLine($"first num: {line} = {w}");
                    return w;
                }
            }
            throw new Exception("no number found");
        }

        string lastNum(string line){
            for(int i=line.Length -1; i >= 0; i--){
                if (line[i] >= '0' && line[i] <= '9') {
                    Console.WriteLine($"last num: {line} = {line[i].ToString()}");
                    return line[i].ToString();
                }
                string w = isWord(line, i);
                if (w != null) {
                    Console.WriteLine($"last num: {line} = {w}");
                    return w;
                }
            }
            throw new Exception("no number found");
        }

        private Tuple<string, string>[] words = new Tuple<string, string>[] {
                                new Tuple<string, string>("one", "1"),
                                new Tuple<string, string>("two", "2"),
                                new Tuple<string, string>("three", "3"),
                                new Tuple<string, string>("four", "4"),
                                new Tuple<string, string>("five", "5"),
                                new Tuple<string, string>("six", "6"),
                                new Tuple<string, string>("seven", "7"),
                                new Tuple<string, string>("eight", "8"),
                                new Tuple<string, string>("nine", "9")};
        string isWord(string line, int index){
            foreach (Tuple<string, string> word in words){
                for (int i=0; ; i++) {
                    if (i == word.Item1.Length) {
                        return word.Item2;
                    }
                    if (index + i >= line.Length){
                        break;
                    }
                    if (line[index + i] != word.Item1[i]){
                        break;
                    }
                }
            }
            return null;
        }

    }
}