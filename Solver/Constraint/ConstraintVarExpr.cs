using System;
using System.Collections.Generic;
using System.Text;

namespace MaraSolver.BaseConstraint
{
	public abstract class ConstraintVarExpr : ConstraintVar
	{
		protected ConstraintVarExpr( Solver solver, Variable[] varList ) :
			base( solver, varList, new VariableList[ 0 ] )
		{
			m_ExprList	= new List<ConstraintVarExpr>();
		}

		public override void Add()
		{
			base.Add();

			foreach( ConstraintVarExpr expr in m_ExprList )
			{
				m_Solver.Add( expr );			
			}
		}

		public List<ConstraintVarExpr> ExprList
		{
			get
			{
				return m_ExprList;
			}
		}

		public void AddExpr( ConstraintVarExpr expr )
		{
			m_ExprList.Add( expr );
		}

		List<ConstraintVarExpr> m_ExprList;
	}

}
