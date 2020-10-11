using System;
using System.Collections.Generic;

namespace KourageousTourists
{
	public class ProtoTourist
	{
		public int level { get; set; }
		public List<String> abilities { get; set; }
		public List<String> situations { get; set; }
		public List<String> celestialBodies { get; set; }
		public double srfspeed { get; set; }
		public bool isSkydiver { get; set; }

		public ProtoTourist ()
		{
			abilities = new List<String> ();
			situations = new List<String> ();
			celestialBodies = new List<String> ();
		}

		public override String ToString()
		{
			return (String.Format("Tourist: < lvl={0}, abilities: [{1}], situations: [{2}], bodies: [{3}], speed: {4:F2}, skydiver: {5} >",
				level, 
				String.Join(", ", abilities.ToArray()),
				String.Join(", ", situations.ToArray()),
				String.Join(", ", celestialBodies.ToArray()), 
				srfspeed, isSkydiver));
		}
	}
}

