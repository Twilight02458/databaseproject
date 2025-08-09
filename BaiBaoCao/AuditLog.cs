using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiBaoCao
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; } // "INSERT", "UPDATE", "DELETE"
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
