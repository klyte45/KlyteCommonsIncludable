using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class CoroutineWithData<T> where T : class
    {
        public Coroutine Coroutine { get; private set; }
        public T result;
        public IEnumerator<T> m_target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator<T> target)
        {
            m_target = target;
            Coroutine = owner.StartCoroutine(Run());
        }



        private IEnumerator Run()
        {
            while (m_target.MoveNext())
            {
                result = m_target.Current;
                yield return result;
            }
        }
    }

    public static class CoroutineWithData
    {
        public static CoroutineWithData<U> From<U>(MonoBehaviour owner, IEnumerator<U> target) where U : class => new CoroutineWithData<U>(owner, target);
    }
}
