//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Constraint/IDemon.cs $
 * 
 * 8     2/10/09 10:00p Patrick
 * simplified code
 * 
 * 7     20-10-07 0:56 Patrick
 * added 2nd version of threaded propagation queue
 * 
 * 6     11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 5     8-08-07 22:28 Patrick
 * renamed IUpdate => IDemon
 * 
 * 4     8-08-07 22:27 Patrick
 * renamed IsChanged => InQueue
 * 
 * 3     8-08-07 21:53 Patrick
 * changed interface
 * 
 * 2     25-07-07 3:59 Patrick
 * added Violate()
 */
//--------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
namespace MaraSolver.BaseConstraint
{
	public interface IDemon
	{
		int CountUpdate
		{
			get;
			set;
		}

		int CountFail
		{
			get;
			set;
		}

		bool InQueue
		{
			get;
			set;
		}

		void Update( Variable variable );
		void Update();
	}
}

//--------------------------------------------------------------------------------
