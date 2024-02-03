using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlastPlayGround
{
	/// <summary>
	/// Represents a Pressure-Impulse (PI) curve or a collection of PI curves.
	/// This class can be used to determine if a given load point intersects any of the PI curves.
	/// </summary>
	public class PICurve
	{
		/// <summary>
		/// Gets the list of points defining the PI curve.
		/// </summary>
		public List<(double I, double P)> Points { get; private set; }

		/// <summary>Stores either a single PI curve or a collection of PI curves.</summary>
		/// This list is initialized to contain the current instance for a single curve
		/// or multiple instances for representing multiple curves. This design allows
		/// the Intersects method to uniformly handle both single and multiple curve scenarios.
		private List<PICurve> curves;

		/// <summary>
		/// Initializes a new instance of the PICurve class with a single PI curve.
		/// </summary>
		/// <param name="points">The points defining the PI curve.</param>
		public PICurve(List<(double I, double P)> points)
		{
			Points = points ?? throw new ArgumentNullException(nameof(points));
			curves = new List<PICurve> { this }; // Initialize with this curve
		}

		/// <summary>
		/// Initializes a new instance of the PICurve class with multiple PI curves.
		/// </summary>
		/// <param name="curves">A collection of PICurve instances representing multiple PI curves.</param>
		public PICurve(List<PICurve> curves)
		{
			if (curves == null || !curves.Any())
				throw new ArgumentException("Curves collection cannot be null or empty.", nameof(curves));

			this.curves = curves;
			Points = new List<(double I, double P)>(); // Initialize an empty list for points
		}

		/// <summary>
		/// Checks if the line segment from the origin to the specified load point intersects with any of the PI curves.
		/// Considers edge cases where the load point's line segment may extend beyond the range of the curve's points.
		/// </summary>
		/// <param name="loadPoint">The load point (I, P) to check for intersection.</param>
		/// <returns>True if there is an intersection with any of the PI curves; otherwise, false.</returns>		      
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

		/// <summary>
		/// Counts the number of PI curves that the line segment from the origin to the specified load point intersects.
		/// </summary>
		/// <param name="loadPoint">The load point (I, P) to check for intersection with the PI curves.</param>
		/// <returns>The count of PI curves that intersect with the line segment from the origin to the load point.</returns>
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

	/// <summary>
	/// Provides methods for determining if two line segments intersect.
	/// </summary>
	internal class LineSegmentIntersection
	{
		/// <summary>
		/// Checks if two line segments, defined by points A, B and C, D, intersect.
		/// </summary>
		/// <param name="A">The start point of the first line segment.</param>
		/// <param name="B">The end point of the first line segment.</param>
		/// <param name="C">The start point of the second line segment.</param>
		/// <param name="D">The end point of the second line segment.</param>
		/// <returns>True if the line segments intersect; otherwise, false.</returns>
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

		/// <summary>
		/// Calculates the orientation of three points in 2D space.
		/// </summary>
		/// <param name="P">First point.</param>
		/// <param name="Q">Second point.</param>
		/// <param name="R">Third point.</param>
		/// <returns>0 if collinear, 1 if clockwise, 2 if counterclockwise.</returns>
		internal static int GetOrientation((double, double) P, (double, double) Q, (double, double) R)
		{
			double val = (Q.Item2 - P.Item2) * (R.Item1 - Q.Item1) - (Q.Item1 - P.Item1) * (R.Item2 - Q.Item2);

			if (val == 0) return 0;		// Collinear
			return (val > 0) ? 1 : 2;	// Clockwise or Counterclock wise
		}

	}

}
