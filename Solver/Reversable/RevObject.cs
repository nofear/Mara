//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Reversable/RevObject.cs $
 * 
 * 13    10-11-07 1:27 Patrick
 * change ObjectRead/Change
 * 
 * 12    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 11    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 10    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 9     20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 8     19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 7     6/14/06 10:21p Patrick
 * added IStateStack interface
 * 
 * 6     14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 5     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 4     25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 3     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 2     12/14/05 10:06p Patrick
 * refactored using interface and delegates
 * 
 * 1     6-12-05 21:23 Patrick
 * added
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	/// <summary>
	/// Generic class for reversable reference objects.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("{m_Object}")]
	public class RevObject<T> : RevBase 
		where T:ICloneable
	{
		public RevObject( IStateStack stateStack ) :
			base( stateStack )
		{
		}

		public RevObject( IStateStack stateStack, T obj ) :
			base( stateStack )
		{
			m_Object	= obj;
		}

		public override object State
		{
			get
			{
				return m_Object;
			}

			set
			{
				m_Object = (T) value;
			}
		}

		public T Value
		{
			get
			{
				return m_Object;
			}

			set
			{
				if( !ReferenceEquals( m_Object, value ) )
				{
					Store();
				
					m_Object	= value;
				}
			}
		}

		T m_Object;
	}
}

//--------------------------------------------------------------------------------
