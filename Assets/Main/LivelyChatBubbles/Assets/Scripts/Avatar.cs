using System.Collections;
using UnityEngine;

namespace LivelyChatBubbles
{
	public class Avatar : MonoBehaviour
	{
		[Header("Movement")]
		public bool CanMove = true;

		[Tooltip("Valid movement area box collider.")]
		public BoxCollider MovementArea;

		[Tooltip("Seconds between each random movement.")]
		public float MovementInterval = 5;

		[Tooltip("Multiplier to move speed.")]
		public float MovementSpeed = 1f;

		[Tooltip("Required range for next movement.")]
		public float MovementRange = 10f;

		[Tooltip("Multiplier to the movement bob (Y axis) speed.")]
		public float MovementBobSpeed = 4.0f;

		[Tooltip("Multiplier to the idle bob (Y axis) speed.")]
		public float IdleBobSpeed = 0.25f;

		[Tooltip("Multiplier to the turn speed.")]
		public float TurnSpeed = 5.0f;

		bool isMoving;
		Vector3 targetDestination;

		void OnEnable()
		{
			var osc = GetComponentInChildren<Oscillator>();
			if (osc)
				osc.Speed = IdleBobSpeed;

			if (MovementArea)
				InvokeRepeating("TryMovement", Random.Range(0, MovementInterval), MovementInterval);
		}

		void OnDisable()
		{
			CancelInvoke("TryMovement");
		}

		void TryMovement()
		{
			StartCoroutine(FindDestinationAndMove());
		}

		IEnumerator FindDestinationAndMove()
		{
			isMoving = true;

			var osc = GetComponentInChildren<Oscillator>();
			if (osc)
				osc.Speed = MovementBobSpeed;

			yield return FindDestination();
			yield return Move();

			if (osc)
				osc.Speed = IdleBobSpeed;

			isMoving = false;
		}

		IEnumerator FindDestination()
		{
			do
			{
				yield return null;

				targetDestination = new Vector3(
					Random.Range(MovementArea.bounds.center.x - MovementArea.bounds.extents.x, MovementArea.bounds.center.x + MovementArea.bounds.extents.x),
					transform.position.y,
					Random.Range(MovementArea.bounds.center.z - MovementArea.bounds.extents.z, MovementArea.bounds.center.z + MovementArea.bounds.extents.z));
			}
			while (Vector3.Distance(transform.position, targetDestination) > MovementRange);
		}

		IEnumerator Move()
		{
			while (Vector3.Distance(transform.position, targetDestination) > 0.25)
			{
				if (!CanMove)
					break;
				transform.position = Vector3.Lerp(transform.position, targetDestination, Time.deltaTime * MovementSpeed);
				yield return null;
			}
		}

		void Update()
		{
			if (isMoving)
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDestination - transform.position, TurnSpeed * Time.deltaTime, 0.0F));
		}

	}
}

