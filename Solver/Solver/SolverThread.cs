//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Solver/Solver.cs $
 * 
 * 58    26-03-07 14:31 Patrick
 * no need to manually call Problem.Post()
 * 
 * 57    21-03-07 23:23 Patrick
 * implemented cloning of Problem/Constraint/Variable
 * 
 * 56    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 55    7-03-07 18:52 Patrick
 * removed problem -> Solver reference
 * 
 * 54    7-03-07 18:26 Patrick
 * moved propagation queue from Solver to Problem
 * 
 * 53    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 52    2-03-07 20:30 Patrick
 * removed dependency from StateStack
 * 
 * 51    27-02-07 1:35 Patrick
 * renamed property
 * refactored PostRecursive
 * 
 * 50    22-02-07 0:18 Patrick
 * added WriteDot()
 * 
 * 49    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 48    20-02-07 20:59 Patrick
 * refactored var domain a bit
 * 
 * 47    19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 46    1-02-07 22:54 Patrick
 * added change flag to variable
 * 
 * 45    5-11-06 13:23 Patrick
 * added Propagate()
 * 
 * 44    11-06-06 22:39 Patrick
 * added Time property
 * 
 * 43    5-06-06 21:49 Patrick
 * made accessor GoalStack public
 * 
 * 42    6/02/06 9:18a Patrick
 * 
 * 41    6/01/06 1:09p Patrick
 * added 'withDomain' parameter to ToString()
 * 
 * 40    6/01/06 11:11a Patrick
 * cleanup comments
 * 
 * 39    5/29/06 9:37p Patrick
 * enable posting of related constraints
 * 
 * 38    23-03-06 23:12 Patrick
 * implemented objective
 * 
 * 37    22-03-06 23:06 Patrick
 * added IntObjective
 * 
 * 36    14-03-06 22:19 Patrick
 * added constraint namespace
 * 
 * 35    14-03-06 22:07 Patrick
 * added integer & float namespace
 * 
 * 34    14-03-06 21:51 Patrick
 * put classes into proper namespace
 * 
 * 33    14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 32    22-02-06 22:38 Patrick
 * renamed FSolver to Solver
 * 
 * 31    22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 30    16-02-06 22:48 Patrick
 * seperated int/flt variables in Solver
 * 
 * 29    25-01-06 22:02 Patrick
 * renamed method
 * 
 * 28    25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 27    8-01-06 18:13 Patrick
 * made goalstack internal
 * renamed SolverQueue
 * 
 * 26    8-01-06 15:16 Patrick
 * adjusted naming
 * 
 * 25    8-01-06 15:00 Patrick
 * some more renaming
 * 
 * 24    8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 23    7-01-06 22:17 Patrick
 * 
 * 22    5-01-06 22:49 Patrick
 * refactored to Post() & reversible constraint lists
 * 
 * 21    3-01-06 23:16 Patrick
 * refactored constrait hierarchy
 * 
 * 20    31-12-05 14:49 Patrick
 * refactored print info
 * 
 * 19    20-10-05 21:49 Patrick
 * added comment
 * 
 * 18    20-10-05 21:26 Patrick
 * removed sentinel values from horizon
 * 
 * 17    10/19/05 10:16p Patrick
 * made class internal
 * 
 * 16    7/01/05 9:58p Patrick
 * made horizion an interval
 * 
 * 15    30-05-05 14:52 Patrick
 * added CountViolated
 * 
 * 14    28-05-05 19:49 Patrick
 * upgrade to visual studio 2005
 * added generics where available
 * 
 * 13    26-05-05 23:01 Patrick
 * 
 * 12    26-05-05 20:21 Patrick
 * renamed Add(..) => Union(..)
 * 
 * 11    26-05-05 19:58 Patrick
 * renamed PELib -> Solver
 * 
 * 10    5/26/05 7:25p Patrick
 * added improved stats
 * using Stack()
 * 
 * 9     24-05-05 21:59 Patrick
 * print information
 * 
 * 8     24-05-05 20:59 Patrick
 * renamed classes
 * 
 * 7     19-05-05 23:16 Patrick
 * moved code to base class, renamed member variable
 */
//--------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using ConstraintSolver.Reversible;
using ConstraintSolver.Interval;
using ConstraintSolver.Float;
using ConstraintSolver.Integer;
using ConstraintSolver.BaseConstraint;

//--------------------------------------------------------------------------------
namespace ConstraintSolver
{
	/// <summary>
	/// Summary description for Solver.
	/// </summary>
	public sealed class Solver
	{
		public Solver( Problem problem )
		{
			m_IntObjective		= new IntObjective( this );	
			m_GoalStack			= new GoalStack( m_IntObjective );
						
			m_Out				= Console.Out;
			
			m_Time				= DateTime.Now;

			m_Problem			= problem;

			m_Problem.Propagate();
		}

		public Problem Problem
		{
			get
			{
				return m_Problem;
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

		public void Propagate()
		{
			m_Problem.PropagationQueue.Propagate();
		}
				
		public bool Solve( Goal goal )
		{
			return m_GoalStack.Solve( goal );
		}
		
		public bool NextSolution()
		{
			return m_GoalStack.NextSolution();
		}

		public void PrintConstraints()
		{
			m_Problem.PrintConstraints( Out );
		}
		
		public void PrintVariables()
		{
			m_Problem.PrintVariables( Out );
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
			Out.WriteLine( "Number of integer variables : " + m_Problem.IntVarList.Count.ToString() );
			Out.WriteLine( "Number of float variables   : " + m_Problem.FltVarList.Count.ToString() );
			Out.WriteLine( "Number of constraints       : " + m_Problem.ConstraintList.Count.ToString() );
			Out.WriteLine( "Time elapsed since creation : " + Time.ToString() );
		}

		Problem						m_Problem;

		/// <link>aggregation</link>
		GoalStack					m_GoalStack;
		
		DateTime					m_Time;
		
		TextWriter					m_Out;
		
		IntObjective				m_IntObjective;
	}
}

//--------------------------------------------------------------------------------
