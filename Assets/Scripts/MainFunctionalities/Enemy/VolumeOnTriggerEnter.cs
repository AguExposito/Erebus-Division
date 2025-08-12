using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class VolumeOnTriggerEnter : MonoBehaviour
{
    public float originalFOV=80;
    public float targetFOV =110;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone");
            StopCoroutine("ChangeFOV");
            StartCoroutine(ChangeFOV(targetFOV, 0.5f));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine("ChangeFOV");
            StartCoroutine(ChangeFOV(originalFOV, 0.5f));
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
