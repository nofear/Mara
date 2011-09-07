//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Integer/IntBase/IntVar.cs $
 * 
 * 88    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 87    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 86    9-03-08 17:54 Patrick
 * added to source control
 * 
 * 85    27-02-08 22:23 Patrick
 * added IsIntervalChanged()
 * 
 * 84    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 83    19-10-07 0:35 Patrick
 * added demonlist
 * 
 * 82    16-10-07 0:43 Patrick
 * added threaded propagation queue
 * 
 * 81    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 80    15-09-07 0:29 Patrick
 * 
 * 79    8-08-07 3:07 Patrick
 * added Index to variable
 * 
 * 78    7-08-07 16:05 Patrick
 * added Index property
 * 
 * 77    12-07-07 21:31 Patrick
 * added index
 * 
 * 76    7-07-07 15:18 Patrick
 * added ISolverCopy to SolverBase
 * 
 * 75    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 74    4-07-07 20:23 Patrick
 * added Init() methods
 * 
 * 73    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 72    27-06-07 22:17 Patrick
 * added SolverCopier class
 * 
 * 71    20-06-07 22:46 Patrick
 * renamed namespace
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraSolver.Float;

//--------------------------------------------------------------------------------
namespace MaraSolver.Integer
{
	/// <summary>
	/// Summary description for IntVar.
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class IntVar : Variable, IEnumerable<int>
	{		
		public IntVar( Solver solver ) :
			this( solver, string.Empty )
		{
		}

		public IntVar( Solver solver, string name ) :
			this( solver, solver.Horizon, name )
		{
		}
		
		public IntVar( Solver solver, int val ) :
			this( solver, val, val, string.Empty )
		{
		}

		public IntVar( Solver solver, int min, int max ) :
			this( solver, min, max, string.Empty )
		{
		}

		public IntVar( Solver solver, int min, int max, string name ) :
			this( solver, new IntInterval( min, max ), name )
		{
		}

		public IntVar( Solver solver, IntInterval interval, string name ) :
			this( solver, new IntDomain( interval ), name )
		{
		}

		public IntVar( Solver solver, IntDomain domain, string name ) :
			base( solver, name )
		{
			m_Domain			= domain;
			m_DomainPrev		= domain;
			m_DomainPrevInvalid	= false;

			m_Solver.IntVarList.Add( this );
		}

		public override int Index
		{
			get
			{
				return m_Solver.IntVarList.IndexOf( this );
			}
		}

		public override string DefaultName
		{
			get
			{
				return "i" + Index.ToString( CultureInfo.CurrentCulture );
			}
		}

		public override string ToString( bool withDomain )
		{
			string str	= ( Name != "" )
						? Name
						: DefaultName;
		
			if( withDomain )
			{
				str		+= ":" + m_Domain.ToString();
			}
			
			return str;
		}

		public override string ToString()
		{		
			return ToString( true ); 
		}
		
		public override bool Equals( object obj )
		{
			return ReferenceEquals( this, obj );
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override object State
		{
			get
			{				
				return m_Domain;
			}

			set
			{
				m_Domain			= value as IntDomain;
				m_DomainPrevInvalid	= true;
			}
		}

		public IntDomain DomainPrev
		{
			get
			{
				if( m_DomainPrevInvalid )
				{
					object statePrev	= StatePrev;

					m_DomainPrev		= !ReferenceEquals( statePrev, null )
										? statePrev as IntDomain
										: m_Domain;

					m_DomainPrevInvalid	= false;
				}

				return m_DomainPrev;
			}
		}

		public IntDomain Domain
		{
			get
			{
				return m_Domain;
			}

			set
			{
				lock( this )
				{
					Intersect( value );
				}
			}
		}
	
		public int Value
		{
			get
			{
				//if( !IsBound() )
					//throw new Exception( "variable not bound" );
				
				return m_Domain.Min;
			}
			
			set
			{
				if( !m_Domain.Contains( value ) )
				{
					UnboundDirect();
				}
			
				Intersect( value );
			}
		}
		
		public int Min
		{
			get
			{
				return m_Domain.Min;
			}

			set
			{
				Intersect( value , m_Solver.Horizon.Max );
			}
		}

		public int Max
		{
			get
			{
				return m_Domain.Max;
			}
		
			set
			{
				Intersect( m_Solver.Horizon.Min, value );
			}	
		}

		public override void UnboundDirect()
		{
			if( !m_Domain.Interval.Equals( m_Solver.Horizon ) )
			{
				m_Domain.Union( m_Solver.Horizon );

				base.UnboundDirect();
			}
		}
		
		public override bool IsIntervalChanged()
		{
			return m_Domain.Interval != DomainPrev.Interval;
		}
		
		public override bool IsBound()
		{
			return m_Domain.Interval.IsBound();
		}
		
		public override bool IsEmpty()
		{
			return m_Domain.IsEmpty();
		}

		public FltVarCmpEqualIntVar Equals( FltVar v )
		{			
			return new FltVarCmpEqualIntVar( this, v );
		}

		public FltVar ToFlt()
		{
			FltVarCmpEqualIntVar cmp	= new FltVarCmpEqualIntVar( this, new FltVar( m_Solver ) );
			
			return cmp.FltVar;
		}

		static public implicit operator FltVar( IntVar v )
		{
			return v.ToFlt();
		}

		#region Union

		public void Union( int val )
		{
			Update( m_Domain.Union( val ) );
		}

		public void Union( int min, int max )
		{
			Update( m_Domain.Union( min, max ) );
		}

		public void Union( IntInterval interval )
		{
			Update( m_Domain.Union( interval ) );
		}

		public void Union( IntDomain domain )
		{		
			Update( m_Domain.Union( domain ) );
		}

		#endregion
	
		#region Difference

		public void Difference( int val )
		{
			Update( m_Domain.Difference( val ) );
		}

		public void Difference( int min, int max )
		{
			Update( m_Domain.Difference( min, max ) );
		}

		public void Difference( IntInterval interval )
		{
			Update( m_Domain.Difference( interval ) );
		}

		public void Difference( IntDomain domain )
		{
			Update( m_Domain.Difference( domain ) );
		}

		#endregion

		#region Intersect

		public void Intersect( int val )
		{
			Update( m_Domain.Intersect( val ) );
		}

		public void Intersect( int min, int max )
		{
			Update( m_Domain.Intersect( min, max ) );
		}

		public void Intersect( IntInterval interval )
		{
			Update( m_Domain.Intersect( interval ) );
		}

		public void Intersect( IntDomain domain )
		{
			Update( m_Domain.Intersect( domain ) );
		}

		#endregion
		
		internal void Update( IntDomain result )
		{
			if( !ReferenceEquals( m_Domain, result ) )
			{
				Store();
				
				m_DomainPrevInvalid	= false;
				m_DomainPrev		= m_Domain;
				m_Domain			= result;
				
				OnDomainChange();
			}
		}

		#region Var <expr> Var
		
		// v0 = v1 * v2
		public static IntVarExprVar operator *( IntVar lhs, IntVar rhs )
		{			
			return new IntVarExprVarMul( lhs, rhs );
		}

		// v0 = v1 / v2
		public static IntVarExprVar operator /( IntVar lhs, IntVar rhs )
		{			
			return new IntVarExprVarDiv( lhs, rhs );
		}

		// v0 = v1 + v2
		public static IntVarExprVar operator +( IntVar lhs, IntVar rhs )
		{			
			return new IntVarExprVarAdd( lhs, rhs );
		}

		// v0 = v1 - v2
		public static IntVarExprVar operator -( IntVar lhs, IntVar rhs )
		{			
			return new IntVarExprVarSub( lhs, rhs );
		}

		#endregion

		#region Var <expr> Val / Val <expr> Var

		// v0 = v1 + val
		public static IntVarExprVal operator +( IntVar lhs, int rhs )
		{			
			return new IntVarExprValAdd( lhs, rhs );
		}

		// v0 = val + v1
		public static IntVarExprVal operator +( int lhs, IntVar rhs )
		{			
			return new IntVarExprValAddRev( lhs, rhs );
		}

		// v0 = v1 - val
		public static IntVarExprVal operator -( IntVar lhs, int rhs )
		{			
			return new IntVarExprValSub( lhs, rhs );
		}

		// v0 = val - v1
		public static IntVarExprVal operator -( int lhs, IntVar rhs )
		{			
			return new IntVarExprValSubRev( lhs, rhs );
		}

		// v0 = v1 * val
		public static IntVarExprVal operator *( IntVar lhs, int rhs )
		{			
			return new IntVarExprValMul( lhs, rhs );
		}

		// v0 = val * v1
		public static IntVarExprVal operator *( int lhs, IntVar rhs )
		{			
			return new IntVarExprValMulRev( lhs, rhs );
		}

		// v0 = v1 / val
		public static IntVarExprVal operator /( IntVar lhs, int rhs )
		{			
			return new IntVarExprValDiv( lhs, rhs );
		}

		// v0 = val / v1
		public static IntVarExprVal operator /( int lhs, IntVar rhs )
		{			
			return new IntVarExprValDivRev( lhs, rhs );
		}

		#endregion

		#region Var <comp> Var

		public static IntVarCmp operator ==( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpEqual( lhs, rhs );
		}

		public static IntVarCmp operator !=( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpNotEqual( lhs, rhs );
		}

		public static IntVarCmp operator <( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpLess( lhs, rhs );
		}

		public static IntVarCmp operator >( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpLess( rhs, lhs );
		}

		public static IntVarCmp operator <=( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpLessEqual( lhs, rhs );
		}

		public static IntVarCmp operator >=( IntVar lhs, IntVar rhs )
		{			
			return new IntVarCmpLessEqual( rhs, lhs );
		}

		#endregion

		#region Var <comp> val

		public static IntVarCmpVal operator ==( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValEqual( lhs, rhs );
		}

		public static IntVarCmpVal operator !=( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValNotEqual( lhs, rhs );
		}

		public static IntVarCmpVal operator <( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValLess( lhs, rhs );
		}

		public static IntVarCmpVal operator >( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValGreater( lhs, rhs );
		}

		public static IntVarCmpVal operator <=( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValLessEqual( lhs, rhs );
		}

		public static IntVarCmpVal operator >=( IntVar lhs, int rhs )
		{			
			return new IntVarCmpValGreaterEqual( lhs, rhs );
		}
		
		#endregion

		#region Negate

		public static IntVarNeg operator-( IntVar var )
		{
			return new IntVarNeg( var );
		}

		#endregion

		#region ValueEnumerator

		// Inner class implements IEnumerator interface:
		private sealed class ValueEnumerator : IEnumerator<int>
		{
			int							m_Value;
			bool						m_IndexMoveNext;
			IEnumerator<IntInterval>	m_Index;

			public ValueEnumerator( IntVar var )
			{
				m_Index		= var.Domain.GetEnumerator();

				Reset();
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if( m_IndexMoveNext )
				{
					if( m_Value < m_Index.Current.Max )
					{
						++m_Value;
					}
					else
					{
						m_IndexMoveNext	= m_Index.MoveNext();
						if( m_IndexMoveNext )
						{
							m_Value		= m_Index.Current.Min;
						}
						else
						{
							m_Value		= 0;
						}
					}
				}
								
				return m_IndexMoveNext;
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				m_Index.Reset();
				m_IndexMoveNext	= m_Index.MoveNext();
				m_Value			= ( m_IndexMoveNext )
								? m_Index.Current.Min - 1
								: int.MinValue;
			}

			#region IEnumerator<int> Members

			public int Current
			{
				get
				{
					return m_Value;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get
				{
					return m_Value;
				}
			}

			#endregion
		}

		#endregion

		#region IEnumerable<int> Members

		public IEnumerator<int> GetEnumerator()
		{
			return new ValueEnumerator( this );
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ValueEnumerator( this );
		}

		#endregion

		IntDomain		m_Domain;
		IntDomain		m_DomainPrev;
		bool			m_DomainPrevInvalid;
	}	
}
