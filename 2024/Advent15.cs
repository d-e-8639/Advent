using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.Net;
using Advent.lib;

namespace Advent.A2024
{
    public class Advent15
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent15.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> lines2 = lines.SplitBy("").ToList();


            Grid<WHContent> warehouse = new Grid<WHContent>(
                lines2[0].Select(row => row.Select(
                    c => (WHContent)
                        (c == '#' ? new Wall() :
                         c == 'O' ? new Box() : 
                         c == '@' ? new Robot() :
                         c == '.' ? null :
                         throw new Exception())
                )));
            warehouse.NullItemToStringValue = ".";

            Console.WriteLine(warehouse.ToString());

            List<Direction> robotOrders = lines2[1].SelectMany(l => l)
                                            .Where(c => "^v<>".Contains(c))
                                            .Select(c =>
                                                c == '^' ? Direction.Up :
                                                c == 'v' ? Direction.Down :
                                                c == '<' ? Direction.Left :
                                                c == '>' ? Direction.Right :
                                                throw new Exception())
                                            .ToList();

            //task1(warehouse, robotOrders);

            List<List<WHContent>> wh2Items = new List<List<WHContent>>();
            foreach (string line in lines2[0]) {
                wh2Items.Add(new List<WHContent>());
                foreach (char c in line) {
                    wh2Items.Last().AddRange(
                        (c == '#' ? new WHContent[] {new Wall(), new Wall()} :
                         c == 'O' ? new LargeBox(new BoxPart('['), new BoxPart(']')).Parts : 
                         c == '@' ? new WHContent[] {new Robot(), null} :
                         c == '.' ? new WHContent[] {null, null} :
                         throw new Exception())
                    );
                }
            }
            Grid<WHContent> warehouse2 = new Grid<WHContent>(wh2Items);
            warehouse2.NullItemToStringValue = ".";

            Console.WriteLine(warehouse2.ToString());


            task2(warehouse2, robotOrders);
        }

        private static void task1(Grid<WHContent> warehouse, List<Direction> robotOrders) {
            Robot robot = warehouse.First(x => x.Item is Robot).Item as Robot;

            foreach (Direction d in robotOrders) {
                robot.Shove(d);
            }

            Console.WriteLine(warehouse.ToString());

            Console.Write("GPS Sum: " + gpsSum(warehouse));
        }

        private static void task2(Grid<WHContent> warehouse2, List<Direction> robotOrders) {
            Robot robot = warehouse2.First(x => x.Item is Robot).Item as Robot;

            foreach (Direction d in robotOrders) {
                robot.Shove(d);
            }

            Console.WriteLine(warehouse2.ToString());

            long sum=0;
            foreach (GridItem<WHContent> box in warehouse2.Where(x => x.Item is BoxPart && (x.Item as BoxPart)._display == '[')) {
                sum += 100 * box.Y + box.X;
            }

            Console.WriteLine("GPS Sum: " + sum);
        }

        private static string gpsSum(Grid<WHContent> warehouse) {
            long sum=0;
            foreach (GridItem<WHContent> box in warehouse.Where(x => x.Item is Box)) {
                sum += 100 * box.Y + box.X;
            }
            return sum.ToString();
        }

        public abstract class WHContent : IGridRegisterable<WHContent>
        {
            GridItem<WHContent> gridRef;
            public long ShoveAttemptSuccessful = -1;

            public void Register(GridItem<WHContent> gridItem)
            {
                gridRef = gridItem;
            }

            private static long _shoveAttempt = 0;

            public bool Shove(Direction d) {
                long shoveAttempt = _shoveAttempt++;
                List<GridItem<WHContent>> toMove = new List<GridItem<WHContent>>();
                if (Shove(d, toMove, shoveAttempt)) {
                    // Everything is moveable, so move it
                    foreach (GridItem<WHContent> item in toMove) {
                        item.Move(getDirectionItem(item, d));
                    }
                    return true;
                }
                // Not all movable, so move nothing
                return false;
            }

            protected virtual bool Shove(Direction d, List<GridItem<WHContent>> toMove, long shoveAttempt) {
                GridItem<WHContent> behind = getDirectionItem(this.gridRef, d);


                if (behind.IsEmpty() || behind.Item.ShoveAttemptSuccessful == shoveAttempt) {
                    //this.gridRef.Move(behind);
                    toMove.Add(this.gridRef);
                    this.ShoveAttemptSuccessful = shoveAttempt;
                    return true;
                }
                else if (behind.Item.Moveable) {
                    if (behind.Item.Shove(d, toMove, shoveAttempt)) {
                        //this.gridRef.Move(behind);
                        toMove.Add(this.gridRef);
                        this.ShoveAttemptSuccessful = shoveAttempt;
                        return true;
                    }
                }

                return false;
            }

            private static GridItem<WHContent> getDirectionItem(GridItem<WHContent> item, Direction d) {
                if (d == Direction.Up) {
                    return item.Up;
                }
                else if (d == Direction.Down) {
                    return item.Down;
                }
                else if (d == Direction.Left) {
                    return item.Left;
                }
                else if (d == Direction.Right) {
                    return item.Right;
                }
                throw new Exception();
            }

            public abstract bool Moveable {get;}
        }

        public class Robot : WHContent {
            public override bool Moveable => throw new NotSupportedException();


            public override string ToString()
            {
                return "@";
            }
        }

        public class Box : WHContent {
            public override bool Moveable => true;


            public override string ToString()
            {
                return "O";
            }
        }

        public class LargeBox {
            public BoxPart[] Parts;

            public LargeBox(params BoxPart[] parts) {
                Parts = parts;
                foreach (BoxPart b in parts) {b.lBox = this;}
            }

            internal bool Shove(Direction d, List<GridItem<WHContent>> toMove, long shoveAttempt)
            {
                if (Parts.All(p => p.VerticalShove(d, toMove, shoveAttempt))) {
                    return true;
                }
                return false;
            }
        }

        public class BoxPart : WHContent {
            public override bool Moveable => true;

            public LargeBox lBox;
            public char _display;

            public BoxPart(char display) {
                _display = display;
            }

            protected override bool Shove(Direction d, List<GridItem<WHContent>> toMove, long shoveAttempt)
            {
                if (d == Direction.Left || d == Direction.Right) {
                    return base.Shove(d, toMove, shoveAttempt);
                }
                return lBox.Shove(d, toMove, shoveAttempt);
            }

            public bool VerticalShove(Direction d, List<GridItem<WHContent>> toMove, long shoveAttempt) {
                return base.Shove(d, toMove, shoveAttempt);
            }

            public override string ToString()
            {
                return _display.ToString();
            }
        }

        public class Wall : WHContent {
            public override bool Moveable => false;


            public override string ToString()
            {
                return "#";
            }
        }

        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }

    }
}
