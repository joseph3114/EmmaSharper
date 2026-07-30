using System.Collections;

namespace EmmaSharper
{
    public class Dates : IEnumerable
    {
        public Date? Key { get; set; }
        public int Value { get; set; }

        public Dates() { }

        public IEnumerator GetEnumerator() => GetEnumerator();
    }
}
