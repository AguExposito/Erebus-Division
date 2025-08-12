using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class VolumeOnTriggerEnter : MonoBehaviour
{
    public float originalFOV=80;
    public float targetFOV =110;
    public float enterDuration =0.5f;
    public float exitDuration =2f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone");
            StopCoroutine("ChangeFOV");
            StartCoroutine(ChangeFOV(targetFOV, enterDuration));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine("ChangeFOV");
            StartCoroutine(ChangeFOV(originalFOV, exitDuration));
        }
    }

    IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        float startFOV = GameObject.FindGameObjectWithTag("MainCinemachine").GetComponent<CinemachineCamera>().Lens.FieldOfView;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            GameObject.FindGameObjectWithTag("MainCinemachine").GetComponent<CinemachineCamera>().Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            yield return null;
        }
        GameObject.FindGameObjectWithTag("MainCinemachine").GetComponent<CinemachineCamera>().Lens.FieldOfView = targetFOV;
    }
}
