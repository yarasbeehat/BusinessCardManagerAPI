﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Domain.Entities
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Result { get; set; }
        public int TotalCount { get; set; }
    }
}
