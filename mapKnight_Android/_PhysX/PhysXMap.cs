using System;
using System.Collections.Generic;

using mapKnight.Values;

namespace mapKnight.Android.PhysX
{
	public class PhysXMap : Map
	{
		public const int TILE_BOX_SIZE = 5;
		private List<PhysXEntity> addedEntitys = new List<PhysXEntity> ();
		private fVector2D Gravity = new fVector2D (0, -10);

		public PhysXMap (string name) : base (name)
		{
			
		}

		public PhysXMap (string name, bool isDev) : base (name, isDev)
		{
		}

		public void AddEntity (PhysXEntity entity)
		{
			addedEntitys.Add (entity);
		}

		public void RemoveEntity (PhysXEntity entity)
		{
			if (addedEntitys.Contains (entity)) {
				addedEntitys.Remove (entity);
			}
		}

		public void Step (float time)
		{
			time /= 1000f;

			foreach (PhysXEntity entity in addedEntitys) {
				if (entity.CollisionMask.HasFlag (PhysXFlag.Map)) {

					// move on X
					bool moved = false;

					if (entity.Velocity.X > 0) {
						// right
						for (int x = (int)((entity.Position.X + entity.Bounds.Width) / TILE_BOX_SIZE); x <= (int)(entity.Position.X + entity.Bounds.Width + entity.Velocity.X * time) / TILE_BOX_SIZE; x++) {
							if (this.GetTile (x, (int)(entity.Position.Y / TILE_BOX_SIZE)) != Tile.Air) {
								entity.Position.X = (float)(x * TILE_BOX_SIZE) - (float)entity.Bounds.Width - 0.00001f;
								entity.Velocity.X = 0;
								moved = true;
								break;
							}
						}
					} else if (entity.Velocity.X < 0) {
						// left
						for (int x = (int)(entity.Position.X / TILE_BOX_SIZE); x >= (int)(entity.Position.X + entity.Velocity.X * time) / TILE_BOX_SIZE; x--) {
							if (this.GetTile (x, (int)(entity.Position.Y / TILE_BOX_SIZE)) != Tile.Air) {
								entity.Position.X = (float)((x + 1) * TILE_BOX_SIZE);
								entity.Velocity.X = 0;
								moved = true;
								break;
							}
						}
					}
					if (!moved) {
						entity.Position.X += entity.Velocity.X * time; 
					}
					entity.Velocity.X += (this.Gravity.X + entity.Acceleration.X) * time;

					moved = false;
					// move on Y
					if (entity.Velocity.Y > 0) {
						for (int y = (int)((entity.Position.Y + entity.Bounds.Height) / TILE_BOX_SIZE); y <= (entity.Position.Y + entity.Velocity.Y * time + entity.Bounds.Height) / TILE_BOX_SIZE; y++) {
							if (moved)
								break;
							for (int x = (int)(entity.Position.X / TILE_BOX_SIZE); x <= (int)((entity.Position.X + entity.Bounds.Width) / TILE_BOX_SIZE); x++) {
								if (this.GetTile (x, y) != Tile.Air) {
									entity.Position.Y = y * TILE_BOX_SIZE - entity.Bounds.Height;
									entity.Velocity.Y = 0;
									moved = true;
								}
							}
						}
					} else if (entity.Velocity.Y < 0) {
						for (int y = (int)(entity.Position.Y / TILE_BOX_SIZE); y >= (int)(entity.Position.Y + entity.Velocity.Y * time) / TILE_BOX_SIZE; y--) {
							if (moved)
								break;
							for (int x = (int)(entity.Position.X / TILE_BOX_SIZE); x <= (int)((entity.Position.X + entity.Bounds.Width) / TILE_BOX_SIZE); x++) {
								if (this.GetTile (x, y) != Tile.Air) {
									entity.Position.Y = (y + 1) * TILE_BOX_SIZE;
									entity.Velocity.Y = 0;
									moved = true;
									break;
								}
							}
						}
					}
					if (!moved) {
						entity.Position.Y += entity.Velocity.Y * time; 
					}
					entity.Velocity.Y += (this.Gravity.Y + entity.Acceleration.Y) * time;
				}

				entity.Position.Y = Math.Min (Math.Max (0f, entity.Position.Y), Height * TILE_BOX_SIZE - entity.Bounds.Height);
				entity.Position.X = Math.Min (Math.Max (0f, entity.Position.X), Width * TILE_BOX_SIZE - entity.Bounds.Width);

				Log.All (typeof(Content), entity.Position.ToString (), MessageType.Debug);
				Log.All (typeof(Content), entity.Velocity.ToString (), MessageType.Debug);
			}
		}
	}
}

