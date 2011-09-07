//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Reversable/RevValue.cs $
 * 
 * 16    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 15    9-03-07 23:07 Patrick
 * updated copyright notice
 * 
 * 14    3-03-07 18:53 Patrick
 * separated problem/solver
 * 
 * 13    24-02-07 0:25 Patrick
 * added ToString()
 * 
 * 12    20-02-07 23:47 Patrick
 * removed IntVarDomain, FltVarDomain
 * 
 * 11    19-02-07 22:26 Patrick
 * fixed typo in namespace naming
 * 
 * 10    6/14/06 10:21p Patrick
 * added IStateStack interface
 * 
 * 9     14-03-06 21:38 Patrick
 * put things in namespace
 * 
 * 8     22-02-06 22:37 Patrick
 * renamed name space
 * 
 * 7     25-01-06 21:44 Patrick
 * Refactored Reversable to only take a StateStack
 * 
 * 6     8-01-06 14:54 Patrick
 * removed prefix from class names
 * 
 * 5     12/14/05 10:06p Patrick
 * refactored using interface and delegates
 * 
 * 4     6-12-05 21:01 Patrick
 * 
 * 3     11/29/05 9:48p Patrick
 * check if value has changed
 * 
 * 2     10/19/05 9:18p Patrick
 * added comments
 * 
 * 1     28-05-05 19:49 Patrick
 * upgrade to visual studio 2005
 * added generics where available
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Globalization;
using System.Diagnostics;

//--------------------------------------------------------------------------------
namespace MaraSolver.Reversible
{
	/// <summary>
	/// Generic class for simple reversable value types.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("{m_Value}")]
	public class RevValue<T> : RevBase
	{
		public RevValue( IStateStack stateStack ) :
			base( stateStack )
		{
		}

		public RevValue( IStateStack stateStack, T value ) :
			base( stateStack )
		{
			m_Value		= value;
		}

		public override string ToString()
		{
			return m_Value.ToString();
		}

		public override object State
		{
			get
			{
				return m_Value;
			}

			set
			{
				m_Value = (T) value;
			}
		}

		public T Value
		{
			get
			{
				return m_Value;
			}

			set
			{
				if( !m_Value.Equals( value ) )
				{
					Store();

					m_Value = value;
				}
			}
		}

		T m_Value;
	}
}

//--------------------------------------------------------------------------------
