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
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get document and session
            UIApplication uiApplication = commandData.Application;
            Document doc = uiApplication.ActiveUIDocument.Document;

            // Get selection
            Selection selec = uiApplication.ActiveUIDocument.Selection;
            List<CurveElement> selecLine = new List<CurveElement>();

            // Start transaction 1
            Transaction transaction1 = new Transaction(doc);
            transaction1.Start("Selection");
            Reference selecRef = selec.PickObject(ObjectType.Element, "Select Lines");
            CurveElement ele = doc.GetElement(selecRef) as CurveElement;
            selecLine.Add(ele);
            transaction1.Commit();

            // Get points and angle
            Line line = selecLine[0].GeometryCurve as Autodesk.Revit.DB.Line;
            List<double> segments = new List<double> { 0, 0.5, 1 };
            Function func = new Function();
            List<XYZ> points = func.GetPoints(line, segments);
            double angle = line.Direction.AngleTo(new XYZ(0, 1, 0));

            // Get family
            FilteredElementCollector famCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilySymbol));
            FilteredElementIterator famIterator = famCollector.GetElementIterator();
            //famIterator.Reset();
            FamilySymbol fam = doc.GetElement(famIterator.Current.Id) as FamilySymbol;
            while (famIterator.MoveNext())
            {
                ElementId famId = famIterator.Current.Id;
                FamilySymbol famSym = doc.GetElement(famId) as FamilySymbol;
                if (famSym.FamilyName == "my_cube")
                {
                    fam = famSym;
                }
            }

            // Start transaction 2
            Transaction transaction2 = new Transaction(doc);
            transaction2.Start("Create Element");
            for (int i = 0; i < points.Count; i++)
            {
                FamilyInstance famInstance = doc.Create.NewFamilyInstance(points[i], fam,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                ElementTransformUtils.RotateElement(doc, famInstance.Id, func.VerticalLine(points[i]), angle);
            }
            transaction2.Commit();

            return Result.Succeeded;
        }
    }
}
