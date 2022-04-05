
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Support
{
	public static class Helper
	{
		/// <summary>
		/// Get the inverse of the vector scale.
		/// </summary>
		/// <param name="divisor">The vector that will be a denominator of the division.</param>
		/// <param name="dividend">The vector that will be divided.</param>
		/// <returns>The component-wise division of the dividend by divisor.</returns>
		public static Vector3 GetInverseScale(this Vector3 dividend, Vector3 divisor)
		{
			return new Vector3(dividend.x / divisor.x, dividend.y / divisor.y, dividend.z / divisor.z);
		}

		/// <summary>
		/// To execute an action after wait seconds.
		/// </summary>
		/// <param name="timeToWait">The time waiting until execute the action.</param>
		/// <param name="action">The action that will be executed after the waiting time.</param>
		/// <returns>Default <see cref="IEnumerator"/> return.</returns>
		public static IEnumerator WaitForSeconds(float timeToWait, Action action)
		{
			yield return new WaitForSeconds(timeToWait);
			action?.Invoke();
		}

		/// <summary>
		/// To execute an action after wait for end of frame.
		/// </summary>
		/// <param name="action">The action that will be executed after the waiting time.</param>
		/// <returns>Default <see cref="IEnumerator"/> return.</returns>
		public static IEnumerator WaitUntilEndOfFrame(Action action, int times = 1)
		{
			times = times <= 0 ? 1 : times;
			for (int i = 0; i < times; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			action?.Invoke();
		}

		/// <summary>
		/// Gets the object's bounds considering its children.
		/// </summary>
		/// <param name="parentObject">The object to get bounds.</param>
		/// <returns>The bounds of the object.</returns>
		public static Bounds GetBoundsFromChildren(GameObject parentObject)
		{
			Bounds bounds;
			Renderer[] renderers = parentObject.GetComponentsInChildren<Renderer>();
			bounds = new Bounds(parentObject.transform.position, Vector3.zero);
			foreach (Renderer renderer in renderers)
			{
				bounds.Encapsulate(renderer.bounds);
			}
			return bounds;
		}
		/// <summary>
		/// Resets local values of the <see cref="Transform"/>.
		/// </summary>
		/// <param name="transform">The <see cref="Transform"/> that will be change.</param>
		public static void ResetLocalTransform(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
		}
		/// <summary>
		/// Activates or deactivates the collisions of the <paramref name="objectToGetCollider"/>.
		/// </summary>
		/// <param name="objectToGetCollider">The <see cref="GameObject"/> to get the <see cref="Rigidbody"/>.</param>
		/// <param name="active">Pass true to activate or false to deactivate the collisions of the object.</param>
		public static void SetDetectCollisions(GameObject objectToGetCollider, bool active, bool clearInteractableColliders = false)
		{
			if (clearInteractableColliders && !active)
			{
				CleanInteractableColliders(objectToGetCollider);
			}
			Rigidbody[] rgBodies = objectToGetCollider.transform.GetComponentsInChildren<Rigidbody>(true);
			foreach (Rigidbody item in rgBodies)
			{
				item.detectCollisions = active;
			}
		}
		/// <summary>
		/// Reads all <see cref="XRBaseInteractable"/> on the children to clean the colliders.
		/// </summary>
		/// <param name="gameObjectToRemove">The <see cref="GameObject"/> that will be destroyed later.</param>
		public static void CleanInteractableColliders(GameObject gameObjectToRemove)
		{
			//destroy base interactables
			XRBaseInteractable[] basesInteractables = gameObjectToRemove.transform.GetComponentsInChildren<XRBaseInteractable>();

			//destroy base interactables
			if (basesInteractables != null && basesInteractables.Length > 0)
			{
				foreach (XRBaseInteractable item in basesInteractables)
				{
					item.colliders.Clear();
					//To clean interactable colliders and avoid error on the XRBaseInteractable.
					typeof(XRBaseInteractable).GetField("m_Colliders", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(item, new List<Collider>());
				}
			}
		}
	}
}