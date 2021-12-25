using System.Collections.Generic;
using System.Threading.Tasks;

namespace Task5.Models
{
    public class UsersCheckModel
    {
        public Task<List<User>> Users { get; set; }
        public IEnumerable<string> Checked { get; set; }
    }
}
