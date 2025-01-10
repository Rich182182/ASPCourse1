using Rich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rich.DataAccess.Repository.IReposetory
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
