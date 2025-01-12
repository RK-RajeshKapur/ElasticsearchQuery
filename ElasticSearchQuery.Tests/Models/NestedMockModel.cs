﻿// -----------------------------------------------------------------------
// <copyright file="NestedMockModel.cs" company="Enterprise Products Partners L.P. (Enterprise)">
// © Copyright 2012 - 2019, Enterprise Products Partners L.P. (Enterprise), All Rights Reserved.
// Permission to use, copy, modify, or distribute this software source code, binaries or
// related documentation, is strictly prohibited, without written consent from Enterprise.
// For inquiries about the software, contact Enterprise: Enterprise Products Company Law
// Department, 1100 Louisiana, 10th Floor, Houston, Texas 77002, phone 713-381-6500.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nest;

namespace ElasticsearchQuery.Tests.Models
{
    public class NestedMockModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        [Nested]
        public IList<MockModel> MockModels { get; set; }

        public double Price { get; set; }
    }
}
