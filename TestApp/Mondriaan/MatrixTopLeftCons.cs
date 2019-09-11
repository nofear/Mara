//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace TestApp {
	public class MatrixTopLeftCons: ConstraintVarList0<IntVarList> {

		private Mondriaan m_Mondriaan;

		public MatrixTopLeftCons(Mondriaan m) :
			base(m.Matrix.VarList) {

			m_Mondriaan = m;
		}

		public override void Post() {
			Post(Variable.How.OnDomain);
		}

		public override void Update() {
			IntVarMatrix matrix = m_Mondriaan.Matrix;

			for(int row = 0; row < matrix.RowCount; ++row) {
				for(int col = 0; col < matrix.RowCount; ++col) {
					IntVar cell = matrix.Cell(row, col);
					if(cell.IsBound()) {
						continue;
					}

					if((row == 0 || matrix.Cell(row - 1, col).IsBound())
						&& (col == 0 || matrix.Cell(row, col - 1).IsBound())) {

						IntDomain d = IntDomain.Empty;
						foreach(int r in cell) {
							Rect rect = m_Mondriaan.RectangleList[r];

							bool fitH = m_Mondriaan.RectFit(row, col, rect.Height, rect.Width);
							bool fitV = m_Mondriaan.RectFit(row, col, rect.Width, rect.Height);
							if(fitH || fitV) {
								d = d.Union(r);
							}
						}

						cell.Intersect(d);
					}
				}
			}
		}
	}
}
