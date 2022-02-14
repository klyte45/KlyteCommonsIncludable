namespace Klyte.Commons.Utils
{
    public class Wrapper<T>
    {
        public Wrapper(T value) => Value = value;
        public Wrapper() { }

        public T Value { get; set; }
    }
}
