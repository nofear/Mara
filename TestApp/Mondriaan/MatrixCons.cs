//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace TestApp {
	public class MatrixCons: ConstraintVarList0<IntVarList> {

		private Mondriaan m_Mondriaan;

		public MatrixCons(Mondriaan m) :
			base(m.Matrix.VarList) {

			m_Mondriaan = m;
		}

		public override void Post() {
			Post(Variable.How.OnDomain);
		}

		public override void Update() {
			IntVarMatrix matrix = m_Mondriaan.Matrix;

			IntDomain placed = IntDomain.Empty;

			for(int row = 0; row < matrix.RowCount; ++row) {
				for(int col = 0; col < matrix.RowCount; ++col) {
					IntVar cell = matrix.Cell(row, col);
					if(!cell.IsBound()) {
						continue;
					}

					if(cell.IsEmpty()){
						break;
					}

					int value = cell.Value;
					Rect rect = m_Mondriaan.RectangleList[value];

					if(placed.Contains(value)) {
						continue;
					}

					bool fitH = m_Mondriaan.RectFit(row, col, rect.Height, rect.Width);
					bool fitV = m_Mondriaan.RectFit(row, col, rect.Width, rect.Height);

					if(fitH && m_Mondriaan.RectHasValue(row, col, rect.Height, rect.Width, value)
						|| fitV && m_Mondriaan.RectHasValue(row, col, rect.Width, rect.Height, value)) {
						placed = placed.Union(value);
						m_Mondriaan.ExcludeRect(row, col, rect.Height, rect.Width, value);
						continue;
					}
					
					placed = placed.Union(value);

					int min = Math.Min(rect.Height, rect.Width);

					if(fitH) {
						fitH &= m_Mondriaan.RectContainsValue(row, col, rect.Height, rect.Width, value); 
					}

					if(fitV) {
						fitV &= m_Mondriaan.RectContainsValue(row, col, rect.Width, rect.Height, value);
					}
					
					if(!fitH && !fitV) {
						cell.Difference(value);
						// TODO other cells

					} else if(rect.Height == rect.Width) {
						m_Mondriaan.AssignRect(row, col, rect.Height, rect.Width, value);
						m_Mondriaan.ExcludeRect(row, col, rect.Height, rect.Width, value);

					} else if(fitH && !fitV) {
						m_Mondriaan.AssignRect(row, col, rect.Height, rect.Width, value);
						m_Mondriaan.ExcludeRect(row, col, rect.Height, rect.Width, value);

					} else if(!fitH && fitV) {
						m_Mondriaan.AssignRect(row, col, rect.Width, rect.Height, value);
						m_Mondriaan.ExcludeRect(row, col, rect.Width, rect.Height, value);

					} else {
						m_Mondriaan.AssignRect(row, col, min, min, value);

						IntVar cellH = matrix.Cell(row, col + min);
						IntVar cellV = matrix.Cell(row + min, col);

						if(cellH.IsBound() && cellH.Value == value) {
							m_Mondriaan.AssignRect(row, col, rect.Height, rect.Width, value);
							m_Mondriaan.ExcludeRect(row, col, rect.Height, rect.Width, value);

						} else if(cellV.IsBound() && cellV.Value == value) {
							m_Mondriaan.AssignRect(row, col, rect.Width, rect.Height, value);
							m_Mondriaan.ExcludeRect(row, col, rect.Width, rect.Height, value);

						} else {
							m_Mondriaan.AssignRect(row, col, rect.Height, rect.Width, value);
							m_Mondriaan.ExcludeRect(row, col, rect.Height, rect.Width, value);

						}

					}
				}

			}
		}
	}
}
