using System;
using System.Collections.Generic;
using System.Text;

namespace MaraSolver
{
	public class Copier
	{
		public Copier()
		{
			m_Map	= new Dictionary<int,ICopy>();
		}

		public ICopy Get( ICopy other )
		{
			ICopy copy;
			if( !m_Map.TryGetValue( other.Id, out copy ) )
			{
				copy		= other.New();
				other.Id	= m_Map.Count + 1;
				
				m_Map[ other.Id ]	= copy;

				copy.Copy( this, other );
			} 

			return copy;
		}

		Dictionary<int,ICopy>	m_Map;
	}
}
