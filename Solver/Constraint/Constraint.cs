//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/Constraint.cs $
 * 
 * 9     2/22/09 2:42p Patrick
 * 
 * 8     2/10/09 10:00p Patrick
 * simplified code
 * 
 * 7     6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 6     27-02-08 23:25 Patrick
 * added default to Post(..) mechanism
 * support OnInterval/OnDomain changes
 * 
 * 5     20-10-07 0:56 Patrick
 * added 2nd version of threaded propagation queue
 * 
 * 4     16-10-07 0:43 Patrick
 * added threaded propagation queue
 * 
 * 3     11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 2     9-08-07 1:04 Patrick
 * 
 * 1     9-08-07 0:33 Patrick
 * added ConstraintVar class
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

using MaraSolver;
using MaraSolver.Reversible;

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	public enum PropagateLevel
	{
		Low,
		Normal,
		High
	};

	/// <summary>
	/// Summary description for Constraint.
	/// </summary>
	public abstract class Constraint : Goal, IDemon
	{
		protected Constraint( Solver solver ) :
			base( solver )
		{
			m_Level				= PropagateLevel.Normal;
			m_CountFail			= 0;
			m_CountUpdate		= 0;
			m_Index				= -1;
		}

		public override void Execute()
		{
			m_Solver.PropagationQueue.IsViolated	= false;
	
			Update();
			
			m_Solver.PropagationQueue.Propagate();

			if( m_Solver.PropagationQueue.IsViolated )
			{
				base.Fail();
			}
		}

		public PropagateLevel Level
		{
			get
			{
				return m_Level;
			}

			set
			{
				m_Level		= value;
			}
		}

		public int Index
		{
			get
			{
				return m_Index;
			}
			
			set
			{
				m_Index		= value;
			}
		}

		public string DefaultName
		{
			get
			{
				return "c" + Index.ToString( CultureInfo.CurrentCulture );
			}
		}

		public abstract string ToString( bool wd );

		public override string ToString()
		{
			return ToString( false );
		}

		public int CountFail
		{
			get
			{
				return m_CountFail;
			}
			
			set
			{
				m_CountFail		= value;
			}
		}

		public int CountUpdate
		{
			get
			{
				return m_CountUpdate;
			}

			set
			{
				m_CountUpdate	= value;
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

		// Called to hook into the variable callback mechanism
		public abstract void Post();
		
		// Called to add composite constraints (e.g. compound expressions)
		public abstract void Add();

		public void Violate()
		{
			m_Solver.PropagationQueue.IsViolated	= true;
		}
		
		// Returns true if the constraint is violated.
		public virtual bool IsViolated()
		{
			return false;
		}

		// Called to unbound all the variables related to this constraint
		public virtual void Unbound()
		{
		}
		
		// Returns true if all the variables related to this constraint are bound.
		public virtual bool IsBound()
		{
			return false;
		}

		public virtual void Update( Variable variable )
		{
			Update();
		}

		public virtual void Update()
		{
		}

		PropagateLevel	m_Level;
	
		bool			m_InQueue;

		/// <summary>
		/// statistic: number of fails during search.
		/// </summary>
		int				m_CountFail;
		
		/// <summary>
		/// statistic: number of calls to Update(..)
		/// </summary>
		int				m_CountUpdate;

		int				m_Index;
	}
}
