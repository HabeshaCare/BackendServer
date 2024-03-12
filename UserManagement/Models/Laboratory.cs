using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class Laboratory : Institution
    {
        public LabTest[] AvailableTests { get; set; } = Array.Empty<LabTest>();
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}