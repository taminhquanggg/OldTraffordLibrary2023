using OldTraffordLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldTraffordLibrary.Service
{
    public class BookService
    {
        public List<tbl_Book> GetListBook()
        {
            var dbContext = new OldTraffordLibraryEntities();
            return dbContext.tbl_Book.ToList();
        }
    }
}
