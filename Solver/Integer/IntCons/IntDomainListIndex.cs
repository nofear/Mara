//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntCons/IntDomainListIndex.cs $
 * 
 * 5     1/19/09 11:48p Patrick
 * 
 * 4     9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 3     6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 2     17-11-07 18:00 Patrick
 * added IntDomainList
 * 
 * 1     17-11-07 2:22 Patrick
 * added domain version
 */
//--------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;

using MaraInterval.Interval;
using MaraSolver.BaseConstraint;
using MaraSolver.Integer;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Implements the constraint R = List[ Index ].
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntDomainListIndex : ConstraintVar1<IntVar,IntVar>
	{
		public IntDomainListIndex( Solver solver, IntDomainList list ) :
			this( new IntVar( solver ), new IntVar( solver ), list )
		{
		}

		public IntDomainListIndex( IntVar var0, IntVar index, IntDomainList list ) :
			base( var0, index )
		{
			m_DomainList	= list;
		}

		public override string ToString( bool wd )
		{
			StringBuilder str	= new StringBuilder();
			str.Append( Var0.ToString( wd ) );
			str.Append( "=" );
			str.Append( Index.ToString( wd ) );
			str.Append( "{" );
			
			foreach( IntDomain dom in m_DomainList )
			{
				str.Append( dom.ToString() );
			}
		
			str.Append( "}" );

			return str.ToString();
		}

		public new IntVar Index
		{
			get
			{
				return Var1;
			}
		}

		public override bool IsViolated()
		{
			return false;
		}

		public override void Update()
		{
			UpdateIndex();
			UpdateVar0();
			UpdateVar1();
		}

		private void UpdateVar0()
		{
			IntDomain domain	= IntDomain.Empty;

			foreach( int idx in Var1 )
			{
				domain	= domain.Union( m_DomainList[ idx ] );
			}
		
			Var0.Intersect( domain );
		}

		private void UpdateVar1()
		{
			IntDomain diff	= IntDomain.Empty;

			foreach( int idx in Index )
			{
				IntDomain dom	= m_DomainList[ idx ];

				if( !Var0.Domain.IntersectsWith( dom ) )
				{
					diff	= diff.Union( idx );
				}
			}

			Index.Difference( diff );
		}

		private void UpdateIndex()
		{
			Index.Intersect( 0, m_DomainList.Count - 1 );
		}
		
		IntDomainList m_DomainList;
	}
}

//--------------------------------------------------------------------------------
