//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/Solver.cs $
 * 
 * 32    2/22/09 2:42p Patrick
 * 
 * 31    2/10/09 10:00p Patrick
 * simplified code
 * 
 * 30    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 29    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 28    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 27    12-01-08 13:17 Patrick
 * removed recursive adding of constraints
 * 
 * 26    11-01-08 22:43 Patrick
 * renamed folders
 * 
 * 25    17-11-07 2:23 Patrick
 * 
 * 24    25-10-07 21:56 Patrick
 * added Close()
 * copy objective variable
 * 
 * 23    20-10-07 0:56 Patrick
 * added 2nd version of threaded propagation queue
 * 
 * 22    19-10-07 0:35 Patrick
 * added demonlist
 * 
 * 21    18-10-07 23:20 Patrick
 * added constraint threaded
 * 
 * 20    16-10-07 0:43 Patrick
 * added threaded propagation queue
 * 
 * 19    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 18    9-08-07 0:33 Patrick
 * added ConstraintVar class
 * 
 * 17    8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 16    8-08-07 2:18 Patrick
 * added solution property
 * 
 * 15    7-08-07 15:31 Patrick
 * added Add() method
 * 
 * 14    31-07-07 21:35 Patrick
 * renamed
 * 
 * 13    29-07-07 22:11 Patrick
 * 
 * 12    25-07-07 23:01 Patrick
 * added Clone() to IStateStack
 * 
 * 11    25-07-07 22:59 Patrick
 * added Clone() for IPropagationQueue
 * 
 * 10    12-07-07 21:34 Patrick
 * removed registration of var-lists
 * 
 * 9     9-07-07 22:02 Patrick
 * removed PostInt()
 * 
 * 8     7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 7     5-07-07 1:38 Patrick
 * new deep copy mechanism
 */
//--------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaraSolver.Reversible;
using MaraInterval.Interval;
using MaraSolver.Float;
using MaraSolver.Integer;
using MaraSolver.BaseConstraint;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public partial class Solver
	{
		public Solver() :
			this( IntInterval.Whole )
		{
		}
	
		public Solver( int min, int max ) :
			this( new IntInterval( min, max ) )
		{
		}

		public Solver( IntInterval horizon )
		{
			m_PropagationQueue	= new PropagationQueueVar();
			m_IntObjective		= new IntObjective( this );	

			Init( horizon );
		}

		private void Init( IntInterval horizon )
		{
			m_Horizon			= horizon;

			m_VarList			= new List<Variable>();
			m_IntVarList		= new List<IntVar>();
			m_FltVarList		= new List<FltVar>();
			m_ConstraintList	= new List<Constraint>();

			m_StateStack		= new StateStack();
			m_GoalStack			= new GoalStack( m_IntObjective );

			m_Out				= Console.Out;
			m_Time				= DateTime.Now;
		}

		public IntInterval Horizon
		{
			get
			{
				return m_Horizon;
			}

			set
			{
				m_Horizon	= value;
			}
		}
	
		public List<IntVar> IntVarList
		{
			get
			{
				return m_IntVarList;
			}
		}

		public List<FltVar> FltVarList
		{
			get
			{
				return m_FltVarList;
			}
		}

		public List<Variable> VarList
		{
			get
			{
				return m_VarList;
			}
		}

		public List<Constraint> ConstraintList
		{
			get
			{
				return m_ConstraintList;
			}
		}

		public int CountViolated
		{
			get
			{
				int count	= 0;

				foreach( ConstraintVar cons in m_ConstraintList )
				{
					if( cons.IsViolated() )
					{
						++count;
					}
				}
				
				return count;
			}
		}

		public TextWriter Out
		{
			get
			{
				return m_Out;
			}
			
			set
			{
				m_Out	= value;
			}
		}
		
		public GoalStack GoalStack
		{
			get
			{
				return m_GoalStack;
			}
		}

		public IntObjective IntObjective
		{
			get
			{
				return m_IntObjective;
			}
		}

		public TimeSpan Time
		{
			get
			{
				return DateTime.Now - m_Time;
			}
		}

		public IPropagationQueue PropagationQueue
		{
			get
			{
				return m_PropagationQueue;
			}
		}

		public IStateStack StateStack
		{
			get
			{
				return m_StateStack;
			}
		}

		public Solution Solution
		{
			get
			{
				Solution solution	= new Solution();

				IntDomain[] intArray	= new IntDomain[ m_IntVarList.Count ];
				for( int idx = 0; idx < m_IntVarList.Count; ++idx )
				{
					intArray[ idx ]		= m_IntVarList[ idx ].Domain;
				}

				FltDomain[] fltArray	= new FltDomain[ m_FltVarList.Count ];
				for( int idx = 0; idx < m_FltVarList.Count; ++idx )
				{
					fltArray[ idx ]		= m_FltVarList[ idx ].Domain;
				}

				solution.IntDomainList	= intArray;
				solution.FltDomainList	= fltArray;

				return solution;
			}
		
			set
			{
				if( m_IntVarList.Count != value.IntDomainList.Length )
					return;
				
				if( m_FltVarList.Count != value.FltDomainList.Length )
					return;

				for( int idx = 0; idx < value.IntDomainList.Length; ++idx )
				{
					m_IntVarList[ idx ].Domain	= value.IntDomainList[ idx ];
				}

				for( int idx = 0; idx < value.FltDomainList.Length; ++idx )
				{
					m_FltVarList[ idx ].Domain	= value.FltDomainList[ idx ];
				}
			}
		}

		public bool Solve( Goal goal )
		{
			return m_GoalStack.Solve( goal );
		}
		
		public bool Next()
		{
			return m_GoalStack.Next();
		}
		
		public void Propagate()
		{
			m_PropagationQueue.IsViolated	= false;

			foreach( ConstraintVar constraint in m_ConstraintList )
			{
				constraint.Update();
			}
		}

		internal void OnDomainChange( Variable var )
		{
			m_PropagationQueue.OnChangeVariable( var );
		}

		public void Add( ConstraintVar constraint )
		{
			if( constraint.Index == -1 )
			{
				constraint.Index	= m_ConstraintList.Count;
			
				m_ConstraintList.Add( constraint );

				constraint.Add();
				constraint.Post();
				constraint.Update();
			}
		}

		/// <summary>
		/// Displays summary information about most recent solve.
		/// </summary>
		public void PrintInformation()
		{
			Out.WriteLine( "Number of fails             : " + m_GoalStack.FailCount.ToString() );
			Out.WriteLine( "Number of choice points     : " + m_GoalStack.StackOrCount.ToString() );
			Out.WriteLine( "Size of Or stack            : " + m_GoalStack.StackOrMax.ToString() );
			Out.WriteLine( "Size of And stack           : " + m_GoalStack.StackAndMax.ToString() );
			Out.WriteLine( "Number of integer variables : " + m_IntVarList.Count.ToString() );
			Out.WriteLine( "Number of float variables   : " + m_FltVarList.Count.ToString() );
			Out.WriteLine( "Number of constraints       : " + m_ConstraintList.Count.ToString() );
			Out.WriteLine( "Time elapsed since creation : " + Time.ToString() );
		}

		public void PrintVariables()
		{
			PrintVariables( Out );
		}

		/// <summary>
		/// Displays all variables
		/// </summary>
		public void PrintVariables( TextWriter tw )
		{
			tw.WriteLine( "Name\tDomain\tConstraints" );

			foreach( IntVar var in m_IntVarList )
			{
				tw.WriteLine( var.ToString( false ) + "\t" + var.Domain.ToString() + "\t" + var.ConstraintList.Count.ToString() );
			}

			foreach( FltVar var in m_FltVarList )
			{
				tw.WriteLine( var.ToString( false ) + "\t" + var.Domain.ToString() + "\t" + var.ConstraintList.Count.ToString() );
			}
		}

		public void PrintConstraints()
		{
			PrintConstraints( Out );
		}

		/// <summary>
		/// Displays all constraints
		/// </summary>
		public void PrintConstraints( TextWriter tw )
		{
			tw.WriteLine( "Update\tFail\tName" );

			foreach( Constraint constraint in ConstraintList )
			{
				tw.WriteLine( constraint.CountUpdate.ToString() + "\t"+ constraint.CountFail.ToString() + "\t" + constraint.ToString() );
			}
		}

		public void WriteDot()
		{
			FileStream file	= new FileStream( "test.dot", FileMode.Create );
			StreamWriter sw	= new StreamWriter( file );
						
			sw.WriteLine( "digraph" );
			sw.WriteLine( "{" );

			foreach( Variable var in m_VarList )
			{
				sw.WriteLine( "\"" + var.ToString(false) + "\"" );
			}

			foreach( Constraint cons in m_ConstraintList )
			{
				sw.WriteLine( "\"" + cons.ToString() + "\"" );
			}

			foreach( Constraint cons in m_ConstraintList )
			{
				ConstraintVar consVar	= cons as ConstraintVar;
				if( !ReferenceEquals( consVar, null ) )
				{
					foreach( Variable var in consVar.VariableList )
					{
						sw.WriteLine( "\"" + cons.ToString() + "\" -> \"" + var.ToString(false) + "\"");
					}
				}
			}

			sw.WriteLine( "}" );
			sw.Close();
			file.Close();
		}
		
		DateTime				m_Time;
		TextWriter				m_Out;
		IntInterval				m_Horizon;

		/// <link>aggregation</link>
		GoalStack				m_GoalStack;

		/// <link>aggregation</link>
		IPropagationQueue		m_PropagationQueue;

		/// <link>aggregation</link>
		IStateStack				m_StateStack;

		IntObjective			m_IntObjective;

		List<Variable>			m_VarList;
		List<IntVar>			m_IntVarList;
		List<FltVar>			m_FltVarList;
		
		List<Constraint>		m_ConstraintList;
	}

}

//--------------------------------------------------------------------------------
