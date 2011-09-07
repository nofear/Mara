//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/GoalDelegate.cs $
 * 
 * 1     10-11-07 1:28 Patrick
 * added class
 */ 
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public delegate void ExecuteDelegate();

	public class GoalDelegate : Goal
	{
		public GoalDelegate( Solver solver, ExecuteDelegate execute ) :
			base( solver )
		{
			m_Execute	= execute;
		}

		public override void Execute()
		{
			if( !ReferenceEquals( m_Execute, null ) )
			{
				m_Execute();
			}
		}
		
		ExecuteDelegate m_Execute;
	};

}

//--------------------------------------------------------------------------------
