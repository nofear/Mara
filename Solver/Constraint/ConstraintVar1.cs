//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVar1.cs $
 * 
 * 44    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 43    29-02-08 19:40 Patrick
 * fixed Post()/Post(..)
 * 
 * 42    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 41    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 40    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 39    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 38    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 37    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 36    27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 35    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 34    20-06-07 22:46 Patrick
 * renamed namespace
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for IntVarCons1.
	/// </summary>
	public abstract class ConstraintVar1<TVar0,TVar1> : ConstraintVar
		where TVar0:Variable
		where TVar1:Variable
	{
		protected ConstraintVar1( TVar0 var0, TVar1 var1 ) :
			base( var0.Solver, new Variable[] { var0, var1 }, new VariableList[ 0 ] )
		{
		}

		internal override void OnSet()
		{
			m_TypeVar0		= (TVar0) VariableList[ 0 ];
			m_TypeVar1		= (TVar1) VariableList[ 1 ];
		}

		public override string ToString( bool wd )
		{
			return "(" + Var0.ToString( wd ) + "," + Var1.ToString( wd ) + ")";
		}

		public TVar0 Var0
		{
			get
			{
				return m_TypeVar0;
			}
		}

		public TVar1 Var1
		{
			get
			{
				return m_TypeVar1;
			}
		}

		TVar0		m_TypeVar0;
		TVar1		m_TypeVar1;
	}
}

