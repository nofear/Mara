//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Variable/Variable.cs $
 * 
 * 75    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 74    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 73    27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 72    12-10-07 0:28 Patrick
 * moved compare class
 * 
 * 71    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 70    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 69    8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 68    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 67    8-08-07 3:07 Patrick
 * added Index to variable
 * 
 * 66    31-07-07 21:41 Patrick
 * 
 * 65    31-07-07 21:37 Patrick
 * simplified
 * 
 * 64    12-07-07 21:32 Patrick
 * using IUpdate
 * 
 * 63    7-07-07 15:36 Patrick
 * fixed another copy issue
 * 
 * 62    5-07-07 1:38 Patrick
 * new deep copy mechanism

 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using MaraSolver.BaseConstraint;
using MaraInterval.Interval;
using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Base class for a variable.
	/// </summary>
	public abstract class Variable : RevBase
	{
		public enum How
		{
			OnIsbound	= 0,
			OnInterval,
			OnDomain
		};

		protected Variable( Solver solver ) :
			this( solver, string.Empty )
		{
		}

		protected Variable( Solver solver, string name ) :
			base( solver.StateStack )
		{
			m_Solver			= solver;
			m_Name				= name;
			m_InQueue			= false;
			m_IsLead			= false;

			m_ConstraintList	= new RevList<ConstraintVar>( m_Solver.StateStack );

			m_OnVarDomainList	= new RevList<IDemon>( m_Solver.StateStack );
			m_OnVarIntervalList	= new RevList<IDemon>( m_Solver.StateStack );
			m_OnVarIsBoundList	= new RevList<IDemon>( m_Solver.StateStack );

			m_Solver.VarList.Add( this );
		}

		public abstract string ToString( bool wd );
		
		public abstract int Index
		{
			get;
		}
		
		public abstract string DefaultName
		{
			get;
		}

		public Solver Solver
		{
			get
			{
				return m_Solver;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}

			set
			{
				m_Name	= value;
			}
		}
		
		public bool IsLead
		{
			get
			{
				return m_IsLead;
			}
			
			set
			{
				m_IsLead	= value;
			}
		}

		public bool InQueue
		{
			get
			{
				return m_InQueue;
			}
			
			set
			{
				m_InQueue	= value;
			}
		}

		public IList<IDemon> this[ Variable.How how ]
		{
			get
			{
				switch( how )
				{
					case How.OnIsbound:
						return m_OnVarIsBoundList;

					case How.OnInterval:
						return m_OnVarIntervalList;

					case How.OnDomain:
						return m_OnVarDomainList;

					default:
						return null;
				}
			}
		}

		public IList<ConstraintVar> ConstraintList
		{
			get
			{
				return m_ConstraintList;
			}
		}

		public IList<IDemon> OnVarDomainList
		{
			get
			{
				return m_OnVarDomainList;
			}
		}

		public IList<IDemon> OnVarIntervalList
		{
			get
			{
				return m_OnVarIntervalList;
			}
		}

		// called when variable gets bound
		public IList<IDemon> OnVarIsBoundList
		{
			get
			{
				return m_OnVarIsBoundList;
			}
		}

		#region IDomainChange Members

		///<summary>
		/// This method is called when the domain of the variable changes.
		/// </summary>
		public void OnDomainChange()
		{
			m_Solver.OnDomainChange( this );
		}

		#endregion

		public virtual void Unbound()
		{
			if( !m_IsLead )
			{
				UnboundDirect();
			}
		}

		public virtual void UnboundDirect()
		{
			foreach( ConstraintVar cons in ConstraintList )
			{					
				cons.Unbound();
			}
		}

		abstract public bool IsIntervalChanged();
		abstract public bool IsBound();
		abstract public bool IsEmpty();

		#region Predicates
		
		public static bool IsBound( Variable var )
		{
			return var.IsBound();
		}

		public static bool IsEmpty( Variable var )
		{
			return var.IsEmpty();
		}

		#endregion

		public class ComparerCons : IComparer<Variable>
		{
			public int Compare( Variable x, Variable y ) 
			{
				int r	= ( x.ConstraintList.Count < y.ConstraintList.Count ) ? -1
						: ( x.ConstraintList.Count > y.ConstraintList.Count ) ? 1
						: 0;
						
				return r;
			}
		}
		
		protected Solver		m_Solver;

		string					m_Name;
		bool					m_IsLead;
		bool					m_InQueue;

		/// <summary>
		/// List of constraints related to this variable(list) base.
		/// </summary>
		RevList<ConstraintVar>		m_ConstraintList;

		RevList<IDemon>		m_OnVarIsBoundList;
		RevList<IDemon>		m_OnVarIntervalList;
		RevList<IDemon>		m_OnVarDomainList;
	}
}
