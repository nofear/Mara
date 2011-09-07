//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Solver/SolverBase.cs $
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
 * 
 * 8     1-02-07 23:13 Patrick
 * renamed Base -> SolverBase
 * 
 * 7     1-02-07 22:52 Patrick
 * 
 * 6     22-02-06 22:38 Patrick
 * renamed FSolver to Solver
 * 
 * 5     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 4     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 3     5-01-06 21:57 Patrick
 * changed access mode of data member
 * 
 * 2     10/19/05 10:37p Patrick
 * added comment
 * 
 * 1     26-05-05 20:27 Patrick
 * added FBase
 */
//--------------------------------------------------------------------------------

using System;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	/// <summary>
	/// Base class for all solver related classes
	/// </summary>
	public abstract class SolverBaseCopy : SolverBase, ISolverCopy
	{
		public SolverBaseCopy( Solver solver ) :
			base( solver )
		{
		}

		protected SolverBaseCopy() :
			base()
		{
		}

		#region ISolverCopy Members

		public abstract ISolverCopy New();

		public virtual void Init( SolverCopier copier, ISolverCopy other )
		{
			Init( copier.Solver );
		}

		#endregion
	}
}

//--------------------------------------------------------------------------------
