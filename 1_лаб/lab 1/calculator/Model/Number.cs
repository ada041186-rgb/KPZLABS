using calculator.Enumes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.Model
{
    public class Number
    {
        public double Value { get; set; }
        public Operators Operation { get; set; } = Operators.None;
    }
}
