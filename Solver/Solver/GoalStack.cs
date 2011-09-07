//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/GoalStack.cs $
 * 
 * 51    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 50    8-11-07 2:05 Patrick
 * comment out experimental code
 * 
 * 49    25-10-07 21:55 Patrick
 * fixed issue with Stack..
 * 
 * 48    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 47    31-07-07 21:35 Patrick
 * renamed
 * 
 * 46    29-07-07 22:12 Patrick
 * 
 * 45    10-07-07 22:29 Patrick
 * removed Int/Flt Reduce Goals
 * 
 * 44    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 43    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 42    11-06-07 23:26 Patrick
 * added copying of goals
 * 
 * 41    26-03-07 15:09 Patrick
 * cleanup IntObjective()
 * 
 * 40    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 39    2-03-07 20:30 Patrick
 * removed dependency from StateStack
 * 
 * 38    23-02-07 2:04 Patrick
 * fixed issue with Stack<>
 * 
 * 37    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 36    19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 */
//--------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;

using MaraSolver.Reversible;
using MaraSolver.Utility;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Implements the execution and choice point stack.
	/// </summary>
	public sealed class GoalStack
	{
		public GoalStack( IntObjective intObjective )
		{
			m_StackAndState	= new StackEx<StackEx<Goal>>();
			m_StackAnd		= new StackEx<Goal>();
			m_StackOr		= new StackEx<GoalOr>();
			m_IntObjective	= intObjective;

			m_IsExecuting	= false;
			m_Stop			= false;

			m_StackAndMax	= 0;
			m_StackOrMax	= 0;
			m_StackOrCount	= 0;

			m_IsFailed		= false;
			m_CountFail		= 0;
		}

		public bool Stop
		{
			get
			{
				return m_Stop;
			}
	
			set
			{
				m_Stop	= value;
			}
		}
		
		public int FailCount
		{
			get
			{
				return m_CountFail;
			}
		}

		public int StackAndMax
		{
			get
			{
				return m_StackAndMax;
			}
		}

		public int StackOrMax
		{
			get
			{
				return m_StackOrMax;
			}
		}

		public int StackOrCount
		{
			get
			{
				return m_StackOrCount;
			}
		}

		public void Print( TextWriter tw )
		{
			PrintOrStack( tw );
			PrintAndStack( tw );

			tw.WriteLine( "" );
		}

		/// <summary>
		/// Prints the execution stack to the given TextWriter.
		/// </summary>
		/// <param name="tw">text writer to write to.</param>
		public void PrintAndStack( TextWriter tw )
		{
			tw.WriteLine( "AndStack:" );

			int index	= m_StackAnd.Count;
			foreach( Goal goal in m_StackAnd )
			{
				--index;
				tw.WriteLine( "#" + index.ToString() + " " + goal.ToString() );
			}
		}

		/// <summary>
		/// Prints the choice point stack to the given TextWriter
		/// </summary>
		/// <param name="tw">text writer to write to</param>
		public void PrintOrStack( TextWriter tw )
		{
			tw.WriteLine( "OrStack:" );

			int index	= m_StackOr.Count;
			foreach( GoalOr goal in m_StackOr )
			{
				--index;
				tw.WriteLine( "#" + index.ToString() + " " + goal.ToString() );
			}
		}

		public bool Solve( Goal goal )
		{
			Add( goal );
			Execute();
			
			m_IntObjective.Init();
			
			return !m_IsFailed;
		}

		public bool Next()
		{
			bool hasSolution	= m_StackOr.Count > 0;
			if( hasSolution )
			{
				m_IntObjective.Next();

				Fail();

				Execute();

				hasSolution		= !m_IsFailed;
			}
			
			return hasSolution;
		}

		public void Add( Goal goal )
		{
			m_StackAnd.Push( goal );
			m_StackAndMax	= Math.Max( m_StackAndMax, m_StackAnd.Count );

			GoalOr goalOr	= goal as GoalOr;

			if( !ReferenceEquals( goalOr, null ) )
			{
				m_StackOr.Push( goalOr );
				m_StackOrCount++;
				m_StackOrMax	= Math.Max( m_StackOrMax, m_StackOr.Count );
			}
		}

/*
		private void AddTest( Goal goal )
		{
			GoalOr goalOr	= goal as GoalOr;

			if( !ReferenceEquals( goalOr, null ) )
			{
				AddObjective();

				m_StackAnd.Push( goal );
				m_StackAndMax	= Math.Max( m_StackAndMax, m_StackAnd.Count );

				m_StackOr.Push( goalOr );
				m_StackOrCount++;
				m_StackOrMax	= Math.Max( m_StackOrMax, m_StackOr.Count );
			}
			else
			{
				m_StackAnd.Push( goal );
				m_StackAndMax	= Math.Max( m_StackAndMax, m_StackAnd.Count );
			}
		}
*/

		public void Fail()
		{
			++m_CountFail;

			m_IsFailed	= m_StackOr.Count == 0;
			if( m_IsFailed )
			{
				m_StackAnd.Clear();
			}
			else
			{
				GoalOr goal	= m_StackOr.Peek();

				goal.Pop();

				if( goal.IsDone() )
				{
					m_StackOr.Pop();

					Fail();
				}
				else
				{
					if( !ReferenceEquals( m_IntObjective.Var, null ) )
					{
						m_StackAnd.Push( m_IntObjective.Create() );
					}

					m_StackAnd.Push( goal );
				}
			}
		}

		/// <summary>
		/// Executes all goals on the 'and' stack, until it's empty.
		/// </summary>
		private void Execute()
		{
			if( !m_IsExecuting )
			{
				m_IsExecuting		= true;

				while( m_StackAnd.Count > 0 && !m_Stop )
				{
					Goal goal	= m_StackAnd.Pop();

					goal.Execute();
				}

				m_IsExecuting		= false;
			}
		}
	
		internal void Push()
		{
			m_StackAndState.Push( m_StackAnd );
			
			m_StackAnd	= new StackEx<Goal>( m_StackAnd );
		}

		internal void Pop()
		{
			m_StackAnd	= m_StackAndState.Pop();
		}

		bool					m_IsExecuting;
		bool					m_Stop;

		StackEx<StackEx<Goal>>	m_StackAndState;

		StackEx<Goal>			m_StackAnd;
		int						m_StackAndMax;

		StackEx<GoalOr>			m_StackOr;
		int						m_StackOrMax;
		int						m_StackOrCount;
		
		bool					m_IsFailed;
		int						m_CountFail;
		
		IntObjective			m_IntObjective;
	}
}
