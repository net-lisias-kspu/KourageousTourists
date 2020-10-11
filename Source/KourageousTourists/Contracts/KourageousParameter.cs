using System;
using Contracts;

namespace KourageousTourists.Contracts
{
	public abstract class KourageousParameter : ContractParameter
	{
		protected CelestialBody targetBody;
		protected string tourist;

		public KourageousParameter() {
			this.targetBody = Planetarium.fetch.Home;
			this.tourist = "Unknown";
		}

		public KourageousParameter(CelestialBody target, String kerbal) {
			this.targetBody = target;
			this.tourist = String.Copy(kerbal);
		}

		protected override void OnLoad (ConfigNode node)
		{
			int bodyID = int.Parse(node.GetValue ("targetBody"));
			foreach (var body in FlightGlobals.Bodies)
				if (body.flightGlobalsIndex == bodyID) {
					targetBody = body;
					break;
				}

			this.tourist = String.Copy(node.GetValue ("tourist"));
		}

		protected override void OnSave (ConfigNode node)
		{
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue ("targetBody", bodyID);
			node.AddValue ("tourist", tourist);
		}
	}
}

