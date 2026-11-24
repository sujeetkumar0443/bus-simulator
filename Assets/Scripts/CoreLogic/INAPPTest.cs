using UnityEngine;

public class INAPPTest : MonoBehaviour
{
	private void Start()
	{
	}

	public void onInitialized()
	{
		Debug.Log("IAP onInitialized");
	}

	public void onProductRequestSuccess()
	{
		Debug.Log("IAP onProductRequestSuccess");
	}

	public void onProductRequestFailure()
	{
		Debug.Log("IAP onProductRequestFailure");
	}

	public void onSuccess()
	{
		Debug.Log("IAP onSuccess");
	}

	public void onFailure()
	{
		Debug.Log("IAP onFailure");
	}

	public void onCanceled()
	{
		Debug.Log("IAP onCanceled");
	}

	public void onRestored()
	{
		Debug.Log("IAP onRestored");
	}

	public void onRestoreComplete()
	{
		Debug.Log("IAP onProductRequestFailure");
	}
}
