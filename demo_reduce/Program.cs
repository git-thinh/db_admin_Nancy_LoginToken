using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;

namespace demo_reduce
{
    class Program
    {
        public static Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool> Function()
        {
            Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool> where = kv_item =>
            {
                if (kv_item.Key.Item2 > 1100000 && kv_item.Key.Item2 < 1199999)
                    return true;
                return false;
            };
            return where;
        }

        static void Main(string[] args)
        {
            //Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool> where = kv_item =>
            //{
            //    if (kv_item.Key.Item2 > 1100000 && kv_item.Key.Item2 < 1199999)
            //        return true;
            //    return false;
            //};

            //DictionaryList<Tuple<long, int, int>, double> db = new DictionaryList<Tuple<long, int, int>, double>() { };

            //var ls = db.Where(where).Select(x => x.Value).ToArray();

            List<Tuple<long, int, int>> ls = new List<Tuple<long, int, int>>() {
                new Tuple<long,int,int>(123,878687,10000),
                new Tuple<long,int,int>(345,123567,10030),
                new Tuple<long,int,int>(678,678345,10100),
                new Tuple<long,int,int>(357,398456,10130),
                new Tuple<long,int,int>(683,578932,10200),
            };

            var l0 = ls.Where("Item1 == 123 || Item2 == 578932");
            var l1 = ls.Where("Item1 == 123 || Item2 == 578932").Cast<Tuple<long, int, int>>().ToArray();
            var l2 = ls.Where("Item1 == 123 || Item2 == 578932").Cast<Tuple<long, int, int>>()
                .SortMultiple(new List<Tuple<string, string>>() { new Tuple<string, string>("Item3", "desc") })
                .ToArray();

            List<double[]> dt = new List<double[]>() {
                    new double[] { 1,2,3,4,5,6,7,8,9 },
                    new double[] { 2213,123,321,123,435,657,98 },
                    new double[] { 345,77,878,32,45,989 },
            };

            var baseQuery = new int[] { 1, 2, 3, 4, 5 }.AsQueryable();

            //Using the it keyword in a select statement
            var result1 = baseQuery.Select("it * $").ToDynamicArray();

            //Using the it keyword in a where statement
            var result2 = baseQuery.Where("it % 2 = 0").ToDynamicArray();


            var d0 = dt.Select(" it[0] ").ToDynamicArray();
            var d1 = dt.Where(" it[0] > 1000 ").ToDynamicArray();
            var d2 = dt.Where(" it[3] == 2").ToDynamicArray();

            var d3 = dt.Select(" new (0 as t1, it as t2) ").ToDynamicArray();

            var d4 = dt.Select(x =>
            {
                List<double> li = x.ToList();
                li.Insert(0, 99);
                return li.ToArray();
            }).ToDynamicArray();


        }
    }
}
