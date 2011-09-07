//--------------------------------------------------------------------------------
// Copyright © 2004-2008 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCons/FltVarLog.cs $
 * 
 * 4     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 3     6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 2     14-01-08 20:07 Patrick
 * fixed name
 * 
 * 1     14-01-08 19:55 Patrick
 * added log
 */
//--------------------------------------------------------------------------------

using System;

using MaraSolver.BaseConstraint;
using MaraSolver.Float;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	public class FltVarLog : ConstraintVar1<FltVar,FltVar>
	{
		public FltVarLog( FltVar var1 ) :
			base( new FltVar( var1.Solver ), var1 )
		{
			//QQQ: could make this more space efficient
			Var0.Name	= "Log(" + var1.ToString(false) + ")";
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
			Var0.Intersect( Var1.Domain.Interval.Log() );
			Var1.Intersect( Var0.Domain.Interval.Exp() );
		}
	}
}

//--------------------------------------------------------------------------------
