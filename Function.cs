using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddin
{
    internal class Function
    {
        public List<XYZ> GetPoints(Line line, List<double> segments)
        {
            List<XYZ> points = new List<XYZ>();
            for (int i = 0; i < segments.Count; i++)
            {
                XYZ point = line.GetEndPoint(0).Add(line.Direction.Normalize().Multiply(segments[i] * line.Length));
                points.Add(point);
            }
            return points;
        }

        public Line VerticalLine(XYZ point)
        {
            return Line.CreateBound(point, point.Add(new XYZ(0, 0, 1)));
        }
    }
}
