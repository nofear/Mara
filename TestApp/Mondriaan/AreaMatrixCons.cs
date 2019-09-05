//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace TestApp
{
    public class AreaMatrixCons : ConstraintVarList1<IntVar, IntVarList>
    {
        private Mondriaan m_Mondriaan;

        public AreaMatrixCons(Mondriaan m) :
            base(m.Area, m.Matrix.VarList)
        {
            m_Mondriaan = m;
        }

        public override void Post()
        {
            Post(Variable.How.OnDomain);
        }

        public override void Update()
        {
			IntDomain d = IntDomain.Empty;
			bool allBound = true;
			foreach(IntVar v in VarList) {
				allBound &= v.IsBound();
				if(!allBound) {
					break;
				}
				d = d.Union(v.Value);
			}

			int minArea = int.MaxValue;
			int maxArea = 0;
			foreach(IntInterval intv in d) {
				for(int i = intv.Min; i <= intv.Max; ++i) {
					int area = m_Mondriaan.RectangleList[i].Area;

					minArea = Math.Min(minArea, area);
					maxArea = Math.Max(maxArea, area);
				}
			}

			if(allBound) {
				Solver.IntObjective.Value = maxArea - minArea;

				Var0.Intersect(new IntInterval(minArea, maxArea));
			}

			int minAreaMax = maxArea - Solver.IntObjective.Value;

			IntDomain validRects = IntDomain.Empty;
			for(int idx = 0; idx < m_Mondriaan.RectangleList.Count; ++idx) {
				int area = m_Mondriaan.RectangleList[idx].Area;
				if(area >= minAreaMax) {
					validRects = validRects.Union(idx);
				}
			}

			foreach(IntVar v in VarList) {
				v.Intersect(validRects);
			}

		}
    }
}
