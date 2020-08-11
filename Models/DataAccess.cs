using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace COMP2001_API.Models
{
    public class DataAccess
    {
        private readonly string _connection;

        public DataAccess(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("COMP2001_DB");
        }

    }
}
