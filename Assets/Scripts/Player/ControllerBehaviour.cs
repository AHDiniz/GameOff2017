using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ControllerBehaviour : MonoBehaviour
{
	
	#region constants
		const float skinWidth = .15f; // The width of the part of the collider that won't detect collisions
	#endregion

	#region publicReferences
		[Header("Amount of Raycasts")]
		public int horizontalRayCount = 4; // amount of rays in the sides of the player
		public int verticalRayCount = 4; // amount of rays upwards and downwards the player
		[Header("Collision Detection")]
		public LayerMask collisionMask; // where the collision will be detected
		public CollisionInfo colInfo; // where the collision is happening
		[Header("Slope Angles")]
		public float maxClimbAngle = 45.0f; // the maximum angle that the player can climb in a slope
		public float maxDescendAngle = 60.0f; // the maximum angle that the player can descend in a slope
    #endregion

    #region privateReferences
    	Collider2D playerCollider; // the player collider component
		RaycastOrigins raycastOrigins; // the origin positions of the raycasts
		float horizontalRaySpacing, verticalRaySpacing; // the distance between each ray
	#endregion

	struct RaycastOrigins // The limits in wich the player will be able to raycast
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo // Checking if the player is colliding with something
	{
		public bool above, below;
		public bool left, right;
		public bool climbingSlope, descendingSlope;
		public float slopeAngle, previousSlopeAngle;
		public Vector3 previousVelocity;

		public void Reset() // Reseting the values when the player isn't colliding 
		{
			above = below = false;
			left = right = false;
			descendingSlope = climbingSlope = false;
			previousSlopeAngle = slopeAngle;
			slopeAngle = .0f;
		}
	}

	public void Init() // The initializer
	{
		playerCollider = GetComponent<Collider2D>();
		CalculateRaySpacing();
	}

	public void Move(Vector3 v) // Making the object move
	{
		UpdateRaycastOrigins();
		colInfo.Reset();
		colInfo.previousVelocity = v;
		if (v.y < 0)
			DescendSlope(ref v);
		if (v.x != 0)
			HorizontalCollisions(ref v);
		if (v.y != 0)
			VerticalCollisions(ref v);
		transform.Translate(v);
	}

	void HorizontalCollisions(ref Vector3 v) // Detecting horizontal collisions
	{
		float directionX = Mathf.Sign(v.x);
		float rayLenght = Mathf.Abs(v.x) + skinWidth;
		for (int i = 0; i < horizontalRayCount; i ++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * ((horizontalRaySpacing * i));
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector2.right * directionX), rayLenght, collisionMask);
			Debug.DrawRay(rayOrigin, ((Vector2.right * directionX) * rayLenght), Color.blue);
			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (i == 0 && slopeAngle <= maxClimbAngle)
				{
					if (colInfo.descendingSlope)
					{
						colInfo.descendingSlope = false;
						v = colInfo.previousVelocity;
					}
					float distanceToSlopeStart = .0f;
					if (slopeAngle != colInfo.previousSlopeAngle)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						v.x -= (distanceToSlopeStart * directionX);
					}
					ClimbSlope(ref v, slopeAngle);
					v.x += (distanceToSlopeStart * directionX);
				}
				if (!colInfo.climbingSlope || slopeAngle > maxClimbAngle)
				{
					v.x = ((hit.distance - skinWidth) * directionX);
					rayLenght = hit.distance;
					if (colInfo.climbingSlope)
					{
						v.y = Mathf.Tan(colInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(v.x);
					}
					colInfo.right = (directionX == 1);
					colInfo.left = (directionX == -1);
				}
			}
		}
	}

	void VerticalCollisions(ref Vector3 v) // Detecting vertical collisions
	{
		float directionY = Mathf.Sign(v.y);
		float rayLenght = Mathf.Abs(v.y) + skinWidth;
		for (int i = 0; i < verticalRayCount; i ++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * ((verticalRaySpacing * i) + v.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector2.up * directionY), rayLenght, collisionMask);
			Debug.DrawRay(rayOrigin, ((Vector2.up * directionY) * rayLenght), Color.red);
			if (hit)
			{
				v.y = ((hit.distance - skinWidth) * directionY);
				rayLenght = hit.distance;
				if (colInfo.climbingSlope)
				{
					v.x = v.y / Mathf.Tan(colInfo.slopeAngle) * Mathf.Sign(v.x);
				}
				colInfo.above = (directionY == 1);
				colInfo.below = (directionY == -1);
			}
		}
		if (colInfo.climbingSlope)
		{
			float directionX = Mathf.Sign(v.x);
			rayLenght = Mathf.Abs(v.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + (Vector2.up * v.y);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector2.right * directionX), rayLenght, collisionMask);
			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != colInfo.slopeAngle)
				{
					v.x = ((hit.distance - skinWidth) * directionX);
					colInfo.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 v, float s) // making the object climb a slope
	{
		float moveDistace = Mathf.Abs(v.x);
		float climbVelY = ((Mathf.Sin(s * Mathf.Deg2Rad)) * moveDistace);
		if (v.y <= climbVelY)
		{
			v.y = climbVelY;
			v.x = (((Mathf.Cos(s * Mathf.Deg2Rad)) * moveDistace) * Mathf.Sign(v.x));
			colInfo.below = true;
			colInfo.climbingSlope = true;
			colInfo.slopeAngle = s;
		}
	}

	void DescendSlope(ref Vector3 v) // making the object descend a slope
	{
		float directionX = Mathf.Sign(v.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == directionX)
				{
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(v.x))
					{
						float moveDistace = Mathf.Abs(v.x);
						float descVelY = ((Mathf.Sin(slopeAngle * Mathf.Deg2Rad)) * moveDistace);
						v.x = (((Mathf.Cos(slopeAngle * Mathf.Deg2Rad)) * moveDistace) * Mathf.Sign(v.x));
						v.y -= descVelY;
						colInfo.slopeAngle = slopeAngle;
						colInfo.descendingSlope = true;
						colInfo.below = true;
					}
				}
			}
		}
	}

	void UpdateRaycastOrigins() // Updating the positions where the rays will be cast
	{
		Bounds bounds = playerCollider.bounds;
		bounds.Expand(skinWidth * -2);
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() // calculating the distance between each ray
	{
		Bounds bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2);
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
}
