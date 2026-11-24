using UnityEngine;
using UnityEngine.AI;

public class PathFinding : MonoBehaviour
{
	public static PathFinding instance;

	private void Start()
	{
		instance = this;
	}

	public void SetPathTarget(GameObject target, Transform pos)
	{
		base.transform.position = pos.position;
		GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
	}
}
