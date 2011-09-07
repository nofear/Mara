//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Solver/Solver/Utility/StackEx.cs $
 * 
 * 2     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 1     2-03-07 20:30 Patrick
 * original stack constructor sucks
 */
//--------------------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;

//--------------------------------------------------------------------------------
namespace MaraSolver.Utility
{
	public class StackEx<T> :
		Stack<T>
	{
		public StackEx() :
			base()
		{
		}

		public StackEx( int capacity ) :
			base( capacity )
		{
		}

		public StackEx( IEnumerable<T> collection )
		{
		}

		public StackEx( StackEx<T> collection ) :
			base( collection.Count )
		{
			T[] array	= collection.ToArray();
			
			for( int idx = array.Length; idx > 0; --idx )
			{
				Push( array[ idx - 1 ]  );
			}
		}
	}
}

//--------------------------------------------------------------------------------
