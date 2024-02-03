using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlastPlayGround
{
	public class PICurve
	{
		public List<(double I, double P)> Points { get; private set; }
		private List<PICurve> curves;

		// Constructor for a single curve
		public PICurve(List<(double I, double P)> points)
		{
			Points = points ?? throw new ArgumentNullException(nameof(points));
			curves = new List<PICurve> { this }; // Initialize with this curve
		}

		// Constructor for multiple curves
		public PICurve(List<PICurve> curves)
		{
			if (curves == null || !curves.Any())
				throw new ArgumentException("Curves collection cannot be null or empty.", nameof(curves));

			this.curves = curves;
			Points = new List<(double I, double P)>(); // Initialize an empty list for points
		}

		// Check intersection for either a single curve or multiple curves		      

		public bool Intersects((double I, double P) loadPoint)
		{
			(double, double) origin = (0, 0);
			foreach (var curve in curves)
			{
				var firstPoint = curve.Points.First();
				var lastPoint = curve.Points.Last();

				// Edge case: Checking against the vertical extension from the first point
				if (loadPoint.I > firstPoint.I && loadPoint.P > firstPoint.P)
				{
					return true; // Intersection exists with the vertical extension
				}

				// Edge case: Checking against the horizontal extension from the last point
				if (loadPoint.I > lastPoint.I && loadPoint.P > lastPoint.P)
				{
					return true; // Intersection exists with the horizontal extension
				}

				// Regular intersection check within the curve segments
				for (int i = 0; i < curve.Points.Count - 1; i++)
				{
					if (LineSegmentIntersection.DoLineSegmentsIntersect(origin, loadPoint, curve.Points[i], curve.Points[i + 1]))
					{
						return true; // Intersection found within the curve
					}
				}
			}
			return false; // No intersection found
		}

		public int CountIntersectingCurves((double I, double P) loadPoint)
		{
			int count = 0;
			foreach (var curve in curves)
			{
				for (int i = 0; i < curve.Points.Count - 1; i++)
				{
					if (LineSegmentIntersection.DoLineSegmentsIntersect((0, 0), loadPoint, curve.Points[i], curve.Points[i + 1]))
					{
						count++;
						break; // Move to the next curve after finding an intersection
					}
				}
			}
			return count; // Return the count of intersecting curves
		}

	}

	internal class LineSegmentIntersection
	{
		internal static bool DoLineSegmentsIntersect((double, double) A, (double, double) B, (double, double) C, (double, double) D)
		{
			// Check if the line segments AB and CD intersect
			var orientation1 = GetOrientation(A, B, C);
			var orientation2 = GetOrientation(A, B, D);
			var orientation3 = GetOrientation(C, D, A);
			var orientation4 = GetOrientation(C, D, B);

			if (orientation1 != orientation2 && orientation3 != orientation4)
				return true; // The line segments intersect

			return false; // The line segments do not intersect
		}

		internal static int GetOrientation((double, double) P, (double, double) Q, (double, double) R)
		{
			double val = (Q.Item2 - P.Item2) * (R.Item1 - Q.Item1) - (Q.Item1 - P.Item1) * (R.Item2 - Q.Item2);

			if (val == 0) return 0;  // Collinear
			return (val > 0) ? 1 : 2; // Clockwise or Counterclock wise
		}

	}

}
