//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/SolverBase.cs $
 * 
 * 18    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 17    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 16    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 15    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 14    4-07-07 21:08 Patrick
 * added Init(..)
 * 
 * 13    4-07-07 20:09 Patrick
 * fixed typo
 * 
 * 12    4-07-07 20:09 Patrick
 * added Init()
 * 
 * 11    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 10    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 9     9-03-07 23:07 Patrick
 * updated copyright notice
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Base class for all solver related classes
	/// </summary>
	public abstract class SolverBase
	{
		public SolverBase( Solver solver )
		{
			m_Solver	= solver;
		}

		public Solver Solver
		{
			get
			{
				return m_Solver;
			}
		}

		protected Solver	m_Solver;
	}
}

//--------------------------------------------------------------------------------
