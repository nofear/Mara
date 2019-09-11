//--------------------------------------------------------------------------------

using System;
using MaraSolver;
using MaraInterval.Interval;
using MaraSolver.Integer;
using System.Collections.Generic;

//--------------------------------------------------------------------------------
namespace TestApp {

	public class Rect {
		public Rect(int w, int h) {
			Width = w;
			Height = h;
		}

		public int Width { get; }
		public int Height { get; }
		public int Area {
			get {
				return Width * Height;
			}
		}

		public override string ToString() {
			return Width + "x" + Height + " (" + Area + ")";
		}
	}

	public class Mondriaan: Problem {
		public Mondriaan(int size) :
			base(0, size * size) {
			RectangleList = new List<Rect>();
			for(int h = 1; h <= size - 1; ++h) {
				for(int w = h; w <= size; ++w) {
					RectangleList.Add(new Rect(w, h));
				}
			}

			RectangleList.Sort((lhs, rhs) => lhs.Area.CompareTo(rhs.Area));


			int max = RectangleList.Count - 1;
			IntInterval range = new IntInterval(0, max);

			IntObjective objective = Solver.IntObjective;
			objective.Var = Area;
			objective.Value = (size * (size - 1)) - (size * 1) + 1;

			Area = new IntVar(Solver, 0, objective.Value, "area difference");
			Matrix = new IntVarMatrix(Solver, size, size, range);

			//Solver.Add(new CornerLess(this, 0, 0, 0, size - 1));
			//Solver.Add(new CornerLess(this, 0, size - 1, size - 1, size - 1));
			//Solver.Add(new CornerLess(this, size - 1, size - 1, size - 1, 0));

			// Solver.Add(new AreaMatrixCons(this));
			// Solver.Add(new MatrixTopLeftCons(this));
			Solver.Add(new MatrixCons(this));

			for(int row = 0; row < size; row++) {
				for(int col = 0; col < size; col++) {
					Solver.Add(new CellReduce(row, col, this));
				}
			}
		}

		public IntVarMatrix Matrix { get; }

		public IntVar RectangleSet { get; }

		public List<Rect> RectangleList { get; }

		public IntVar Area { get; set; }

		public bool CellHasValue(int row, int col, int value) {
			IntVar cell = Matrix.Cell(row, col);
			return cell.IsBound() && cell.Value == value;
		}

		public bool RectFit(int row0, int col0, int h, int w) {
			return row0 + h <= Matrix.RowCount
				&& col0 + w <= Matrix.ColCount;
		}

		public bool RectIsBound(int row0, int col0, int h, int w) {
			if(!RectFit(row0, col0, h, w)) {
				return false;
			}

			for(int row = row0; row < row0 + h; row++) {
				for(int col = col0; col < col0 + w; col++) {
					IntVar cell = Matrix.Cell(row, col);
					if(!cell.IsBound()) {
						return false;
					}
				}
			}

			return true;
		}

		public bool RectHasValue(int row0, int col0, int h, int w, int value) {
			for(int row = row0; row < row0 + h; row++) {
				for(int col = col0; col < col0 + w; col++) {
					if(!CellHasValue(row, col, value)) {
						return false;
					}
				}
			}

			return true;
		}


		public bool RectContainsValue(int row0, int col0, int h, int w, int value) {
			for(int row = row0; row < row0 + h; row++) {
				for(int col = col0; col < col0 + w; col++) {
					IntVar cell = Matrix.Cell(row, col);
					if(!cell.Domain.Contains(value)) {
						return false;
					}
				}
			}

			return true;
		}

		public void AssignRect(int row0, int col0, int h, int w, int value) {
			// Solver.Out.WriteLine("AssignRect: row0=" + row0 + ", col0=" + col0 + ", w=" + w + ", h=" + h + ", r=" + r);

			for(int row = row0; row < row0 + h; row++) {
				for(int col = col0; col < col0 + w; col++) {
					Matrix.Cell(row, col).Intersect(value);
				}
			}
		}

		public void ExcludeRect(int row0, int col0, int h, int w, int value) {
			for(int row = 0; row < Matrix.RowCount; row++) {
				for(int col = 0; col < Matrix.ColCount; col++) {
					bool inRect = col >= col0 && col < (col0 + w)
										&& row >= row0 && row < (row0 + h);
					if(!inRect) {
						Matrix.Cell(row, col).Difference(value);
					}
				}
			}
		}

	}








}

//--------------------------------------------------------------------------------
