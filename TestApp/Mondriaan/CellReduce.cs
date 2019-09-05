//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace TestApp
{
    public class CellReduce : ConstraintVar0<IntVar>
    {
        private Mondriaan m_Mondriaan;
        private int m_Row;
        private int m_Col;


        public CellReduce(int row, int col, Mondriaan m) :
            base(m.Matrix.Cell(row, col))
        {
            m_Mondriaan = m;
            m_Row = row;
            m_Col = col;
        }

        public override void Post()
        {
            Post(Variable.How.OnDomain);
        }

        public override void Update()
        {
            if (Var0.IsBound())
            {
                return;
            }

            IntVarMatrix matrix = m_Mondriaan.Matrix;

            int row0, row1;
            for (row0 = m_Row; row0 - 1 >= 0 && !matrix.Cell(row0 - 1, m_Col).IsBound(); row0--) { };
            for (row1 = m_Row; row1 + 1 < matrix.RowCount && !matrix.Cell(row1 + 1, m_Col).IsBound(); row1++) { };

            int col0, col1;
            for (col0 = m_Col; col0 - 1 >= 0 && !matrix.Cell(m_Row, col0 - 1).IsBound(); col0--) { };
            for (col1 = m_Col; col1 + 1 < matrix.ColCount && !matrix.Cell(m_Row, col0 + 1).IsBound(); col1++) { };

            int h = row1 - row0 + 1;
            int w = col1 - col0 + 1;

            int h0 = Math.Min(h, w);
            int w0 = Math.Max(h, w);

            IntDomain v = Var0.Domain;
            foreach (int i in Var0)
            {
                var t = m_Mondriaan.RectangleList[i];
                if (t.Width > w0 || t.Height > h0)
                {
                    v = v.Difference(i);
                }
            }

            Var0.Intersect(v);
        }
    }
}
