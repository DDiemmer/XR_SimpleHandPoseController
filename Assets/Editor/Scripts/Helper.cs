
using System;
using System.Collections;
using UnityEngine;

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

	}
}