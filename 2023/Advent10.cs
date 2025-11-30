using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.CompilerServices;

namespace Advent
{
    public class Advent10
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent10.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            PipeMap p = new PipeMap(lines);
            p.Walk();

            task1(p);
            task2(p);
        }

        private static void task1(PipeMap p) {
            //Console.WriteLine(p.ToString(1));
            Console.WriteLine("Max is: " + p.Squares.Max(row => row.Max(s => s.PathIndex)));
        }
//                 F--7                                                                                           F7                                 
//                 |  |                                                       
//  F--7           |  L--7                                                                                        |L7                                
//  |  |           |     |                                                    
//  |  |        F--J  F--J  F--7  F-----------7                                                            F7F7F--JFJ                                
//  |  |        |     |     |  |  |           |                               
//  |  |  F--7  L--7  L-----J  |  |  F--------J  I  I  I  I  F--7         F7                            F-7|||||F-7|F7    F7                         
//  |  |  |  |     |           |  |  |                       |  |             
//  |  L  J  L--7  L--7  F-----J  |  |  F  7  F  7  I  F  7  |  L  7IIIIII||IIIIIIIIIIIIIIIIIIF7IIIIF-7FJFJ|LJ|LJI|LJL7IIFJL7IIF7IIIIIIIIIIIIIIIIIIII
//              |     |  |        |  |                                        

        private static void task2(PipeMap p) {
            foreach (List<MapSquare> row in p.Squares){
                bool openUp=false,openDown=false;
                bool withinLoop = false;
                foreach(MapSquare ms in row) {
                    if (ms.PathIndex != -1){
                        if (ms.Pipe == '-'){
                            // nothing
                        }
                        else if (ms.Pipe == 'F') {
                            openDown = true;
                            openUp = false;
                        }
                        else if (ms.Pipe == 'L') {
                            openUp = true;
                            openDown = false;
                        }
                        else if (ms.Pipe == 'J') {
                            if (openDown) {
                                withinLoop = !withinLoop;
                            }
                            openDown = false;
                        }
                        else if (ms.Pipe == '7') {
                            if (openUp){
                                withinLoop = !withinLoop;
                            }
                            openUp = false;
                        }
                        else {
                            withinLoop = !withinLoop;
                            //Console.Write("O");
                        }
                        Console.Write(ms.Pipe);
                    }
                    else {
                        if (withinLoop) {
                            Console.Write("I");
                        }
                        else {
                            //Console.Write(ms.Pipe);
                            Console.Write(' ');
                        }
                    }
                    
                }
                Console.WriteLine();
            }
        }


// ............................................................................................................................................
// ............................F7..............................................................................................................
// ............................||..............................................................................................................
// ...........................FJ|............F7.............................................................F7.................................
// ...........................L7|.F7....F7...|L7............................................................|L7................................
// .........................F-7||.||....||..FJFJF7F---7..............................................F7F7F--JFJ................................
// .........................L7||L7||....||F7L7L-J||F--J....F7.......F7............................F-7|||||F-7|F7....F7.........................
// ..........................||L7||L-7F7|LJL7L7F-J||F7F7.F7|L7......||..................F7....F-7FJFJ|LJ|LJ.|LJL7..FJL7..F7....................
// .........................FJL-JLJF-J|||F7FJFJL--JLJ||L7|||FJ......|L-7F-7............FJL7...L7|L7L7L-7L7F-JF--JF7L7FJF-J|....................
// .........................L-7F-7FJ.FJ|||LJ.L7F--7F7|L7LJ||L-7...F7|F-JL7|F7.....F7...L-7L7.F-JL7|FJF-JFJL-7L-7FJ|FJL7|F-J....................
// ...........................||FJL7.L7LJL7F--JL-7LJLJ.|F7LJF-J..FJ||L7F-JLJ|.....|L7F-7.L7|.L-7FJ|L7|F-J.F7L7FJ|FJ|F7LJL7.....................


                        //    FJ|            F7                                                             F7                                 
                        //    L7| F7    F7   |L7                                                            |L7                                
                        //  F-7|| ||    ||  FJFJF7F---7                                              F7F7F--JFJ                                
                        //  L7||L7||    ||F7L7L-J||F--JIIIIF7       F7                            F-7|||||F-7|F7    F7                         
                        //   ||L7||L-7F7|LJL7L7F-J||F7F7IF7|L7IIIIII||IIIIIIIIIIIIIIIIIIF7IIIIF-7FJFJ|LJ|LJI|LJL7IIFJL7IIF7IIIIIIIIIIIIIIIIIIII
                        //  FJL-JLJF-J|||F7FJFJL--JLJ||L7|||FJIIIIII|



        // public enum Pipe {
        //      = "|", // is a vertical pipe connecting north and south.
        //      = "-", // is a horizontal pipe connecting east and west.
        //      = "L", // is a 90-degree bend connecting north and east.
        //      = "J", // is a 90-degree bend connecting north and west.
        //      = "7", // is a 90-degree bend connecting south and west.
        //      = "F", // is a 90-degree bend connecting south and east.
        //      = ".", // is ground; there is no pipe in this tile.
        //      = "S" // is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.
        // }


        public class PipeMap {
            public MapSquare EntryPoint;

            public List<List<MapSquare>> Squares;
            public PipeMap(string[] lines) {
                Squares = new List<List<MapSquare>>();
                for (int y=0; y < lines.Length; y++) {
                    List<MapSquare> row = new List<MapSquare>();
                    for (int x=0; x < lines[y].Length; x++) {
                        row.Add(new MapSquare(this, x, y, lines[y][x]));
                        if (row.Last().Pipe == 'S') {
                            EntryPoint = row.Last();
                        }
                    }
                    Squares.Add(row);
                }
            }

            public void Walk() {
                EntryPoint.PathIndex = 0;

                MapSquare[] paths = (new MapSquare[]{EntryPoint.North(), EntryPoint.East(), EntryPoint.South(), EntryPoint.West()}).Where(ms => ms != null).ToArray();
                MapSquare northbound = paths[0], southbound = paths[1];
                northbound.PathIndex = 1;
                southbound.PathIndex = 1;
                while (true) {
                    MapSquare nextNorth = next(northbound);
                    if (nextNorth.PathIndex >=0) {
                        break;
                    }
                    nextNorth.PathIndex = northbound.PathIndex + 1;
                    northbound = nextNorth;

                    MapSquare nextSouth = next(southbound);
                    if (nextSouth.PathIndex >=0) {
                        break;
                    }
                    nextSouth.PathIndex = southbound.PathIndex + 1;
                    southbound = nextSouth;
                }

            }

            private bool isNextValid(MapSquare current, MapSquare next) {
                return (next != null && (next.PathIndex < 0 || next.PathIndex > current.PathIndex));
            }

            private MapSquare next (MapSquare ms){
                MapSquare n = ms.North();
                if (isNextValid(ms, n)) {
                    return n;
                }
                MapSquare e = ms.East();
                if (isNextValid(ms, e)) {
                    return e;
                }
                MapSquare s = ms.South();
                if (isNextValid(ms, s)) {
                    return s;
                }
                MapSquare w = ms.West();
                if (isNextValid(ms, w)) {
                    return w;
                }
                throw new Exception();
            }

            public string ToString(int mode)
            {
                StringBuilder sb = new StringBuilder();
                for (int y=0; y < Squares.Count; y++) {
                    for (int x=0; x < Squares[y].Count; x++){
                        MapSquare ms = Squares[y][x];
                        if (mode == 0) {
                            sb.Append(ms.PathIndex >= 0 ? ms.PathIndex.ToString() : ms.Pipe);
                        }
                        if (mode == 1) {
                            sb.Append(ms.PathIndex >= 0 ? ms.PathIndex.ToString() : '.');
                        }
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }

        public class MapSquare {
            private readonly Advent10.PipeMap map;
            public readonly int X,Y;
            public readonly char Pipe;
            public int PathIndex=-1;

            public MapSquare(Advent10.PipeMap map, int x, int y, char pipe){
                this.map = map;
                X = x;
                Y = y;
                Pipe = pipe;
            }

            public const string ConnectFromNorth = "S|LJ";
            public const string ConnectFromEast = "S-LF";
            public const string ConnectFromSouth = "S|7F";
            public const string ConnectFromWest = "S-J7";

            public const string ConnectToNorth = "S|7F";
            public const string ConnectToEast = "S-J7";
            public const string ConnectToSouth = "S|LJ";
            public const string ConnectToWest = "S-LF";

            public MapSquare North(){
                if (Y <= 0) {
                    return null;
                }
                MapSquare to = map.Squares[Y-1][X];
                if (ConnectFromNorth.Contains(Pipe) && ConnectToNorth.Contains(to.Pipe)) {
                    return to;
                }
                return null;
            }

            public MapSquare East(){
                if (X >= map.Squares.Count - 1) {
                    return null;
                }
                MapSquare to = map.Squares[Y][X + 1];
                if (ConnectFromEast.Contains(Pipe) && ConnectToEast.Contains(to.Pipe)) {
                    return to;
                }
                return null;
            }

            public MapSquare South(){
                if (Y >= map.Squares[0].Count - 1) {
                    return null;
                }
                MapSquare to = map.Squares[Y+1][X];
                if (ConnectFromSouth.Contains(Pipe) && ConnectToSouth.Contains(to.Pipe)) {
                    return to;
                }
                return null;
            }

            public MapSquare West(){
                if (X <= 0) {
                    return null;
                }
                MapSquare to = map.Squares[Y][X-1];
                if (ConnectFromWest.Contains(Pipe) && ConnectToWest.Contains(to.Pipe)) {
                    return to;
                }
                return null;
            }

        }

    }
}
