namespace Klyte.Commons.Utils.UtilitiesClasses
{

    public class MutableTuple<T1, T2, T3, T4, T5> : MutableTuple<T1, T2, T3, T4>
    {
        public T5 Fifth { get; set; }
        internal MutableTuple(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth, ref T5 fifth) : base(ref first, ref second, ref third, ref fourth) => Fifth = fifth;
    }

    public class MutableTuple<T1, T2, T3, T4> : MutableTuple<T1, T2, T3>
    {
        public T4 Fourth { get; set; }
        internal MutableTuple(ref T1 first, ref T2 second, ref T3 third, ref T4 fourth) : base(ref first, ref second, ref third) => Fourth = fourth;
    }

    public class MutableTuple<T1, T2, T3> : MutableTuple<T1, T2>
    {
        public T3 Third { get; set; }
        internal MutableTuple(ref T1 first, ref T2 second, ref T3 third) : base(ref first, ref second) => Third = third;
    }

    public class MutableTuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        internal MutableTuple(ref T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public static class MutableTuple
    {
        public static MutableTuple<T1, T2, T3, T4, T5> New<T1, T2, T3, T4, T5>(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
        {
            var MutableTuple = new MutableTuple<T1, T2, T3, T4, T5>(ref first, ref second, ref third, ref fourth, ref fifth);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            var MutableTuple = new MutableTuple<T1, T2, T3, T4>(ref first, ref second, ref third, ref fourth);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            var MutableTuple = new MutableTuple<T1, T2, T3>(ref first, ref second, ref third);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var MutableTuple = new MutableTuple<T1, T2>(ref first, ref second);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2, T3, T4> NewRef<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            var MutableTuple = new MutableTuple<T1, T2, T3, T4>(ref first, ref second, ref third, ref fourth);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2, T3> NewRef<T1, T2, T3>(ref T1 first, ref T2 second, ref T3 third)
        {
            var MutableTuple = new MutableTuple<T1, T2, T3>(ref first, ref second, ref third);
            return MutableTuple;
        }
        public static MutableTuple<T1, T2> NewRef<T1, T2>(ref T1 first, ref T2 second)
        {
            var MutableTuple = new MutableTuple<T1, T2>(ref first, ref second);
            return MutableTuple;
        }
    }
}
