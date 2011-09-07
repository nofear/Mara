//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltCmp/FltVarCmpEqualIntVar.cs $
 * 
 * 36    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 35    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 34    4-03-08 23:39 Patrick
 * use Epsilon compare
 * 
 * 33    29-02-08 22:23 Patrick
 * 
 * 32    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 31    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 30    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 29    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 28    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 27    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 26    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 25    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 24    10-03-07 0:46 Patrick
 * simplified base constraint Update() mechanism
 */
//--------------------------------------------------------------------------------

using System;
using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Implements the constraint var0(int) == var1(flt)
	/// </summary>
	public class FltVarCmpEqualIntVar : ConstraintVar1<IntVar,FltVar>
	{
		public FltVarCmpEqualIntVar( IntVar var0, FltVar var1 ) :
			base( var0, var1 )
		{
		}

		public IntVar IntVar
		{
			get
			{
				return base.Var0 as IntVar;
			}
		}

		public FltVar FltVar
		{
			get
			{
				return base.Var1 as FltVar;
			}
		}

		public override string ToString()
		{
			return IntVar.ToString() + "==" +FltVar.ToString();
		}

		public override bool IsViolated()
		{
			return ( IsBound()
						&& !Epsilon.Equal( IntVar.Value, FltVar.Value ) );
		}

		public override void Update()
		{
			UpdateVar0();
			UpdateVar1();
		}

		protected void UpdateVar0()
		{
			IntDomain domain	= new IntDomain();
			
			foreach( FltInterval flt_iset in FltVar.Domain )
			{
				domain	= domain.Union( (int) Math.Floor( flt_iset.Min ), (int) Math.Ceiling( flt_iset.Max ) );
			}
			
			IntVar.Intersect( domain );
		}

		protected void UpdateVar1()
		{
			FltDomain domain	= new FltDomain();
			
			foreach( IntInterval int_iset in IntVar.Domain )
			{
				domain	= domain.Union( int_iset.Min, int_iset.Max );
			}
			
			FltVar.Intersect( domain );
		}
	}
}
