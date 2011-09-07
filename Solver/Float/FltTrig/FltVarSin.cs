//--------------------------------------------------------------------------------
// Copyright © 2004-2008 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltTrig/FltVarSin.cs $
 * 
 * 3     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 2     6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 1     9-03-08 17:54 Patrick
 * added to source control
 */
//--------------------------------------------------------------------------------

using System;
using MaraSolver.BaseConstraint;
using MaraSolver.Float;
using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	public class FltVarSin : ConstraintVar1<FltVar,FltVar>
	{
		public FltVarSin( FltVar var1 ) :
			base( new FltVar( var1.Solver ), var1 )
		{
			//QQQ: could make this more space efficient
			Var0.Name	= "Sin(" + var1.ToString() + ")";
		}

		public override string ToString()
		{
			return Var0.ToString();
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !( Var0.Value == Math.Sin( Var1.Value ) ) );
		}

		public override void Update()
		{
			Var0.Intersect( -Var1.Domain );
			Var1.Intersect( -Var0.Domain );
		}
	}
}

//--------------------------------------------------------------------------------
