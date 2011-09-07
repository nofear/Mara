//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVarList2.cs $
 * 
 * 43    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 42    29-02-08 19:40 Patrick
 * fixed Post()/Post(..)
 * 
 * 41    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 40    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 39    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 38    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 37    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 36    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 35    27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 34    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 33    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 32    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for ConstraintVarList2.
	/// </summary>
	[DebuggerDisplay("{Var0},{Var1},{VarList}")]
	public abstract class ConstraintVarList2<TVar0,TVar1,TVarList> : ConstraintVar
		where TVar0:Variable
		where TVar1:Variable
		where TVarList:VariableList
	{
		protected ConstraintVarList2( TVar0 var0, TVar1 var1, TVarList list ) :
			base( list.Solver, new Variable[] { var0, var1 }, new VariableList[] { list } )
		{
		}

		internal override void OnSet()
		{
			m_TypeVar0		= (TVar0) VariableList[ 0 ];
			m_TypeVar1		= (TVar1) VariableList[ 1 ];
			m_TypeVarList	= (TVarList) VariableListList[ 0 ];
		}

		public override string ToString( bool wd )
		{
			return "(" + Var0.ToString( wd ) + "," + Var1.ToString( wd ) + "," + VarList.ToString( wd ) + ")";
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

		public TVarList VarList
		{
			get
			{
				return m_TypeVarList;
			}
		}
	
		TVar0		m_TypeVar0;
		TVar1		m_TypeVar1;
		TVarList	m_TypeVarList;
	}
}
