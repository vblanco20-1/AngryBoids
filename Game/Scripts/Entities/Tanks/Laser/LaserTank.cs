﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CryEngine;

namespace CryGameCode.Tanks
{
	[Entity(Category = "Tanks")]
	public class LaserTank : Tank
	{
		public override string TurretModel { get { return "objects/tanks/turret_laser.chr"; } }

		public override void FireWeapon(Vec3 mouseWorldPos)
		{
			throw new NotImplementedException();
		}
		
		public override float TankSpeed
		{
			get
			{
				return 10;
			}
		}		
	}
}
