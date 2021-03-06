﻿using System;

using CryEngine;
using CryGameCode.Projectiles;

namespace CryGameCode.Tanks
{
	public abstract class TankTurret
	{
        public TankTurret() { }

		public TankTurret(Tank owner)
		{
			Owner = owner;

			Attachment = Owner.GetAttachment("turret");
			Attachment.UseEntityRotation = true;

			Attachment.LoadObject(Model);
			Attachment.Material = Material.Find("objects/tanks/tank_turrets_" + Owner.Team);

            Attachment.OnDestroyed += (x) => { Destroy(); };

            if (Owner.IsLocalClient)
            {
                // Temp hax for right mouse events not working
                Input.ActionmapEvents.Add("attack2", (e) =>
                {
                    switch (e.KeyEvent)
                    {
                        case KeyEvent.OnPress:
                            if (AutomaticFire)
                                m_rightFiring = true;
                            break;

                        case KeyEvent.OnRelease:
                            if (AutomaticFire)
                                m_rightFiring = false;
                            else
                                FireRight();
                            break;
                    }
                });

                Input.MouseEvents += ProcessMouseEvents;
            }
		}

		public void Destroy()
		{
			if(Owner.IsLocalClient)
			{
				Input.MouseEvents -= ProcessMouseEvents;
				Input.ActionmapEvents.RemoveAll(this);
			}
		}

		private void ProcessMouseEvents(MouseEventArgs e)
		{
			switch(e.MouseEvent)
			{
			case MouseEvent.LeftButtonDown:
			{
				if(AutomaticFire)
					m_leftFiring = true;

				ChargeWeapon();
			}
			break;

			case MouseEvent.LeftButtonUp:
			{
				if(AutomaticFire)
					m_leftFiring = false;
				else
					FireLeft();
			}
			break;
			}
		}

		public void Update()
		{
			if(m_leftFiring)
				FireLeft();

			if(m_rightFiring)
				FireRight();

			var dir = Renderer.ScreenToWorld(Input.MouseX, Input.MouseY) - Attachment.Position;
			var rot = Attachment.Rotation;
			rot.SetRotationZ((float)Math.Atan2(-dir.X, dir.Y));

			Attachment.Rotation = Quat.CreateNlerp(Attachment.Rotation, rot, Time.DeltaTime * 10);
		}

		#region Weapons
		protected virtual void ChargeWeapon() { }

		private void Fire(ref float shotTime, string helper)
		{
			if(Time.FrameStartTime > shotTime + (TimeBetweenShots * 1000))
			{
				shotTime = Time.FrameStartTime;

				var jointAbsolute = Attachment.GetJointAbsolute(helper);
				jointAbsolute.T = Attachment.Transform.TransformPoint(jointAbsolute.T) + jointAbsolute.Q * new Vec3(0, 0, 0);
				Entity.Spawn("pain", ProjectileType, jointAbsolute.T, Attachment.Rotation);
				OnFire(jointAbsolute.T);
			}
		}

		protected void FireLeft()
		{
			Fire(ref m_lastleftShot, LeftHelper);
		}

		protected void FireRight()
		{
			if(!string.IsNullOrEmpty(RightHelper))
				Fire(ref m_lastRightShot, RightHelper);
		}

		protected virtual void OnFire(Vec3 firePos) { }

		public virtual string LeftHelper { get { return "turret_term"; } }
		public virtual string RightHelper { get { return string.Empty; } }

		public virtual bool AutomaticFire { get { return false; } }
		public virtual float TimeBetweenShots { get { return 1; } }

		public virtual System.Type ProjectileType { get { return typeof(Bullet); } }

		private float m_lastleftShot;
		private float m_lastRightShot;
		private bool m_rightFiring;
		private bool m_leftFiring;
		#endregion

		public void Hide(bool hide)
		{
			if(!Attachment.IsDestroyed)
				Attachment.Hidden = hide;
		}

		public Tank Owner { get; private set; }
		public Attachment Attachment { get; private set; }

		public abstract string Model { get; }
	}
}
