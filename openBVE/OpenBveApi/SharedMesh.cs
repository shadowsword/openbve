﻿using System;
using OpenBveApi.Math;

namespace OpenBveApi.Objects {

	/// <summary>Represents a vertex used in a shared mesh.</summary>
	public struct SharedVertex {
		// --- members ---
		/// <summary>A reference to the list of spatial coordinates in the underlying shared mesh.</summary>
		public int SpatialCoordinates;
		/// <summary>A reference to the list of texture coordinates in the underlying shared mesh.</summary>
		public int TextureCoordinates;
		/// <summary>A reference to the list of normals in the underlying shared mesh.</summary>
		public int Normal;
	}

	/// <summary>Represents a face used in a shared mesh.</summary>
	public struct SharedFace {
		// --- members ---
		/// <summary>The vertices of this face.</summary>
		public SharedVertex[] Vertices;
		/// <summary>The material used by this face.</summary>
		public AbstractMaterial Material;
		// --- functions ---
		/// <summary>Flips the face.</summary>
		public void Flip() {
			Array.Reverse(this.Vertices);
		}
	}
	
	/// <summary>Represents a mesh with coordinates and colors shared between faces.</summary>
	public class SharedMesh : StaticObject {
		// --- members ---
		/// <summary>The list of unique spatial coordinates.</summary>
		public Vector3[] SpatialCoordinates;
		/// <summary>The list of unique texture coordinates.</summary>
		public Vector2[] TextureCoordinates;
		/// <summary>The list of unique normals.</summary>
		public Vector3[] Normals;
		/// <summary>The faces stored in this mesh.</summary>
		public SharedFace[] Faces;
		// --- functions ---
		/// <summary>Translates the object by the specified offset.</summary>
		/// <param name="offset">The offset by which to translate.</param>
		public override void Translate(Vector3 offset) {
			for (int i = 0; i < this.SpatialCoordinates.Length; i++) {
				this.SpatialCoordinates[i].Translate(offset);
			}
		}
		/// <summary>Translates the object by the specified offset that is measured in the specified orientation.</summary>
		/// <param name="orientation">The orientation along which to translate.</param>
		/// <param name="offset">The offset measured in the specified orientation.</param>
		public override void Translate(Orientation3 orientation, Vector3 offset) {
			for (int i = 0; i < this.SpatialCoordinates.Length; i++) {
				this.SpatialCoordinates[i].Translate(orientation, offset);
			}
		}
		/// <summary>Rotates the object around the specified axis.</summary>
		/// <param name="direction">The axis along which to rotate.</param>
		/// <param name="cosineOfAngle">The cosine of the angle by which to rotate.</param>
		/// <param name="sineOfAngle">The sine of the angle by which to rotate.</param>
		public override void Rotate(Vector3 direction, double cosineOfAngle, double sineOfAngle) {
			for (int i = 0; i < this.SpatialCoordinates.Length; i++) {
				this.SpatialCoordinates[i].Rotate(direction, cosineOfAngle, sineOfAngle);
			}
			for (int i = 0; i < this.Normals.Length; i++) {
				this.Normals[i].Rotate(direction, cosineOfAngle, sineOfAngle);
			}
		}
		/// <summary>Rotates the object from the default orientation into the specified orientation.</summary>
		/// <param name="orientation">The target orientation.</param>
		/// <remarks>The default orientation is X = {1, 0, 0), Y = {0, 1, 0} and Z = {0, 0, 1}.</remarks>
		public override void Rotate(Orientation3 orientation) {
			for (int i = 0; i < this.SpatialCoordinates.Length; i++) {
				this.SpatialCoordinates[i].Rotate(orientation);
			}
			for (int i = 0; i < this.Normals.Length; i++) {
				this.Normals[i].Rotate(orientation);
			}
		}
		/// <summary>Scales the object by the specified factor.</summary>
		/// <param name="factor">The factor by which to scale.</param>
		/// <exception cref="System.ArgumentException">Raised when any component in the factor is zero.</exception>
		public override void Scale(Vector3 factor) {
			if (factor.X == 0.0 | factor.Y == 0.0 | factor.Z == 0.0) {
				throw new ArgumentException("The factor contains components that are zero.");
			}
			for (int i = 0; i < this.SpatialCoordinates.Length; i++) {
				this.SpatialCoordinates[i].Scale(factor);
			}
			double inverseFactorX = 1.0 / factor.X;
			double inverseFactorY = 1.0 / factor.Y;
			double inverseFactorZ = 1.0 / factor.Z;
			double inverseFactorSquaredX = inverseFactorX * inverseFactorX;
			double inverseFactorSquaredY = inverseFactorY * inverseFactorY;
			double inverseFactorSquaredZ = inverseFactorZ * inverseFactorZ;
			for (int i = 0; i < this.Normals.Length; i++) {
				double normalSquaredX = this.Normals[i].X * this.Normals[i].X;
				double normalSquaredY = this.Normals[i].Y * this.Normals[i].Y;
				double normalSquaredZ = this.Normals[i].Z * this.Normals[i].Z;
				double norm = normalSquaredX * inverseFactorSquaredX + normalSquaredY * inverseFactorSquaredY + normalSquaredZ * inverseFactorSquaredZ;
				if (norm != 0.0) {
					double scalar = System.Math.Sqrt((normalSquaredX + normalSquaredY + normalSquaredZ) / norm);
					this.Normals[i].X *= inverseFactorX * scalar;
					this.Normals[i].Y *= inverseFactorY * scalar;
					this.Normals[i].Z *= inverseFactorZ * scalar;
				}
			}
			if (factor.X * factor.Y * factor.Z < 0.0) {
				for (int i = 0; i < this.Faces.Length; i++) {
					this.Faces[i].Flip();
				}
			}
			
		}
	}
	
}
