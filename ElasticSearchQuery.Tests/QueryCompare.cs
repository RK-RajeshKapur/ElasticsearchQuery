﻿// -----------------------------------------------------------------------
// <copyright file="QueryCompare.cs" company="Enterprise Products Partners L.P. (Enterprise)">
// © Copyright 2012 - 2019, Enterprise Products Partners L.P. (Enterprise), All Rights Reserved.
// Permission to use, copy, modify, or distribute this software source code, binaries or
// related documentation, is strictly prohibited, without written consent from Enterprise.
// For inquiries about the software, contact Enterprise: Enterprise Products Company Law
// Department, 1100 Louisiana, 10th Floor, Houston, Texas 77002, phone 713-381-6500.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Nest;

namespace ElasticsearchQuery.Tests
{
    public class QueryCompare
    {
        public static bool AreQueryContainersSame(IQueryContainer query1, IQueryContainer query2)
        {
            var nestedNullCount = NullCount(query1.Nested, query2.Nested);
            if (nestedNullCount == 1)
            {
                return false;
            }

            if (nestedNullCount == 2)
            {
                if (AreNestedQueriesSame(query1, query2) == false)
                {
                    return false;
                }
            }

            var nullCount = NullCount(query1.Bool, query2.Bool);
            if (nullCount == 0)
            {
                return AreTermsSame(query1, query2)
                & AreMatchQueriesSame(query1, query2)
                & ArePrefixQueriesSame(query1, query2)
                & AreRangeQueriesSame(query1, query2)
                & AreDateRangeQueriesSame(query1, query2);
            }
            if (nullCount == 1)
            {
                return false;
            }

            return AreBoolQueriesSame(query1, query2);
        }

        public static bool AreNestedQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            var nestedQuery1 = query1.Nested;
            var nestedQuery2 = query2.Nested;
            if (nestedQuery1.Path.ToString() != nestedQuery2.Path.ToString())
            {
                return false;
            }

            return AreQueryContainersSame(nestedQuery1.Query, nestedQuery2.Query);
        }

        public static bool AreBoolQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            var boolQuery1 = query1.Bool;
            var boolQuery2 = query2.Bool;
            return
                AreMustQueriesSame(boolQuery1, boolQuery2)
                & AreShouldQueriesSame(boolQuery1, boolQuery2)
            ;
        }

        public static bool AreMatchQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            var mustQuery1 = query1.Match;
            var mustQuery2 = query2.Match;
            var matchNullCount = NullCount(mustQuery1, mustQuery2);
            if (matchNullCount == 0)
            {
                return true;
            }

            if (matchNullCount == 1)
            {
                return false;
            }

            if (mustQuery1.Field != mustQuery2.Field)
            {
                return false;
            }

            if (mustQuery1.Query?.ToString() != mustQuery2.Query?.ToString())
            {
                return false;
            }

            return true;
        }

        public static bool ArePrefixQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            var mustQuery1 = query1.Prefix;
            var mustQuery2 = query2.Prefix;
            var matchNullCount = NullCount(mustQuery1, mustQuery2);
            if (matchNullCount == 0)
            {
                return true;
            }

            if (matchNullCount == 1)
                {
                return false;
            }
            if (mustQuery1.Field != mustQuery2.Field)
            {
                return false;
            }

            if (mustQuery1.Value?.ToString() != mustQuery2.Value?.ToString())
            {
                return false;
            }

            return true;
        }

        public static bool AreMustQueriesSame(IBoolQuery boolQuery1, IBoolQuery boolQuery2)
        {
            {
                var nullCount = NullCount(boolQuery1.Must, boolQuery2.Must);
                if (nullCount == 0)
                {
                    return true;
                }

                if (nullCount == 1)
                {
                    return false;
                }

                var mustQuery1 = boolQuery1.Must.ToList();
                var mustQuery2 = boolQuery2.Must.ToList();

                for (int i = 0; i < mustQuery1.Count; i++)
                {
                    var check = AreQueryContainersSame((IQueryContainer)mustQuery1[i], (IQueryContainer)mustQuery2[i]);
                    if (check == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreShouldQueriesSame(IBoolQuery boolQuery1, IBoolQuery boolQuery2)
        {
            {
                var nullCount = NullCount(boolQuery1.Should, boolQuery2.Should);
                if (nullCount == 0)
                {
                    return true;
                }

                if (nullCount == 1)
                {
                    return false;
                }

                var mustQuery1 = boolQuery1.Should.ToList();
                var mustQuery2 = boolQuery2.Should.ToList();

                for (int i = 0; i < mustQuery1.Count; i++)
                {
                    var check = AreQueryContainersSame((IQueryContainer)mustQuery1[i], (IQueryContainer)mustQuery2[i]);
                    if (check == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static int NullCount(object a, object b)
        {
            if (a == null && b == null)
            {
                return 0;
            }

            if (a == null || b == null)
            {
                return 1;
            }

            return 2;
        }

        public static bool AreTermsSame(IQueryContainer query1, IQueryContainer query2)
        {
            var nullCount = NullCount(query1.Term, query2.Term);
            if (nullCount == 0)
            {
                return true;
            }

            if (nullCount == 1)
            {
                return false;
            }

            if (query1.Term.Field != query2.Term.Field)
            {
                return false;
            }

            if (query1.Term.Value.ToString() != query2.Term.Value.ToString())
            {
                return false;
            }

            return true;
        }

        public static bool AreRangeQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            INumericRangeQuery range1;
            INumericRangeQuery range2;
            try
            {
                range1 = (INumericRangeQuery)query1.Range;
                range2 = (INumericRangeQuery)query2.Range;
            }
            catch (Exception)
            {
                return true;
            }

            var nullCount = NullCount(range1, range2);
            if (nullCount == 0)
            {
                return true;
            }
            if (nullCount == 1)
            {
                return false;
            }
            if (range1.Field != range2.Field)
            {
                return false;
            }
            if (range1.LessThan != range2.LessThan)
            {
                return false;
            }
            if (range1.LessThanOrEqualTo != range2.LessThanOrEqualTo)
            {
                return false;
            }
            if (range1.GreaterThan != range2.GreaterThan)
            {
                return false;
            }
            if (range1.GreaterThanOrEqualTo != range2.GreaterThanOrEqualTo)
            {
                return false;
            }
            return true;
        }

        public static bool AreDateRangeQueriesSame(IQueryContainer query1, IQueryContainer query2)
        {
            IDateRangeQuery range1;
            IDateRangeQuery range2;
            try
            {
                range1 = (IDateRangeQuery)query1.Range;
                range2 = (IDateRangeQuery)query2.Range;
            }
            catch (Exception)
            {
                return true;
            }
            var nullCount = NullCount(range1, range2);
            if (nullCount == 0)
            {
                return true;
            }
            if (nullCount == 1)
            {
                return false;
            }
            if (range1.Field != range2.Field)
            {
                return false;
            }
            if (range1.LessThan?.ToString() != range2.LessThan?.ToString())
            {
                return false;
            }
            if (range1.LessThanOrEqualTo?.ToString() != range2.LessThanOrEqualTo?.ToString())
            {
                return false;
            }
            if (range1.GreaterThan?.ToString() != range2.GreaterThan?.ToString())
                {
                return false;
            }
            if (range1.GreaterThanOrEqualTo?.ToString() != range2.GreaterThanOrEqualTo?.ToString())
            {
                return false;
            }
            return true;
        }

        public static bool AreSortsSame(SearchRequest req1, SearchRequest req2)
        {
            var sort1 = req1.Sort;
            var sort2 = req2.Sort;
            if (sort1.Count != sort2.Count)
            {
                return false;
            }
            for (int i = 0; i < sort2.Count; i++)
            {
                if (sort1[i].Order != sort2[i].Order || sort1[i].SortKey != sort2[i].SortKey)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AreAggregationDictionarySame(AggregationDictionary dict1, AggregationDictionary dict2)
        {
            return IsDictionaryCountSame(dict1, dict2) &&
                AreSumAggregationsSame(dict1.First().Value, dict2.First().Value) &&
                AreMinAggregationsSame(dict1.First().Value, dict2.First().Value) &&
            AreMaxAggregationsSame(dict1.First().Value, dict2.First().Value) &&
            AreAverageAggregationsSame(dict1.First().Value, dict2.First().Value);
        }

        public static bool IsDictionaryCountSame(AggregationDictionary dict1, AggregationDictionary dict2)
        {
            return dict1.Count() == dict2.Count() ? true : false;
        }

        public static bool AreSumAggregationsSame(IAggregationContainer dict1, IAggregationContainer dict2)
        {
            if (dict1.Sum != null && dict2.Sum != null)
            {
                return (dict1.Sum.Field.ToString().ToLower() == dict2.Sum.Field.ToString().ToLower()) &&
               (dict1.Sum.Name.ToString().ToLower() == dict2.Sum.Name.ToString().ToLower());
            }
            else if (dict1.Sum == null && dict2.Sum == null)
            {
                return true;
            }
            return false;
        }

        public static bool AreMinAggregationsSame(IAggregationContainer dict1, IAggregationContainer dict2)
        {
            if (dict1.Min != null && dict2.Min != null)
            {
                return (dict1.Min.Field.ToString().ToLower() == dict2.Min.Field.ToString().ToLower()) &&
                (dict1.Min.Name.ToString().ToLower() == dict2.Min.Name.ToString().ToLower());
            }
            else if (dict1.Min == null && dict2.Min == null)
            {
                return true;
            }
            return false;
        }

        public static bool AreMaxAggregationsSame(IAggregationContainer dict1, IAggregationContainer dict2)
        {
            if (dict1.Max != null && dict2.Max != null)
            {
                return (dict1.Max.Field.ToString().ToLower() == dict2.Max.Field.ToString().ToLower()) &&
                (dict1.Max.Name.ToString().ToLower() == dict2.Max.Name.ToString().ToLower());
            }
            else if (dict1.Max == null && dict2.Max == null)
            {
                return true;
            }
            return false;
        }

        public static bool AreAverageAggregationsSame(IAggregationContainer dict1, IAggregationContainer dict2)
        {
            if (dict1.Average != null && dict2.Average != null)
            {
                return (dict1.Average.Field.ToString().ToLower() == dict2.Average.Field.ToString().ToLower()) &&
                (dict1.Average.Name.ToString().ToLower() == dict2.Average.Name.ToString().ToLower());
            }
            else if (dict1.Average == null && dict2.Average == null)
            {
                return true;
            }
            return false;
        }
    }
}
