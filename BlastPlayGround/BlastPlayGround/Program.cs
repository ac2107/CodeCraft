using BlastPlayGround;

// DoLineSegmentsIntersect() usage
(double, double) A = (0.0, 0.0);
(double, double) B = (2.0, 2.0);
(double, double) C = (0.0, 2.0);
(double, double) D = (2.0, 0.0);
bool result = LineSegmentIntersection.DoLineSegmentsIntersect(A, B, C, D);
Console.WriteLine($"Do the line segments intersect? {result}");

// Single curve
var singleCurve = new PICurve(new List<(double, double)> { (1, 3), (2, 2), (3, 1) });
Console.WriteLine($"Single curve intersects: {singleCurve.Intersects((2, 3))}");

// Multiple curves
var curve1 = new PICurve(new List<(double, double)> { (1, 3), (2, 2), (3, 1) });
var curve2 = new PICurve(new List<(double, double)> { (0.5, 4), (1.6, 3), (3, 2) });
var multipleCurves = new PICurve(new List<PICurve> { curve1, curve2 });
Console.WriteLine($"Multiple curves intersect: {multipleCurves.Intersects((2, 2.5))}");
Console.WriteLine($"Multiple curves intersect count: {multipleCurves.CountIntersectingCurves((2, 3))}");


