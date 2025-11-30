using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HelloWorld
{
    public class Grid<T> : IEnumerable<GridItem<T>>
        where T: class
    {
        public List<List<GridItem<T>>> Items;

        public int Height => Items.Count;
        public int Width => Items.First().Count;

        public Grid(IEnumerable<IEnumerable<T>> items) {
            Items = new List<List<GridItem<T>>>();
            foreach (IEnumerable<T> rawRow in items) {
                List<GridItem<T>> row = new List<GridItem<T>>();
                foreach (T item in rawRow) {
                    row.Add(new GridItem<T>(this, item, row.Count, Items.Count));
                }
                Items.Add(row);
            }

            for (int y=0; y < Items.Count; y++) {
                for (int x=0; x < Items[y].Count; x++) {
                    GridItem<T> gi = Items[y][x];

                    if (x > 0) {
                        gi.Left = Items[y][x-1];
                    }
                    if (y > 0) {
                        gi.Up = Items[y-1][x];
                    }
                    if (x < Items[0].Count - 1) {
                        gi.Right = Items[y][x+1];
                    }
                    if (y < Items.Count - 1){
                        gi.Down = Items[y+1][x];
                    }

                }
            }
        }

        public IEnumerator<GridItem<T>> GetEnumerator()
        {
            return new GridEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public GridItem<T> GetAt(int x, int y, bool SuppressIndexOutOfRangeException=false) {
            if (SuppressIndexOutOfRangeException) {
                if (y < 0 || y >= Items.Count) {
                    return null;
                }
                if (x < 0 || x >= Items.First().Count) {
                    return null;
                }
            }
            return Items[y][x];
        }

        public T GetItemAt(int x, int y, bool SuppressIndexOutOfRangeException=false) {
            GridItem<T> item = GetAt(x, y, SuppressIndexOutOfRangeException);
            if (item == null) {
                return default(T);
            }
            return item.Item;
        }

        public string NullItemToStringValue = "";
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<GridItem<T>> row in Items) {
                foreach (GridItem<T> item in row) {
                    sb.Append(item.ToString(NullItemToStringValue));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public interface IGridRegisterable<T>
        where T: class
    {
        void Register(GridItem<T> gridItem);
    }

    public interface IGridDeregisterable<T>
        where T: class
    {
        void Deregister();
    }

    public class GridItem<T>
        where T: class
    {
        public Grid<T> Parent;
        public int X, Y;
        public T Item;

        public GridItem<T> Up=null, Down=null, Left=null, Right=null;
        public bool HasUp { get { return Up != null; } }
        public bool HasDown { get { return Down != null; } }
        public bool HasLeft { get { return Left != null; } }
        public bool HasRight { get { return Right != null; } }

        private GridItem<T>[] neighbors = null;
        public IEnumerable<GridItem<T>> Neighbors {
            get {
                if (neighbors == null) {
                    List<GridItem<T>> n = new List<GridItem<T>>();
                    if (Up != null) { n.Add(Up); }
                    if (Down != null) { n.Add(Down); }
                    if (Left != null) { n.Add(Left); }
                    if (Right != null) { n.Add(Right); }
                    neighbors = n.ToArray();
                }
                return neighbors;
            }
        }

        public GridItem(Grid<T> grid, T item, int x, int y)
        {
            this.Parent = grid;
            this.Item = item;
            this.X = x;
            this.Y = y;

            registerItem();
        }

        private void registerItem() {
            if (Item is IGridRegisterable<T> registerable) {
                registerable.Register(this);
            }
        }

        private void deregisterItem() {
            if (Item is IGridDeregisterable<T> deregisterable) {
                deregisterable.Deregister();
            }
        }

        public void Move(GridItem<T> target, bool overwrite=false) {
            if (! ReferenceEquals(this.Parent, target.Parent)) {
                throw new Exception("Moving to a different grid!??!?");
            }

            if (! target.IsEmpty()) {
                if (overwrite) {
                    target.deregisterItem();
                }
                else {
                    throw new Exception("Can't move this GridItem to target, target GridItem is not null.");
                }
            }

            target.Item = this.Item;
            this.Item = default;
            target.registerItem();
        }

        public void Swap(GridItem<T> target) {
            if (! ReferenceEquals(this.Parent, target.Parent)) {
                throw new Exception("Moving to a different grid!??!?");
            }

            T tmp = target.Item;

            target.Item = this.Item;
            target.registerItem();

            this.Item = tmp;
            this.registerItem();
        }

        public bool IsEmpty() {
            return this.Item == null;
        }

        public override string ToString()
        {
            return ToString("");
        }

        public string ToString(string nullItemToStringValue)
        {
            if (Item != null) {
                return Item.ToString();
            }
            return nullItemToStringValue;
        }
    }

    public class GridEnumerator<T> : IEnumerator<GridItem<T>>
        where T: class
    {
        private Grid<T> grid;
        private IEnumerator<List<GridItem<T>>> rowEnum = null;
        private IEnumerator<GridItem<T>> itemEnum = null;

        public GridEnumerator(Grid<T> grid) {
            this.grid = grid;
        }

        public GridItem<T> Current
        {
            get
            {
                return itemEnum.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            if (rowEnum == null) {
                rowEnum = grid.Items.GetEnumerator();
                if (! rowEnum.MoveNext()) {
                    return false;
                }
                itemEnum = rowEnum.Current.GetEnumerator();
                return itemEnum.MoveNext();
            }

            if (! itemEnum.MoveNext()) {
                if (! rowEnum.MoveNext()) {
                    return false;
                }
                itemEnum = rowEnum.Current.GetEnumerator();
                return itemEnum.MoveNext();
            }
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private object lockRef = new object();
        public void Dispose()
        {
            lock (lockRef) {
                if (rowEnum != null) {
                    rowEnum.Dispose();
                    rowEnum = null;
                }
            }
            lock (lockRef) {
                if (itemEnum != null) {
                    itemEnum.Dispose();
                    itemEnum = null;
                }
            }
        }

    }
}