using System;
using System.Collections.Generic;
using System.Text;

using MaraSolver.BaseConstraint;

namespace MaraSolver.Float
{
	public class FltVarPow : ConstraintVar1<FltVar,FltVar>
	{
		public FltVarPow( FltVar var1, int power ) :
			base( new FltVar( var1.Solver ), var1 )
		{
			//QQQ: could make this more space efficient
			Var0.Name	= var1.ToString(false) +"^" + power.ToString();
		
			m_Power		= power;
		}

		public override string ToString()
		{
			return Var0.ToString();
		}

		public override bool IsViolated()
		{
			return false;
		}

		public override void Update()
		{
			Var0.Intersect( Var1.Domain.Pow( m_Power ) );
			//Var1.Intersect( Var0.Domain.Pow( 1.0 / m_Power ) );
		}

		int m_Power;
	}
}
