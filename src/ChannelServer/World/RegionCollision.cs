﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Aura.Channel.Util;
using Aura.Data.Database;
using Aura.Shared.Util;
using Aura.Mabi.Const;

namespace Aura.Channel.World
{
	public class RegionCollision
	{
		private Quadtree<LinePath> _tree;

		private Dictionary<long, List<LinePath>> _reference;

		/// <summary>
		/// Creates new collision manager for region.
		/// </summary>
		public RegionCollision()
		{
			_tree = new Quadtree<LinePath>(new Size(1000, 1000), 2);
			_reference = new Dictionary<long, List<LinePath>>();
		}

		/// <summary>
		/// Adds shapes of entity to collision collection.
		/// </summary>
		/// <param name="shapedEntity"></param>
		public void Add(IShapedEntity shapedEntity)
		{
			if (shapedEntity.Shapes == null || shapedEntity.Shapes.Count == 0 || !shapedEntity.IsCollision)
				return;

			foreach (var points in shapedEntity.Shapes)
			{
				var line1 = new LinePath(points[0], points[1]);
				var line2 = new LinePath(points[1], points[2]);
				var line3 = new LinePath(points[2], points[3]);
				var line4 = new LinePath(points[3], points[0]);

				lock (_tree)
				{
					_tree.Insert(line1);
					_tree.Insert(line2);
					_tree.Insert(line3);
					_tree.Insert(line4);
				}

				lock (_reference)
				{
					if (!_reference.ContainsKey(shapedEntity.EntityId))
						_reference[shapedEntity.EntityId] = new List<LinePath>();

					_reference[shapedEntity.EntityId].Add(line1);
					_reference[shapedEntity.EntityId].Add(line2);
					_reference[shapedEntity.EntityId].Add(line3);
					_reference[shapedEntity.EntityId].Add(line4);
				}
			}
		}

		/// <summary>
		/// Removes collision objects with the given ident.
		/// </summary>
		/// <param name="ident"></param>
		public void Remove(long entityId)
		{
			// Remove lines from tree
			lock (_reference)
			{
				if (!_reference.ContainsKey(entityId))
					return;

				foreach (var obj in _reference[entityId])
					lock (_tree)
						_tree.Remove(obj);

				// Remove references
				_reference.Remove(entityId);
			}
		}

		/// <summary>
		/// Returns true if any intersections are found between from and to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public bool Any(Position from, Position to)
		{
			Position intersection;
			return this.Find(from, to, out intersection);
		}

		/// <summary>
		/// Returns true if any intersections are found in range of
		/// the position.
		/// </summary>
		/// <remarks>
		/// Runs 4 intersection checks to cover 8 directions around position.
		/// </remarks>
		/// <param name="pos"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool AnyInRange(Position pos, int range)
		{
			Position intersection;
			return
				this.Find(new Position(pos.X - range, pos.Y), new Position(pos.X + range, pos.Y), out intersection) ||
				this.Find(new Position(pos.X, pos.Y - range), new Position(pos.X, pos.Y + range), out intersection) ||
				this.Find(new Position(pos.X - range, pos.Y - range), new Position(pos.X + range, pos.Y + range), out intersection) ||
				this.Find(new Position(pos.X - range, pos.Y + range), new Position(pos.X + range, pos.Y - range), out intersection);
		}

		/// <summary>
		/// Returns true if the path between from and to intersects with
		/// anything and returns the intersection position via out.
		/// </summary>
		/// <param name="region"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="intersection"></param>
		/// <returns></returns>
		public bool Find(Position from, Position to, out Position intersection)
		{
			intersection = to;

			var x1 = from.X;
			var y1 = from.Y;
			var x2 = to.X;
			var y2 = to.Y;

			var intersections = new List<Position>();

			// Query lines
			var rect = new LinePath(from, to).Rect;

			// Extend rect a little, so there's no chance to miss any lines.
			rect.X -= 100;
			rect.Y -= 100;
			rect.Width += 200;
			rect.Height += 200;

			List<LinePath> lines;
			lock (_tree)
				lines = _tree.Query(rect);

			// Get intersections
			foreach (var line in lines)
			{
				var x3 = line.P1.X;
				var y3 = line.P1.Y;
				var x4 = line.P2.X;
				var y4 = line.P2.Y;

				Position inter;
				if (this.FindIntersection(x1, y1, x2, y2, x3, y3, x4, y4, out inter))
					intersections.Add(inter);
			}

			// No collisions
			if (intersections.Count < 1)
				return false;

			// One collision
			if (intersections.Count == 1)
			{
				intersection = intersections[0];
				return true;
			}

			// Select nearest intersection
			double distance = double.MaxValue;
			foreach (var inter in intersections)
			{
				var interDist = Math.Pow(x1 - inter.X, 2) + Math.Pow(y1 - inter.Y, 2);
				if (interDist < distance)
				{
					intersection = inter;
					distance = interDist;
				}
			}

			return true;
		}

		/// <summary>
		/// Returns whether the lines x1/y1-x2/y2 and x3/y3-x4/y4 intersect.
		/// The intersection point is returned in the corresponding out-variable.
		/// </summary>
		private bool FindIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, out Position intersection)
		{
			intersection = Position.Zero;

			double denom = ((x2 - x1) * (y4 - y3)) - ((y2 - y1) * (x4 - x3));
			if (denom == 0) return false; // parallel 

			double numer = ((y1 - y3) * (x4 - x3)) - ((x1 - x3) * (y4 - y3));
			double r = numer / denom;
			double numer2 = ((y1 - y3) * (x2 - x1)) - ((x1 - x3) * (y2 - y1));
			double s = numer2 / denom;
			if ((r < 0 || r > 1) || (s < 0 || s > 1)) return false; // nointersect

			double interX = x1 + (r * (x2 - x1));
			double interY = y1 + (r * (y2 - y1));

			intersection = new Position((int)interX, (int)interY);

			return true;
		}
	}

	/// <summary>
	/// Holding two points, making up a path.
	/// </summary>
	public class LinePath : IQuadObject
	{
		public Point P1 { get; private set; }
		public Point P2 { get; private set; }
		public RectangleF Rect { get; private set; }

		public LinePath(Point p1, Point p2)
		{
			this.P1 = p1;
			this.P2 = p2;
			this.Rect = new Rectangle(
				Math.Min(P1.X, P2.X),
				Math.Min(P1.Y, P2.Y),
				Math.Abs(P1.X - P2.X),
				Math.Abs(P1.Y - P2.Y)
			);
		}

		public LinePath(Position p1, Position p2)
			: this(new Point(p1.X, p1.Y), new Point(p2.X, p2.Y))
		{
		}

		public override string ToString()
		{
			return ("(" + P1.X + "," + P1.Y + " - " + P2.X + "," + P2.Y + ")");
		}
	}

	public interface IShapedEntity
	{
		long EntityId { get; }
		bool IsCollision { get; }
		List<Point[]> Shapes { get; }
	}
}
