//--------------------------------------------------------------------------------
// Copyright © 2004-2007 Patrick de Boer
//--------------------------------------------------------------------------------
/* $Log: /Mara/Solver/Float/FltBase/FltVar.cs $
 * 
 * 75    9/04/08 8:29p Patrick
 * Moved interval classes into own project
 * 
 * 74    6/26/08 9:59p Patrick
 * removed copy constructor mechanism
 * 
 * 73    2-03-08 20:59 Patrick
 * added Pow(..)
 * 
 * 72    27-02-08 23:35 Patrick
 * fixed Update(..)
 * 
 * 71    27-02-08 23:23 Patrick
 * added lock
 * 
 * 70    27-02-08 22:23 Patrick
 * added DomainPrev
 * added IsIntervalChanged()
 * 
 * 69    14-01-08 19:57 Patrick
 * refactored expression
 * 
 * 68    11-01-08 22:36 Patrick
 * fixed typo in Index
 * 
 * 67    11-10-07 23:42 Patrick
 * changed copy mechanism
 * 
 * 66    15-09-07 0:29 Patrick
 * 
 * 65    8-08-07 3:07 Patrick
 * added Index to variable
 * 
 * 64    7-08-07 16:05 Patrick
 * added Index property
 * 
 * 63    12-07-07 21:30 Patrick
 * added index
 * 
 * 62    5-07-07 1:38 Patrick
 * new deep copy mechanism
 * 
 * 61    4-07-07 20:23 Patrick
 * added Init() methods
 * 
 * 60    28-06-07 23:01 Patrick
 * change API of Copy(..)
 * 
 * 59    27-06-07 22:16 Patrick
 * added SolverCopier class
 * 
 * 58    20-06-07 22:46 Patrick
 * renamed namespace
 * 
 * 57    6-06-07 0:59 Patrick
 * merged GoalStack into Solver
 * changed Problem to contain instance of Solver
 * 
 * 56    22-03-07 21:09 Patrick
 * refactored Clone()
 */
//--------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using MaraInterval.Interval;
using MaraSolver.Integer;
using MaraInterval.Utility;

//--------------------------------------------------------------------------------
namespace MaraSolver.Float
{
	/// <summary>
	/// Summary description for FltVar.
	/// </summary>
	[DebuggerDisplay("{ToString(true)}")]
	public class FltVar : Variable
	{		
		public FltVar( Solver solver ) :
			this( solver, string.Empty )
		{
		}

		public FltVar( Solver solver, string name ) :
			this( solver, solver.Horizon.Min, solver.Horizon.Max, name )
		{
		}
		
		public FltVar( Solver solver, double i ) :
			this( solver, i, i, string.Empty )
		{
		}

		public FltVar( Solver solver, double min, double max) :
			this( solver, min, max, string.Empty )
		{
		}

		public FltVar( Solver solver, double min, double max, string name ) :
			this( solver, new FltDomain( min, max ), name )
		{
		}

		public FltVar( Solver solver, FltInterval interval, string name ) :
			this( solver, new FltDomain( interval ), name )
		{
		}

		public FltVar( Solver solver, FltDomain domain, string name ) :
			base( solver, name )
		{
			m_Domain			= domain;
			m_DomainPrev		= domain;
			m_DomainPrevInvalid	= false;

			m_Solver.FltVarList.Add( this );
		}

		public override int Index
		{
			get
			{
				return m_Solver.FltVarList.IndexOf( this );
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
				m_Domain			= value as FltDomain;
				m_DomainPrevInvalid	= true;
			}
		}

		public FltDomain DomainPrev
		{
			get
			{
				if( m_DomainPrevInvalid )
				{
					object statePrev	= StatePrev;

					m_DomainPrev		= !ReferenceEquals( statePrev, null )
										? statePrev as FltDomain
										: m_Domain;

					m_DomainPrevInvalid	= false;
				}

				return m_DomainPrev;
			}
		}

		public FltDomain Domain
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
	
		public double Value
		{
			get
			{
				if( !IsBound() )
					throw new Exception( "variable not bound" );
				
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
		
		public double Min
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

		public double Max
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
			FltInterval horizon		= new FltInterval( m_Solver.Horizon.Min, m_Solver.Horizon.Max );
		
			if( !m_Domain.Interval.Equals( horizon ) )
			{
				m_Domain.Union( horizon );

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

		public FltVarCmpEqualIntVar Equals( IntVar v )
		{			
			return new FltVarCmpEqualIntVar( v, this );
		}

		public IntVar ToInt()
		{
			FltVarCmpEqualIntVar cmp	= new FltVarCmpEqualIntVar( new IntVar( m_Solver ), this );
			
			return cmp.IntVar;
		}

		static public implicit operator IntVar( FltVar v )
		{
			return v.ToInt();
		}

		#region Union

		public void Union( double val )
		{
			Update( m_Domain.Union( val ) );
		}

		public void Union( double min, double max )
		{
			Update( m_Domain.Union( min, max ) );
		}

		public void Union( FltInterval interval )
		{
			Update( m_Domain.Union( interval ) );
		}

		public void Union( FltDomain domain )
		{		
			Update( m_Domain.Union( domain ) );
		}

		#endregion
	
		#region Difference

		public void Difference( double val )
		{
			Update( m_Domain.Difference( val ) );
		}

		public void Difference( double min, double max )
		{
			Update( m_Domain.Difference( min, max ) );
		}

		public void Difference( FltInterval interval )
		{
			Update( m_Domain.Difference( interval ) );
		}

		public void Difference( FltDomain domain )
		{
			Update( m_Domain.Difference( domain ) );
		}

		#endregion

		#region Intersect

		public void Intersect( double val )
		{
			Update( m_Domain.Intersect( val ) );
		}

		public void Intersect( double min, double max )
		{
			Update( m_Domain.Intersect( min, max ) );
		}

		public void Intersect( FltInterval interval )
		{
			Update( m_Domain.Intersect( interval ) );
		}

		public void Intersect( FltDomain domain )
		{
			Update( m_Domain.Intersect( domain ) );
		}

		#endregion
		
		internal void Update( FltDomain result )
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
		public static FltVarExpr operator *( FltVar lhs, FltVar rhs )
		{			
			return new FltVarExprMul( lhs, rhs );
		}

		// v0 = v1 / v2
		public static FltVarExpr operator /( FltVar lhs, FltVar rhs )
		{			
			return new FltVarExprDiv( lhs, rhs );
		}

		// v0 = v1 + v2
		public static FltVarExpr operator +( FltVar lhs, FltVar rhs )
		{			
			return new FltVarExprAdd( lhs, rhs );
		}

		// v0 = v1 - v2
		public static FltVarExpr operator -( FltVar lhs, FltVar rhs )
		{			
			return new FltVarExprSub( lhs, rhs );
		}

		#endregion

		#region Var <expr> Val / Val <expr> Var

		// v0 = v1 + val
		public static FltVarExpr operator +( FltVar lhs, double rhs )
		{			
			return new FltVarExprValAdd( lhs, rhs );
		}

		// v0 = val + v1
		public static FltVarExpr operator +( double lhs, FltVar rhs )
		{			
			return new FltVarExprValAddRev( lhs, rhs );
		}

		// v0 = v1 - val
		public static FltVarExpr operator -( FltVar lhs, double rhs )
		{			
			return new FltVarExprValSub( lhs, rhs );
		}

		// v0 = val - v1
		public static FltVarExpr operator -( double lhs, FltVar rhs )
		{			
			return new FltVarExprValSubRev( lhs, rhs );
		}

		// v0 = v1 * val
		public static FltVarExpr operator *( FltVar lhs, double rhs )
		{			
			return new FltVarExprValMul( lhs, rhs );
		}

		// v0 = val * v1
		public static FltVarExpr operator *( double lhs, FltVar rhs )
		{			
			return new FltVarExprValMulRev( lhs, rhs );
		}

		// v0 = v1 / val
		public static FltVarExpr operator /( FltVar lhs, double rhs )
		{			
			return new FltVarExprValDiv( lhs, rhs );
		}

		// v0 = val / v1
		public static FltVarExpr operator /( double lhs, FltVar rhs )
		{			
			return new FltVarExprValDivRev( lhs, rhs );
		}

		#endregion

		#region Var <comp> Var

		public static FltVarCmp operator ==( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpEqual( lhs, rhs );
		}

		public static FltVarCmp operator !=( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpNotEqual( lhs, rhs );
		}

		public static FltVarCmp operator <( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpLess( lhs, rhs );
		}

		public static FltVarCmp operator >( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpLess( rhs, lhs );
		}

		public static FltVarCmp operator <=( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpLessEqual( lhs, rhs );
		}

		public static FltVarCmp operator >=( FltVar lhs, FltVar rhs )
		{			
			return new FltVarCmpLessEqual( rhs, lhs );
		}

		#endregion

		#region Var <comp> val

		public static FltVarCmpVal operator ==( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValEqual( lhs, rhs );
		}

		public static FltVarCmpVal operator !=( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValNotEqual( lhs, rhs );
		}

		public static FltVarCmpVal operator <( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValLess( lhs, rhs );
		}

		public static FltVarCmpVal operator <=( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValLessEqual( lhs, rhs );
		}

		public static FltVarCmpVal operator >( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValGreater( lhs, rhs );
		}

		public static FltVarCmpVal operator >=( FltVar lhs, double rhs )
		{			
			return new FltVarCmpValGreaterEqual( lhs, rhs );
		}

		#endregion

		#region Negate

		public static FltVarNeg operator-( FltVar var )
		{
			return new FltVarNeg( var );
		}

		#endregion


		public FltVarExp Exp()
		{
			return new FltVarExp( this );
		}

		public FltVarLog Log()
		{
			return new FltVarLog( this );
		}

		public FltVarPow Pow( int power )
		{
			return new FltVarPow( this, power );
		}

		public static FltVarPow operator ^( FltVar lhs, int rhs )
		{			
			return new FltVarPow( lhs, rhs );
		}


		FltDomain	m_Domain;
		FltDomain	m_DomainPrev;
		bool		m_DomainPrevInvalid;
	}	
}
