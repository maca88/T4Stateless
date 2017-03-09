using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4Stateless.Tests
{
    public class TestService
    {
        public bool IsValid(bool value)
        {
            return value;
        }

        public async Task<bool> IsValidAsync(bool value)
        {
            await Task.Yield();
            return value;
        }
    }
}
