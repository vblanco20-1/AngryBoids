﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CryEngine;

namespace CryGameCode.Tanks
{
	[Entity(Category = "Tanks")]
	public class RocketTank : Tank
	{
		public override string TurretModel { get { return "objects/tanks/turret_rocket.chr"; } }
	}
}
