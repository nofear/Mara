//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Solver/Solution.cs $
 * 
 * 5     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 4     8-08-07 2:18 Patrick
 * added int/float domain lis
 * 
 * 3     20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 2     26-03-07 14:06 Patrick
 * 
 * 1     26-03-07 13:08 Patrick
 * added Solution
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using MaraInterval.Interval;

//--------------------------------------------------------------------------------
namespace MaraSolver
{
	public class Solution
	{
		public Solution()
		{
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as Solution );
		}

		public bool Equals( Solution other )
		{
			if( ReferenceEquals( other, null ) )
				return false;
		
			if( m_IntDomainList != other.m_IntDomainList )
				return false;

			if( m_FltDomainList != other.m_FltDomainList )
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator==( Solution lhs, Solution rhs )
		{
			return lhs.Equals( rhs );
		}

		public static bool operator!=( Solution lhs, Solution rhs )
		{
			return !lhs.Equals( rhs );
		}
		
		public IntDomain[] IntDomainList
		{
			get
			{
				return m_IntDomainList;
			}
			
			set
			{
				m_IntDomainList		= value;
			}
		}

		public FltDomain[] FltDomainList
		{
			get
			{
				return m_FltDomainList;
			}

			set
			{
				m_FltDomainList		= value;
			}
		}
	
		IntDomain[]		m_IntDomainList;
		FltDomain[]		m_FltDomainList;
	}
}

//--------------------------------------------------------------------------------
