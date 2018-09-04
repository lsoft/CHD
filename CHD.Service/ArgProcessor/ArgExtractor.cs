using System;
using System.Collections.Generic;
using System.Linq;

namespace CHD.Service.ArgProcessor
{
    public sealed class ArgExtractor
    {
        private readonly string[] _args;

        public ArgExtractor(
            params string[] args)
        {
            _args = args ?? new string[0];
        }

        public int GetArgumentCount()
        {
            return
                _args != null ? _args.Length : 0;
        }


        public void Process(
            ref Arg arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }

            var uah = arg.Head.ToUpper();

            var tails = new List<string>();

            var alist = _args.ToList().FindAll(j => j.ToUpper().StartsWith(uah));
            foreach (var a in alist)
            {
                var tail = a.Substring(uah.Length);

                var item = Cleanup(tail);

                tails.Add(item);
            }

            if (tails.Count < arg.MinAllowedCount || tails.Count > arg.MaxAllowedCount)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "������ � ��������� {0}, �� ������������ {1} ���, � ��������� ������������ [{2};{3}] ���.",
                        arg.Head,
                        tails.Count,
                        arg.MinAllowedCount,
                        arg.MaxAllowedCount
                        ));
            }

            foreach (var tail in tails)
            {
                arg.AddTail(tail);
            }
        }

        private string Cleanup(string s)
        {
            var result = s;

            //������� ������� ���� ��� ������� � �����, � �� ��� � ��������
            if (result.StartsWith("\"") && result.EndsWith("\""))
            {
                if (result.Count(j => j == '\"') == 2)
                {
                    //������� ���� ������ ������� ������ � � ����� ������, � �������� ������ ������� ���
                    result = s.Substring(1, s.Length - 2);
                }
            }

            return result;
        }
    }
}
