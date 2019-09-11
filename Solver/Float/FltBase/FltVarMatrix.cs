//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltBase/FltVarMatrix.cs $
 * 
 * 11    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 10    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 9     11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 8     31-07-07 22:27 Patrick
 * removed obsolete Copy(..)
 * 
 * 7     7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 6     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 5     6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 4     22-03-07 23:29 Patrick
 * cleanup
 * 
 * 3     9-03-07 23:06 Patrick
 * updated copyright notice
 * 
 * 2     3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 1     6/02/06 9:14a Patrick
 * added float version
 * 
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;

using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float {
	/// <summary>
	/// Summary description for FltVarMatrix.
	/// </summary>
	public class FltVarMatrix: SolverBase {
		public FltVarMatrix(Solver solver, int rowCount, int colCount) :
			this(solver, rowCount, colCount, FltInterval.Whole) {
		}

		public FltVarMatrix(Solver solver, int rowCount, int colCount, FltInterval interval) :
			this(solver, rowCount, colCount, new FltDomain(interval)) {
		}

		public FltVarMatrix(Solver solver, int rowCount, int colCount, FltDomain domain) :
			base(solver) {
			m_VarList = null;
			m_RowCount = rowCount;
			m_ColCount = colCount;

			InitMatrix(domain);
		}

		public FltVarMatrix(Solver solver, int rowCount, int colCount, FltVarList list) :
			base(solver) {
			m_VarList = list;
			m_RowCount = rowCount;
			m_ColCount = colCount;
		}

		public override string ToString() {
			StringBuilder str = new StringBuilder();

			for(int row = 0; row < m_RowCount; ++row) {
				for(int col = 0; col < m_ColCount; ++col) {
					if(col > 0) {
						str.Append("\t");
					}

					str.Append(Cell(row, col).Domain.ToString());
				}

				str.Append("\n");
			}

			return str.ToString();
		}

		public int RowCount {
			get {
				return m_RowCount;
			}
		}

		public int ColCount {
			get {
				return m_ColCount;
			}
		}

		public FltVarList VarList {
			get {
				return m_VarList;
			}
		}

		public FltVar this[int row, int col] {
			get {
				return Cell(row, col);
			}
		}

		public FltVar Cell(int row, int col) {
			return m_VarList[row * m_ColCount + col];
		}

		public FltVarMatrix Matrix(int rowOffset, int colOffset, int rowCount, int colCount) {
			FltVarList list = new FltVarList(Solver);

			for(int row = 0; row < rowCount; ++row) {
				for(int col = 0; col < colCount; ++col) {
					list.Add(Cell(rowOffset + row, colOffset + col));
				}
			}

			return new FltVarMatrix(Solver, rowCount, colCount, list);
		}

		public FltVarList DiagLeftTopToBottomRight() {
			FltVarList list = new FltVarList(Solver);

			if(m_RowCount == m_ColCount) {
				int size = m_RowCount;

				for(int idx = 0; idx < size; ++idx) {
					list.Add(Cell(idx, idx));
				}
			}

			return list;
		}

		public FltVarList DiagRightTopToBottomLeft() {
			FltVarList list = new FltVarList(Solver);

			if(m_RowCount == m_ColCount) {
				int size = m_RowCount;

				for(int idx = 0; idx < size; ++idx) {
					list.Add(Cell(idx, (size - 1) - idx));
				}
			}

			return list;
		}

		public FltVarList Row(int vrow) {
			FltVarList list = new FltVarList(Solver);

			for(int col = 0; col < m_ColCount; ++col) {
				list.Add(Cell(vrow, col));
			}

			return list;
		}

		public FltVarList Col(int vcol) {
			FltVarList list = new FltVarList(Solver);

			for(int row = 0; row < m_RowCount; ++row) {
				list.Add(Cell(row, vcol));
			}

			return list;
		}


		private void InitMatrix(FltDomain domain) {
			m_VarList = new FltVarList(Solver, m_RowCount * m_ColCount);

			for(int row = 0; row < m_RowCount; ++row) {
				for(int col = 0; col < m_ColCount; ++col) {
					string name = row.ToString() + "." + col.ToString();

					FltVar cell = new FltVar(Solver, domain, name);

					m_VarList.Add(cell);
				}
			}
		}

		FltVarList m_VarList;
		int m_RowCount;
		int m_ColCount;
	}
}

//--------------------------------------------------------------------------------
