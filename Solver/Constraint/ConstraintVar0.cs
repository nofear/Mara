//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/ConstraintVar0.cs $
 * 
 * 40    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 39    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 38    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 37    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 36    7-08-07 15:30 Patrick
 * moved add code to own method
 * 
 * 35    9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 34    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 33    27-06-07 23:05 Patrick
 * refactored registration part
 * 
 * 32    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 31    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 30    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	/// <summary>
	/// Summary description for IntVarCons.
	/// </summary>
	public abstract class ConstraintVar0<TVar0> : ConstraintVar
		where TVar0:Variable
	{
		protected ConstraintVar0( TVar0 var0 ) :
			base( var0.Solver, new Variable[] { var0 }, new VariableList[ 0 ] )
		{
		}

		internal override void OnSet()
		{
			m_TypeVar0		= (TVar0) VariableList[ 0 ];
		}
		
		public override string ToString( bool wd )
		{
			return "(" + Var0.ToString( wd ) + ")";
		}

		public TVar0 Var0
		{
			get
			{
				return m_TypeVar0;
			}
		}

		public override void Post()
		{
			Update();
			
			//Post( Variable.How.OnDomain );
		}

		protected override void Post( Variable.How how )
		{
			Update();

			//m_TypeVar0[ how ].Add( this );
		}
	
		TVar0	m_TypeVar0;
	}
}
