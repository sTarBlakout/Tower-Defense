using System.Collections;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    [SerializeField] float waitBeforeDestoy = 1f;
    [SerializeField] float waitBeforeStop = 0f;

    private Coroutine destroyInSecsCoroutine = null;
    private Coroutine stopInSecsCoroutine = null;

    void Update()
    {
        if (transform.GetComponent<ParticleSystem>().isStopped)
        {
            if (destroyInSecsCoroutine == null)
                destroyInSecsCoroutine = StartCoroutine(DestroyInSeconds(waitBeforeDestoy));
        }
        else
        {
            if (waitBeforeStop != 0f)
            {
                stopInSecsCoroutine = StartCoroutine(StopInSeconds(waitBeforeStop));
            }
        }
    }

    private IEnumerator StopInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.transform.GetComponent<ParticleSystem>().Stop();
    }

    private IEnumerator DestroyInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
