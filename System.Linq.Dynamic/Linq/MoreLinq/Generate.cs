#region License and Terms
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace System.Linq.Dynamic
{
    using System;
    using System.Collections.Generic;

    static partial class MoreEnumerable
    {
        /// <summary>
        /// Returns a sequence of values consecutively generated by a generator function.
        /// </summary>
        /// <typeparam name="TResult">Type of elements to generate.</typeparam>
        /// <param name="initial">Value of first element in sequence</param>
        /// <param name="generator">
        /// Generator function which takes the previous series element and uses it to generate the next element.
        /// </param>
        /// <remarks>
        /// This function defers element generation until needed and streams the results.
        /// </remarks>
        /// <example>
        /// <code>
        /// IEnumerable&lt;int&gt; result = Sequence.Generate(2, n => n * n).Take(5);
        /// </code>
        /// The <c>result</c> variable, when iterated over, will yield 2, 4, 16, 256, and 65536, in turn.
        /// </example>        

        public static IEnumerable<TResult> Generate<TResult>(TResult initial, Func<TResult, TResult> generator)
        {
            if (generator == null) throw new ArgumentNullException("generator");
            return GenerateImpl(initial, generator);
        }

        private static IEnumerable<TResult> GenerateImpl<TResult>(TResult initial, Func<TResult, TResult> generator)
        {
            var current = initial;
            while (true)
            {
                yield return current;
                current = generator(current);
            }
        }
    }
}
