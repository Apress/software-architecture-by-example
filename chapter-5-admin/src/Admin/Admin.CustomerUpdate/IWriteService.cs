﻿using Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.CustomerUpdate
{
    public interface IWriteService
    {
        void Write(IEnumerable<Admin.Common.CustomerModel> customers, string file);
    }
}
