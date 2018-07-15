using Nancy.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host
{
    public class fileRouteNancy : RouteSegmentConstraintBase<string>
    {
        public override string Name
        {
            get { return "file"; }
        }

        protected override bool TryMatch(string constraint, string segment, out string matchedValue)
        {
            if (segment.Contains("."))
            {
                matchedValue = segment;
                return true;
            }

            matchedValue = null;
            return false;
        }
    }
}
