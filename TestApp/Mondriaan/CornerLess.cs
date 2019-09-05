//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace TestApp {
	public class CornerLess: IntVarCmp {
		private Mondriaan m_Mondriaan;

		public CornerLess(Mondriaan m, int row0, int col0, int row1, int col1) :
			base(m.Matrix.Cell(row0, col0), m.Matrix.Cell(row1, col1)) {
			m_Mondriaan = m;
		}

		public override void Post() {
			Post(Variable.How.OnDomain);
		}

		public override void Update() {
			if(ReferenceEquals(Var0.Domain, Var0.DomainPrev)) {
				return;
			}

			int maxArea = 0;
			foreach(int i in Var0) {
				maxArea = Math.Max(maxArea, m_Mondriaan.RectangleList[i].Area);
			}

			foreach(int i in Var1) {
				if(m_Mondriaan.RectangleList[i].Area > maxArea) {
					Var1.Difference(i);
				}
			}
		}
	}
}
