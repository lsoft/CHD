using System;
using System.Collections.Generic;

namespace CHD.Service.ArgProcessor
{
    public sealed class Arg
    {
        public string Head
        {
            get;
            private set;
        }

        public int MinAllowedCount
        {
            get;
            private set;
        }

        public int MaxAllowedCount
        {
            get;
            private set;
        }

        public string FirstTail
        {
            get
            {
                var result = string.Empty;

                if (TailCount > 0)
                {
                    result = Tails[0];
                }

                return result;
            }
        }

        public List<string> Tails
        {
            get;
            private set;
        }

        public int TailCount
        {
            get
            {
                return
                    Tails.Count;
            }
        }

        public bool Exists
        {
            get
            {
                return
                    TailCount > 0;
            }
        }

        /// <summary>
        ///  онструктор аргумента, который может встречатьс€ сколько угодно раз или не встречатьс€ вообще
        /// </summary>
        public Arg(string head)
        {
            if (head == null)
            {
                throw new ArgumentNullException("head");
            }

            Head = head;
            MinAllowedCount = -1;
            MaxAllowedCount = int.MaxValue;

            Tails = new List<string>();
        }

        /// <summary>
        ///  онструктор аргумента, который должен встречатьс€ ровно count раз
        /// </summary>
        /// <param name="head">√олова аргумента</param>
        /// <param name="count">—трогое количество раз, которое может встретитьс€ аргумент</param>
        public Arg(string head, int count)
        {
            if (head == null)
            {
                throw new ArgumentNullException("head");
            }

            Head = head;
            MinAllowedCount = count;
            MaxAllowedCount = count;

            Tails = new List<string>();
        }

        /// <summary>
        ///  онструктор агрумента
        /// </summary>
        /// <param name="head">√олова аргумента</param>
        /// <param name="minAllowedCount">ћинимальное количество раз, которое может встретитьс€ аргумент</param>
        /// <param name="maxAllowedCount">ћаксимальное количество раз, которое может встретитьс€ аргумент</param>
        public Arg(string head, int minAllowedCount, int maxAllowedCount)
        {
            if (head == null)
            {
                throw new ArgumentNullException("head");
            }

            Head = head;
            MinAllowedCount = minAllowedCount;
            MaxAllowedCount = maxAllowedCount;

            Tails = new List<string>();
        }

        public void AddTail(string tail)
        {
            if (tail == null)
            {
                throw new ArgumentNullException("tail");
            }

            Tails.Add(tail);
        }
    }
}