namespace Klyte.Commons.Utils.UtilitiesClasses
{

    public class Tuple<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4>
    {
        public T5 Fifth { get; protected set; }
        internal Tuple(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth, ref T5 fifth) : base(ref first, ref second, ref third, ref fourth) => Fifth = fifth;
    }

    public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
    {
        public T4 Fourth { get; protected set; }
        internal Tuple(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth) : base(ref first, ref second, ref third) => Fourth = fourth;
    }

    public class Tuple<T1, T2, T3> : Tuple<T1, T2>
    {
        public T3 Third { get; protected set; }
        internal Tuple(ref T1 first, ref T2 second, ref T3 third) : base(ref first, ref second) => Third = third;
    }

    public class Tuple<T1, T2>
    {
        public T1 First { get; protected set; }
        public T2 Second { get; protected set; }
        internal Tuple(ref T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }
    }


    public class TupleRef<T1, T2, T3, T4, T5> : TupleRef<T1, T2, T3, T4>
    {
        private T5 m_fifth;
        public ref T5 Fifth => ref m_fifth;
        internal TupleRef(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth, ref T5 fifth) : base(ref first, ref second, ref third, ref fourth) => m_fifth = fifth;
    }

    public class TupleRef<T1, T2, T3, T4> : TupleRef<T1, T2, T3>
    {
        private T4 m_fourth;
        public ref T4 Fourth => ref m_fourth;
        internal TupleRef(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth) : base(ref first, ref second, ref third) => m_fourth = fourth;
    }

    public class TupleRef<T1, T2, T3> : TupleRef<T1, T2>
    {
        private T3 m_third;
        public ref T3 Third => ref m_third;
        internal TupleRef(ref T1 first, ref T2 second, ref T3 third) : base(ref first, ref second) => m_third = third;
    }

    public class TupleRef<T1, T2>
    {
        private T1 m_first;
        public ref T1 First => ref m_first;
        private T2 m_second;
        public ref T2 Second => ref m_second;
        internal TupleRef(ref T1 first, ref T2 second)
        {
            m_first = first;
            m_second = second;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2, T3, T4, T5> New<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            var tuple = new Tuple<T1, T2, T3, T4, T5>(ref first, ref second, ref third, ref fourth, ref fifth);
            return tuple;
        }
        public static Tuple<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            var tuple = new Tuple<T1, T2, T3, T4>(ref first, ref second, ref third, ref fourth);
            return tuple;
        }
        public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            var tuple = new Tuple<T1, T2, T3>(ref first, ref second, ref third);
            return tuple;
        }
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(ref first, ref second);
            return tuple;
        }
        public static TupleRef<T1, T2, T3, T4, T5> NewRef<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            var tuple = new TupleRef<T1, T2, T3, T4, T5>(ref first, ref second, ref third, ref fourth, ref fifth);
            return tuple;
        }
        public static TupleRef<T1, T2, T3, T4> NewRef<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            var tuple = new TupleRef<T1, T2, T3, T4>(ref first, ref second, ref third, ref fourth);
            return tuple;
        }
        public static TupleRef<T1, T2, T3> NewRef<T1, T2, T3>(ref T1 first, ref T2 second, ref T3 third)
        {
            var tuple = new TupleRef<T1, T2, T3>(ref first, ref second, ref third);
            return tuple;
        }
        public static TupleRef<T1, T2> NewRef<T1, T2>(ref T1 first, ref T2 second)
        {
            var tuple = new TupleRef<T1, T2>(ref first, ref second);
            return tuple;
        }
    }
}
