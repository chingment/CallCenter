using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{

    public class CarInsPKindModel
    {
        public CarInsPKindModel()
        {
            this.Child = new List<CarInsCKindModel>();
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public List<CarInsCKindModel> Child { get; set; }
    }

    public class CarInsCKindModel
    {
        public int Id { get; set; }
        public int PId { get; set; }
        public String Name { get; set; }
        public bool IsCheck { get; set; }
        public bool CanWaiverDeductible { get; set; }
        public bool IsWaiverDeductible { get; set; }
        public Enumeration.CarKindType Type { get; set; }
        public Enumeration.CarKindInputType InputType { get; set; }
        public string InputUnit { get; set; }
        public string InputValue { get; set; }
        public List<string> InputOption { get; set; }
        public bool IsHasDetails { get; set; }
    }
}
