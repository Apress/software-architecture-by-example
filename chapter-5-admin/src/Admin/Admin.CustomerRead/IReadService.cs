using Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.CustomerRead
{
    public interface IReadService
    {
        IEnumerable<CustomerModel> ReadAll(string dataFile);
        CustomerModel Read(string dataFile, string customer);
    }
}
